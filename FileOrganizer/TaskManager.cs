using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace FileOrganizer
{
    public class TaskManager
    {
        private readonly object _sync = new object();
        private ManualResetEvent PauseEvent { get; } = new ManualResetEvent(true);
        private Dictionary<string, List<FileInfo>> DictTargetFolderToSrcFiles { get; } = new Dictionary<string, List<FileInfo>>();
        private List<string> FilesProcessed { get; } = new List<string>();
        private List<string> FoldersCreated { get; } = new List<string>();
        private List<Tuple<Task, CancellationTokenSource>> Workers { get; } = new List<Tuple<Task, CancellationTokenSource>>();
        private Tuple<Task, CancellationTokenSource> RevertWorker { get; set; } = null;

        private void InitialiseWork(string srcDir, string dstDir)
        {
            DictTargetFolderToSrcFiles.Clear();
            FilesProcessed.Clear();
            FoldersCreated.Clear();

            DirectoryInfo info = new DirectoryInfo(srcDir);
            foreach (var file in info.GetFiles("*.*", SearchOption.AllDirectories))
            {
                string targetDir = FileOrganizerUtility.PrepareTargetDirFromFile(dstDir, file);

                if (!DictTargetFolderToSrcFiles.ContainsKey(targetDir))
                {
                    DictTargetFolderToSrcFiles.Add(targetDir, new List<FileInfo>() {file});
                }
                else
                {
                    DictTargetFolderToSrcFiles[targetDir].Add(file);
                }
            }
        }

        private KeyValuePair<string, List<FileInfo>>? GetWork()
        {
            lock (_sync)
            {
                if (DictTargetFolderToSrcFiles.Count > 0)
                {
                    KeyValuePair<string, List<FileInfo>> item = DictTargetFolderToSrcFiles.ElementAt(0);
                    DictTargetFolderToSrcFiles.Remove(item.Key);
                    return item;
                }

                return null;
            }
        }

        private void Work(
            CancellationTokenSource tokenSrc,
            Action<string> UpdateStatus)
        {
            while (!tokenSrc.IsCancellationRequested)
            {
                PauseEvent.WaitOne();

                KeyValuePair<string, List<FileInfo>>? work = GetWork();
                if (work == null)
                {
                    //// task exiting. No more files to process.
                    return;
                }

                string targetFolder = work.Value.Key;
                List<FileInfo> srcFiles = work.Value.Value;

                if (!Directory.Exists(targetFolder))
                {
                    DirectoryInfo inf = new DirectoryInfo(targetFolder);
                    while (!inf.Parent.Exists)
                    {
                        inf = inf.Parent;
                    }

                    UpdateStatus?.Invoke($"Creating directory: {targetFolder}");
                    Directory.CreateDirectory(targetFolder);
                    if (!FoldersCreated.Contains(inf.FullName))
                    {
                        lock (_sync)
                        {
                            if (!FoldersCreated.Any(x => inf.FullName.Contains(x)))
                            {
                                FoldersCreated.Add(inf.FullName);
                            }
                        }
                    }
                }

                foreach (FileInfo srcFile in srcFiles)
                {
                    PauseEvent.WaitOne();
                    if (tokenSrc.IsCancellationRequested)
                    {
                        break;
                    }

                    UpdateStatus?.Invoke($"Processing: {srcFile.FullName}");
                    string dstFile = FileOrganizerUtility.GetTargetFileName(
                        srcFile.FullName,
                        targetFolder);

                    if (File.Exists(dstFile))
                    {
                        dstFile = FileOrganizerUtility.AddGuidToFileNameConflict(dstFile);
                    }

                    if (srcFile.CopyTo(dstFile).Exists != true)
                    {
                        throw new IOException(
                            $"Failed to copy file from '{srcFile.FullName}' to '{dstFile}'");
                    }
                    FilesProcessed.Add(dstFile);
                    UpdateStatus?.Invoke($"Copied source: {srcFile.FullName} to destination: {dstFile}");
                }
            }
        }

        private void Revert(
            CancellationTokenSource tokenSrc,
            Action<string> UpdateStatus)
        {
            while (FilesProcessed.Count > 0)
            {
                PauseEvent.WaitOne();
                UpdateStatus($"Deleting file: {FilesProcessed[0]}");
                File.Delete(FilesProcessed[0]);
                FilesProcessed.RemoveAt(0);
            }

            while (FoldersCreated.Count > 0)
            {
                PauseEvent.WaitOne();
                UpdateStatus($"Deleting folder: {FoldersCreated[0]}");
                Directory.Delete(FoldersCreated[0], true);
                FoldersCreated.RemoveAt(0);
            }
        }

        public void PauseWork()
        {
            PauseEvent.Reset();
        }

        public void ResumeWork()
        {
            PauseEvent.Set();
        }

        public void StopWork()
        {
            PauseEvent.Set();
            Workers.ForEach(x => x.Item2.Cancel());
            RevertWorker?.Item2.Cancel();
        }

        public int StartWork(
            uint noOfWorkers,
            string srcDir,
            string dstDir,
            Action<string> updateStatus = null)
        {
            InitialiseWork(srcDir, dstDir);

            Workers.Clear();

            for (int i = 0; i < 2; i++)
            {
                CancellationTokenSource ctk = new CancellationTokenSource();
                Tuple<Task, CancellationTokenSource> tp = Tuple.Create(
                    Task.Run(() => Work(
                            ctk,
                            updateStatus),
                        ctk.Token),
                    ctk);

                Workers.Add(tp);
            }

            Task.WaitAll(Workers.Select(x => x.Item1).ToArray());

            return FilesProcessed.Count;
        }

        public int RevertWork(
            Action<string> updateStatus = null)
        {
            int before = FilesProcessed.Count;
            CancellationTokenSource ctk = new CancellationTokenSource();
            RevertWorker = Tuple.Create(
                Task.Run(() => Revert(
                        ctk,
                        updateStatus),
                    ctk.Token),
                ctk);

            RevertWorker.Item1.Wait();

            return before - FilesProcessed.Count;
        }
    }
}

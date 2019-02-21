using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileOrganizer
{
    public enum AppState
    {
        Start,
        Pause,
        Stop,
        Revert,
        ProcessDone,
        RevertDone,
        None
    }

    public partial class Form1 : Form
    {
        private object sync = new object();
        private AppState _appState = AppState.None;
        public const string AppName = "File Organizer";
        private AppState State
        {
            get
            {
                lock (sync)
                {
                    return _appState;
                }
            }

            set
            {
                lock (sync)
                {
                    _appState = value;
                }
                UpdateUIState();
            }
        }
        public HashSet<string> FilesProcessed = new HashSet<string>();
        public HashSet<string> FoldersCreated = new HashSet<string>();
        public AutoResetEvent start = new AutoResetEvent(false);

        public Form1()
        {
            InitializeComponent();
            UpdateUIState();
            toolTip1.SetToolTip(buttonStart, "Start");
            toolTip1.SetToolTip(buttonPause, "Pause");
            toolTip1.SetToolTip(buttonStop, "Stop");
            toolTip1.SetToolTip(buttonRevert, "Revert");
        }

        private void BrowseSourceBtnClick(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select the source folder...";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxSrcFolder.Text = dialog.SelectedPath;
            }
            else
            {
                textBoxSrcFolder.Text = string.Empty;
            }
        }

        private void BrowseDestBtnClick(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select the destination folder...";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxDestFolder.Text = dialog.SelectedPath;
            }
            else
            {
                textBoxDestFolder.Text = string.Empty;
            }
        }

        private void StartBtnClick(object sender, EventArgs e)
        {
            try
            {
                AppState previous = State;
                State = AppState.Start;
                start.Set();

                if (previous == AppState.None ||
                    previous == AppState.Stop ||
                    previous == AppState.ProcessDone ||
                    previous == AppState.RevertDone)
                {
                    ValidateInput(textBoxSrcFolder.Text, textBoxDestFolder.Text);

                    Task.Run(() => Process(textBoxSrcFolder.Text, textBoxDestFolder.Text));
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.Message,
                    AppName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                FinalizeProcessMsg();
                State = AppState.ProcessDone;
            }
        }

        private void PauseBtnClick(object sender, EventArgs e)
        {
            State = AppState.Pause;
            start.Reset();
        }

        private void StopBtnClick(object sender, EventArgs e)
        {
            State = AppState.Stop;
            start.Set();
        }

        private void RevertBtnClick(object sender, EventArgs e)
        {
            try
            {
                Task.Run(() => Revert());
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.Message,
                    AppName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UpdateUIState()
        {
            switch (State)
            {
                case AppState.None:
                    {
                        UpdateButton(buttonStart, true);
                        UpdateButton(buttonPause, false);
                        UpdateButton(buttonStop, false);
                        UpdateButton(buttonRevert, false);
                        UpdateButton(buttonBrowseSrc, true);
                        UpdateButton(buttonBrowseDest, true);
                        UpdateTextBox(textBoxSrcFolder, true);
                        UpdateTextBox(textBoxDestFolder, true);
                    }
                    break;
                case AppState.Start:
                    {
                        UpdateButton(buttonStart, false);
                        UpdateButton(buttonPause, true);
                        UpdateButton(buttonStop, true);
                        UpdateButton(buttonRevert, false);
                        UpdateButton(buttonBrowseSrc, false);
                        UpdateButton(buttonBrowseDest, false);
                        UpdateTextBox(textBoxSrcFolder, false);
                        UpdateTextBox(textBoxDestFolder, false);
                    }
                    break;
                case AppState.Pause:
                    {
                        UpdateButton(buttonStart, true);
                        UpdateButton(buttonPause, false);
                        UpdateButton(buttonStop, true);
                        UpdateButton(buttonRevert, false);
                        UpdateButton(buttonBrowseSrc, false);
                        UpdateButton(buttonBrowseDest, false);
                        UpdateTextBox(textBoxSrcFolder, false);
                        UpdateTextBox(textBoxDestFolder, false);
                    }
                    break;
                case AppState.Stop:
                    {
                        UpdateButton(buttonStart, true);
                        UpdateButton(buttonPause, false);
                        UpdateButton(buttonStop, false);
                        UpdateButton(buttonRevert, true);
                        UpdateButton(buttonBrowseSrc, true);
                        UpdateButton(buttonBrowseDest, true);
                        UpdateTextBox(textBoxSrcFolder, true);
                        UpdateTextBox(textBoxDestFolder, true);
                    }
                    break;
                case AppState.Revert:
                    {
                        UpdateButton(buttonStart, false);
                        UpdateButton(buttonPause, true);
                        UpdateButton(buttonStop, true);
                        UpdateButton(buttonRevert, false);
                        UpdateButton(buttonBrowseSrc, false);
                        UpdateButton(buttonBrowseDest, false);
                        UpdateTextBox(textBoxSrcFolder, false);
                        UpdateTextBox(textBoxDestFolder, false);
                    }
                    break;
                case AppState.ProcessDone:
                    {
                        UpdateButton(buttonStart, true);
                        UpdateButton(buttonPause, false);
                        UpdateButton(buttonStop, false);
                        UpdateButton(buttonRevert, true);
                        UpdateButton(buttonBrowseSrc, true);
                        UpdateButton(buttonBrowseDest, true);
                        UpdateTextBox(textBoxSrcFolder, true);
                        UpdateTextBox(textBoxDestFolder, true);
                    }
                    break;
                case AppState.RevertDone:
                    {
                        UpdateButton(buttonStart, true);
                        UpdateButton(buttonPause, false);
                        UpdateButton(buttonStop, false);
                        UpdateButton(buttonRevert, false);
                        UpdateButton(buttonBrowseSrc, true);
                        UpdateButton(buttonBrowseDest, true);
                        UpdateTextBox(textBoxSrcFolder, true);
                        UpdateTextBox(textBoxDestFolder, true);
                    }
                    break;
            }
        }

        public void ValidateInput(string input, string output)
        {
            if (!Directory.Exists(input))
            {
                throw new DirectoryNotFoundException($"Invalid source directory '{input}'!");
            }

            if (!Directory.Exists(output))
            {
                throw new DirectoryNotFoundException($"Invalid destination directory '{output}'!");
            }
        }

        private void UpdateListBox(string data)
        {
            listBoxStatus.Invoke((MethodInvoker)delegate {
                listBoxStatus.Items.Add(data);
            });
        }

        private void UpdateButton(Button b, bool enable)
        {
            if (b.InvokeRequired)
            {
                b.Invoke((MethodInvoker) delegate { b.Enabled = enable; });
            }
            else
            {
                b.Enabled = enable;    
            }
        }

        private void UpdateTextBox(TextBox t, bool enable)
        {
            if (t.InvokeRequired)
            {
                t.Invoke((MethodInvoker)delegate { t.Enabled = enable; });
            }
            else
            {
                t.Enabled = enable;
            }
        }

        private void UpdateStatus(Label l, string data)
        {
            if (l.InvokeRequired)
            {
                l.Invoke((MethodInvoker)delegate { l.Text = data; });
            }
            else
            {
                l.Text = data;
            }
        }

        private void ClearProcessingContent()
        {
            FilesProcessed.Clear();
            FoldersCreated.Clear();
        }

        private void ClearListBox()
        {
            listBoxStatus.Invoke((MethodInvoker)delegate {
                listBoxStatus.Items.Clear();
            });
        }

        private void Process(string input, string output)
        {
            ClearProcessingContent();
            ClearListBox();
            InitializeProcessMsg();

            input = input.TrimEnd('\\');
            output = output.TrimEnd('\\');

            DirectoryInfo info = new DirectoryInfo(input);
            FileInfo[] newFiles = info.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (var newFile in newFiles)
            {
                if (State != AppState.Start)
                {
                    if (State == AppState.Pause)
                    {
                        start.WaitOne();

                        if (State == AppState.Stop)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (State == AppState.Stop)
                        {
                            break;
                        }
                    }
                }

                UpdateListBox($"Processing: {newFile.FullName}");

                // create the directory if not exist
                string targetDir = FileOrganizerUtility.PrepareTargetDirFromFile(output, newFile);
                if (!Directory.Exists(targetDir))
                {
                    DirectoryInfo inf = new DirectoryInfo(targetDir);
                    while (!inf.Parent.Exists)
                    {
                        inf = inf.Parent;
                    }
                    UpdateListBox($"Creating directory: {targetDir}");
                    Directory.CreateDirectory(targetDir);
                    FoldersCreated.Add(inf.FullName);
                }

                string targetFilePath = FileOrganizerUtility.GetTargetFileName(newFile.FullName, targetDir);
                if (File.Exists(targetFilePath))
                {
                    targetFilePath = FileOrganizerUtility.AddGuidToFileNameConflict(targetFilePath);
                }

                if (newFile.CopyTo(targetFilePath).Exists != true)
                {
                    throw new IOException($"Failed to copy file from '{newFile.FullName}' to '{targetFilePath}'");
                }

                FilesProcessed.Add(targetFilePath);
                UpdateStatus(labelStatus, $"{FilesProcessed.Count} out of {newFiles.Length} processed...");
            }

            FinalizeProcessMsg();
            State = AppState.ProcessDone;
        }

        private void FinalizeProcessMsg()
        {
            UpdateListBox($"---------------------------------------------------------------------------------------------------");
            UpdateListBox($"Processing complete. Total files processed: {FilesProcessed.Count}");
            UpdateListBox($"---------------------------------------------------------------------------------------------------");
        }

        private void InitializeProcessMsg()
        {
            UpdateListBox($"---------------------------------------------------------------------------------------------------");
            UpdateListBox($"Processing starting...");
            UpdateListBox($"---------------------------------------------------------------------------------------------------");
        }

        private void Revert()
        {
            State = AppState.Revert;

            ClearListBox();

            InitializeRevertMsg();

            uint count = 0;
            foreach (var file in FilesProcessed)
            {
                if (State != AppState.Revert)
                {
                    if (State == AppState.Pause)
                    {
                        start.WaitOne();
                        if (State == AppState.Stop)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (State == AppState.Stop)
                        {
                            break;
                        }
                    }
                }
                UpdateListBox($"Deleting file: {file}");
                File.Delete(file);
                UpdateStatus(labelStatus, $"{++count} out of {FilesProcessed.Count} processed...");
            }

            foreach (var folder in FoldersCreated)
            {
                if (State != AppState.Revert)
                {
                    if (State == AppState.Pause)
                    {
                        start.WaitOne();
                        if (State == AppState.Stop)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (State == AppState.Stop)
                        {
                            break;
                        }
                    }
                }

                if (Directory.Exists(folder))
                {
                    UpdateListBox($"Deleting folder: {folder}");
                    Directory.Delete(folder, true);
                }
            }

            FinalizeRevertMsg();
            ClearProcessingContent();
            State = AppState.RevertDone;
        }

        private void FinalizeRevertMsg()
        {
            UpdateListBox($"---------------------------------------------------------------------------------------------------");
            UpdateListBox($"Revert complete. Total files deleted: {FilesProcessed.Count}");
            UpdateListBox($"---------------------------------------------------------------------------------------------------");
        }

        private void InitializeRevertMsg()
        {
            UpdateListBox($"---------------------------------------------------------------------------------------------------");
            UpdateListBox($"Revert process starting...");
            UpdateListBox($"---------------------------------------------------------------------------------------------------");
        }
    }
}

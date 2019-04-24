using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileOrganizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileOrganizerTests
{
    [TestClass]
    public class FileOrganizerTests
    {
        [TestMethod]
        [DeploymentItem("TestData", nameof(FileOrganizerProcessTest))]
        public void FileOrganizerProcessTest()
        {
            string inFolder = $"{nameof(FileOrganizerProcessTest)}\\in";
            string outFolder = $"{nameof(FileOrganizerProcessTest)}\\out";

            System.Collections.Generic.IEnumerable<string> files = Directory.EnumerateFiles(inFolder);
            int originalCount = files.Count();

            //// process test
            TaskManager mgr = new TaskManager();
            int count = mgr.StartWork(2, inFolder, outFolder, s => { });
            Assert.AreEqual(originalCount, count);
            Assert.IsTrue(Directory.Exists($"{outFolder}\\{DateTime.Today.Year.ToString()}\\April"));
            files = Directory.EnumerateFiles($"{outFolder}\\{DateTime.Today.Year.ToString()}\\April");
            Assert.IsTrue(files.Count(x => x.Contains("duplicate")) == 6);

            //// ensure original is intact
            files = Directory.EnumerateFiles(inFolder);
            Assert.IsTrue(files.Count() == originalCount);
        }

        [TestMethod]
        [DeploymentItem("TestData", nameof(FileOrganizerRevertTest))]
        public void FileOrganizerRevertTest()
        {
            string inFolder = $"{nameof(FileOrganizerRevertTest)}\\in";
            string outFolder = $"{nameof(FileOrganizerRevertTest)}\\out";

            System.Collections.Generic.IEnumerable<string> files = Directory.EnumerateFiles(inFolder);
            int originalCount = files.Count();

            //// process test
            TaskManager mgr = new TaskManager();
            int count = mgr.StartWork(2, inFolder, outFolder, s => { });
            Assert.AreEqual(originalCount, count);
            Assert.IsTrue(Directory.Exists($"{outFolder}\\{DateTime.Today.Year.ToString()}\\April"));
            files = Directory.EnumerateFiles($"{outFolder}\\{DateTime.Today.Year.ToString()}\\April");
            Assert.IsTrue(files.Count(x => x.Contains("duplicate")) == 6);

            //// revert test
            count = mgr.RevertWork(s => { });
            Assert.AreEqual(originalCount, count);
            Assert.IsTrue(!Directory.Exists($"{outFolder}\\{DateTime.Today.Year.ToString()}"));

            //// ensure original is intact
            files = Directory.EnumerateFiles(inFolder);
            Assert.IsTrue(files.Count() == originalCount);
        }

        [TestMethod]
        [DeploymentItem("TestData", nameof(FileOrganizerPauseTest))]
        public void FileOrganizerPauseTest()
        {
            string inFolder = $"{nameof(FileOrganizerPauseTest)}\\in";
            string outFolder = $"{nameof(FileOrganizerPauseTest)}\\out";

            System.Collections.Generic.IEnumerable<string> files = Directory.EnumerateFiles(inFolder);
            int originalCount = files.Count();

            //// process test
            TaskManager mgr = new TaskManager();
            int count = 0;
            var processTask = Task.Run(() =>
            {
                count = mgr.StartWork(2, inFolder, outFolder, s => { });
            });

            mgr.PauseWork();
            Assert.AreNotEqual(originalCount, count);
            mgr.ResumeWork();
            processTask.Wait();
            Assert.AreEqual(originalCount, count);
            Assert.IsTrue(Directory.Exists($"{outFolder}\\{DateTime.Today.Year.ToString()}\\April"));
            files = Directory.EnumerateFiles($"{outFolder}\\{DateTime.Today.Year.ToString()}\\April");
            Assert.IsTrue(files.Count(x => x.Contains("duplicate")) == 6);

            //// ensure original is intact
            files = Directory.EnumerateFiles(inFolder);
            Assert.IsTrue(files.Count() == originalCount);
        }

        [TestMethod]
        [DeploymentItem("TestData")]
        [DeploymentItem("TestData", nameof(FileOrganizerStopTest))]
        public void FileOrganizerStopTest()
        {
            string inFolder = $"{nameof(FileOrganizerStopTest)}\\in";
            string outFolder = $"{nameof(FileOrganizerStopTest)}\\out";

            System.Collections.Generic.IEnumerable<string> files = Directory.EnumerateFiles(inFolder);
            int originalCount = files.Count();

            //// process test
            TaskManager mgr = new TaskManager();
            int count = 0;
            var processTask = Task.Run(() =>
            {
                count = mgr.StartWork(2, inFolder, outFolder, s => { });
            });

            Thread.Sleep(10);
            mgr.StopWork();
            Assert.AreNotEqual(originalCount, count);

            processTask.Wait();
            Assert.IsTrue(Directory.Exists($"{outFolder}\\{DateTime.Today.Year.ToString()}\\April"));

            //// revert test
            count = mgr.RevertWork(s => { });
            Assert.IsTrue(count > 0);
            Assert.IsTrue(!Directory.Exists($"{outFolder}\\{DateTime.Today.Year.ToString()}"));

            //// ensure original is intact
            files = Directory.EnumerateFiles(inFolder);
            Assert.IsTrue(files.Count() == originalCount);
        }
    }
}

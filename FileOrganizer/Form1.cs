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
        private TaskManager _taskManager = null;
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

                if (previous == AppState.None ||
                    previous == AppState.Stop ||
                    previous == AppState.ProcessDone ||
                    previous == AppState.RevertDone)
                {
                    //textBoxSrcFolder.Text = @"C:\temp\FileManager\in";
                    //textBoxDestFolder.Text = @"C:\temp\FileManager\out";
                    ValidateInput(textBoxSrcFolder.Text, textBoxDestFolder.Text);
                    Task.Run(() => Process(textBoxSrcFolder.Text, textBoxDestFolder.Text));
                }
                else if (previous == AppState.Pause)
                {
                    UpdateStatus(labelStatus, $"Processing in progress...");
                    _taskManager.ResumeWork();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.Message,
                    AppName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                State = AppState.ProcessDone;
            }
        }

        private void PauseBtnClick(object sender, EventArgs e)
        {
            State = AppState.Pause;
            _taskManager.PauseWork();
            UpdateStatus(labelStatus, $"Paused");
        }

        private void StopBtnClick(object sender, EventArgs e)
        {
            State = AppState.Stop;
            _taskManager.StopWork();
            UpdateStatus(labelStatus, $"Stopped");
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

        private void ClearListBox()
        {
            listBoxStatus.Invoke((MethodInvoker)delegate {
                listBoxStatus.Items.Clear();
            });
        }

        private void Process(string input, string output)
        {
            ClearListBox();

            UpdateStatus($"---------------------------------------------------------------------------------------------------");
            UpdateStatus($"Processing starting...");
            UpdateStatus($"---------------------------------------------------------------------------------------------------");
            UpdateStatus(labelStatus, $"Processing in progress...");

            input = input.TrimEnd('\\');
            output = output.TrimEnd('\\');
            _taskManager = new TaskManager();
            int processed = _taskManager.StartWork(
                10,
                input,
                output,
                s => UpdateStatus(s));

            UpdateStatus($"---------------------------------------------------------------------------------------------------");
            UpdateStatus($"Processing complete. Total files processed: {processed}");
            UpdateStatus($"---------------------------------------------------------------------------------------------------");
            UpdateStatus(labelStatus, $"Processing complete. Total files processed: {processed}");
            State = AppState.ProcessDone;
        }

        private void Revert()
        {
            State = AppState.Revert;

            ClearListBox();

            UpdateStatus($"---------------------------------------------------------------------------------------------------");
            UpdateStatus($"Revert process starting...");
            UpdateStatus($"---------------------------------------------------------------------------------------------------");
            UpdateStatus(labelStatus, $"Revert in progress...");
            int reverted = _taskManager.RevertWork(
                s => UpdateStatus(s));

            UpdateStatus($"---------------------------------------------------------------------------------------------------");
            UpdateStatus($"Revert complete. Total files deleted: {reverted}");
            UpdateStatus($"---------------------------------------------------------------------------------------------------");
            UpdateStatus(labelStatus, $"Revert complete. Total files deleted: {reverted}");
            State = AppState.RevertDone;
        }

        private void UpdateStatus(string data)
        {
            listBoxStatus.Invoke((MethodInvoker)delegate {
                listBoxStatus.Items.Add(data);
            });
        }
    }
}

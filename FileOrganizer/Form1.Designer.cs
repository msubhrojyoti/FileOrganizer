namespace FileOrganizer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSrcFolder = new System.Windows.Forms.TextBox();
            this.buttonBrowseSrc = new System.Windows.Forms.Button();
            this.buttonBrowseDest = new System.Windows.Forms.Button();
            this.textBoxDestFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonRevert = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.142858F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source folder:";
            // 
            // textBoxSrcFolder
            // 
            this.textBoxSrcFolder.Location = new System.Drawing.Point(161, 26);
            this.textBoxSrcFolder.Name = "textBoxSrcFolder";
            this.textBoxSrcFolder.Size = new System.Drawing.Size(455, 20);
            this.textBoxSrcFolder.TabIndex = 1;
            // 
            // buttonBrowseSrc
            // 
            this.buttonBrowseSrc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.142858F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowseSrc.Location = new System.Drawing.Point(658, 26);
            this.buttonBrowseSrc.Name = "buttonBrowseSrc";
            this.buttonBrowseSrc.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseSrc.TabIndex = 2;
            this.buttonBrowseSrc.Text = "Browse";
            this.buttonBrowseSrc.UseVisualStyleBackColor = true;
            this.buttonBrowseSrc.Click += new System.EventHandler(this.BrowseSourceBtnClick);
            // 
            // buttonBrowseDest
            // 
            this.buttonBrowseDest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.142858F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowseDest.Location = new System.Drawing.Point(658, 64);
            this.buttonBrowseDest.Name = "buttonBrowseDest";
            this.buttonBrowseDest.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseDest.TabIndex = 5;
            this.buttonBrowseDest.Text = "Browse";
            this.buttonBrowseDest.UseVisualStyleBackColor = true;
            this.buttonBrowseDest.Click += new System.EventHandler(this.BrowseDestBtnClick);
            // 
            // textBoxDestFolder
            // 
            this.textBoxDestFolder.Location = new System.Drawing.Point(161, 64);
            this.textBoxDestFolder.Name = "textBoxDestFolder";
            this.textBoxDestFolder.Size = new System.Drawing.Size(455, 20);
            this.textBoxDestFolder.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.142858F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(24, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Destination folder:";
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.FormattingEnabled = true;
            this.listBoxStatus.HorizontalScrollbar = true;
            this.listBoxStatus.Location = new System.Drawing.Point(27, 140);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.ScrollAlwaysVisible = true;
            this.listBoxStatus.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBoxStatus.Size = new System.Drawing.Size(706, 290);
            this.listBoxStatus.TabIndex = 6;
            // 
            // buttonRevert
            // 
            this.buttonRevert.BackgroundImage = global::FileOrganizer.Properties.Resources.revert;
            this.buttonRevert.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonRevert.Location = new System.Drawing.Point(171, 92);
            this.buttonRevert.Name = "buttonRevert";
            this.buttonRevert.Size = new System.Drawing.Size(42, 42);
            this.buttonRevert.TabIndex = 10;
            this.buttonRevert.UseVisualStyleBackColor = true;
            this.buttonRevert.Click += new System.EventHandler(this.RevertBtnClick);
            // 
            // buttonStop
            // 
            this.buttonStop.BackgroundImage = global::FileOrganizer.Properties.Resources.stop;
            this.buttonStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonStop.Location = new System.Drawing.Point(122, 92);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(42, 42);
            this.buttonStop.TabIndex = 9;
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.StopBtnClick);
            // 
            // buttonPause
            // 
            this.buttonPause.BackgroundImage = global::FileOrganizer.Properties.Resources.pause;
            this.buttonPause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonPause.Location = new System.Drawing.Point(75, 92);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(42, 42);
            this.buttonPause.TabIndex = 8;
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.PauseBtnClick);
            // 
            // buttonStart
            // 
            this.buttonStart.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStart.BackgroundImage = global::FileOrganizer.Properties.Resources.start;
            this.buttonStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonStart.Location = new System.Drawing.Point(27, 91);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(42, 42);
            this.buttonStart.TabIndex = 7;
            this.buttonStart.UseVisualStyleBackColor = false;
            this.buttonStart.Click += new System.EventHandler(this.StartBtnClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.142858F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(27, 440);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Status:";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.142858F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(67, 440);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 13);
            this.labelStatus.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(753, 468);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonRevert);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.listBoxStatus);
            this.Controls.Add(this.buttonBrowseDest);
            this.Controls.Add(this.textBoxDestFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonBrowseSrc);
            this.Controls.Add(this.textBoxSrcFolder);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "File Organizer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSrcFolder;
        private System.Windows.Forms.Button buttonBrowseSrc;
        private System.Windows.Forms.Button buttonBrowseDest;
        private System.Windows.Forms.TextBox textBoxDestFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonRevert;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelStatus;
    }
}


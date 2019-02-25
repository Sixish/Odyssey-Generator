namespace TibiaCastRecorderApplication
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
            this.btnStart = new System.Windows.Forms.Button();
            this.lstIgnore = new System.Windows.Forms.ListBox();
            this.lstRead = new System.Windows.Forms.ListBox();
            this.btnAddToRead = new System.Windows.Forms.Button();
            this.btnAddToIgnore = new System.Windows.Forms.Button();
            this.chkSelectAllIgnore = new System.Windows.Forms.CheckBox();
            this.chkSelectAllRead = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pbCompletion = new System.Windows.Forms.ProgressBar();
            this.tbFolderPath = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lstStatus = new System.Windows.Forms.ListBox();
            this.dlgSelectFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.serviceController1 = new System.ServiceProcess.ServiceController();
            this.serviceController2 = new System.ServiceProcess.ServiceController();
            this.serviceController3 = new System.ServiceProcess.ServiceController();
            this.serviceController4 = new System.ServiceProcess.ServiceController();
            this.serviceController5 = new System.ServiceProcess.ServiceController();
            this.button1 = new System.Windows.Forms.Button();
            this.tbDestinationFolder = new System.Windows.Forms.TextBox();
            this.lblDestination = new System.Windows.Forms.Label();
            this.chkSaveOnClose = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 219);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(259, 42);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lstIgnore
            // 
            this.lstIgnore.FormattingEnabled = true;
            this.lstIgnore.Items.AddRange(new object[] {
            "240552",
            "240559",
            "254910",
            "259315"});
            this.lstIgnore.Location = new System.Drawing.Point(13, 82);
            this.lstIgnore.Name = "lstIgnore";
            this.lstIgnore.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstIgnore.Size = new System.Drawing.Size(89, 108);
            this.lstIgnore.TabIndex = 2;
            // 
            // lstRead
            // 
            this.lstRead.FormattingEnabled = true;
            this.lstRead.Items.AddRange(new object[] {
            "240555",
            "263401"});
            this.lstRead.Location = new System.Drawing.Point(182, 82);
            this.lstRead.Name = "lstRead";
            this.lstRead.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstRead.Size = new System.Drawing.Size(89, 108);
            this.lstRead.TabIndex = 3;
            // 
            // btnAddToRead
            // 
            this.btnAddToRead.Location = new System.Drawing.Point(108, 70);
            this.btnAddToRead.Name = "btnAddToRead";
            this.btnAddToRead.Size = new System.Drawing.Size(69, 37);
            this.btnAddToRead.TabIndex = 4;
            this.btnAddToRead.Text = ">>";
            this.btnAddToRead.UseVisualStyleBackColor = true;
            this.btnAddToRead.Click += new System.EventHandler(this.btnAddToRead_Click);
            // 
            // btnAddToIgnore
            // 
            this.btnAddToIgnore.Location = new System.Drawing.Point(108, 176);
            this.btnAddToIgnore.Name = "btnAddToIgnore";
            this.btnAddToIgnore.Size = new System.Drawing.Size(69, 37);
            this.btnAddToIgnore.TabIndex = 5;
            this.btnAddToIgnore.Text = "<<";
            this.btnAddToIgnore.UseVisualStyleBackColor = true;
            this.btnAddToIgnore.Click += new System.EventHandler(this.btnAddToIgnore_Click);
            // 
            // chkSelectAllIgnore
            // 
            this.chkSelectAllIgnore.AutoSize = true;
            this.chkSelectAllIgnore.Location = new System.Drawing.Point(12, 196);
            this.chkSelectAllIgnore.Name = "chkSelectAllIgnore";
            this.chkSelectAllIgnore.Size = new System.Drawing.Size(70, 17);
            this.chkSelectAllIgnore.TabIndex = 6;
            this.chkSelectAllIgnore.Text = "Select All";
            this.chkSelectAllIgnore.UseVisualStyleBackColor = true;
            this.chkSelectAllIgnore.CheckedChanged += new System.EventHandler(this.chkSelectAllIgnore_CheckedChanged);
            // 
            // chkSelectAllRead
            // 
            this.chkSelectAllRead.AutoSize = true;
            this.chkSelectAllRead.Location = new System.Drawing.Point(183, 196);
            this.chkSelectAllRead.Name = "chkSelectAllRead";
            this.chkSelectAllRead.Size = new System.Drawing.Size(70, 17);
            this.chkSelectAllRead.TabIndex = 7;
            this.chkSelectAllRead.Text = "Select All";
            this.chkSelectAllRead.UseVisualStyleBackColor = true;
            this.chkSelectAllRead.CheckedChanged += new System.EventHandler(this.chkSelectAllRead_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Ignore";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(183, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Read";
            // 
            // pbCompletion
            // 
            this.pbCompletion.Location = new System.Drawing.Point(12, 267);
            this.pbCompletion.Name = "pbCompletion";
            this.pbCompletion.Size = new System.Drawing.Size(259, 10);
            this.pbCompletion.TabIndex = 10;
            // 
            // tbFolderPath
            // 
            this.tbFolderPath.Location = new System.Drawing.Point(12, 24);
            this.tbFolderPath.Name = "tbFolderPath";
            this.tbFolderPath.Size = new System.Drawing.Size(165, 20);
            this.tbFolderPath.TabIndex = 11;
            this.tbFolderPath.TextChanged += new System.EventHandler(this.tbFolderPath_TextChanged);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(186, 24);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(86, 20);
            this.btnOpen.TabIndex = 12;
            this.btnOpen.Text = "Open...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Add Recordings in Directory";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 280);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Status: ";
            // 
            // lstStatus
            // 
            this.lstStatus.FormattingEnabled = true;
            this.lstStatus.Location = new System.Drawing.Point(19, 296);
            this.lstStatus.Name = "lstStatus";
            this.lstStatus.Size = new System.Drawing.Size(253, 121);
            this.lstStatus.TabIndex = 15;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(467, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 20);
            this.button1.TabIndex = 16;
            this.button1.Text = "Search...";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tbDestinationFolder
            // 
            this.tbDestinationFolder.Location = new System.Drawing.Point(296, 24);
            this.tbDestinationFolder.Name = "tbDestinationFolder";
            this.tbDestinationFolder.Size = new System.Drawing.Size(165, 20);
            this.tbDestinationFolder.TabIndex = 17;
            // 
            // lblDestination
            // 
            this.lblDestination.AutoSize = true;
            this.lblDestination.Location = new System.Drawing.Point(296, 5);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(84, 13);
            this.lblDestination.TabIndex = 18;
            this.lblDestination.Text = "Output Directory";
            // 
            // chkSaveOnClose
            // 
            this.chkSaveOnClose.AutoSize = true;
            this.chkSaveOnClose.Location = new System.Drawing.Point(299, 82);
            this.chkSaveOnClose.Name = "chkSaveOnClose";
            this.chkSaveOnClose.Size = new System.Drawing.Size(94, 17);
            this.chkSaveOnClose.TabIndex = 19;
            this.chkSaveOnClose.Text = "Save on close";
            this.chkSaveOnClose.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 431);
            this.Controls.Add(this.chkSaveOnClose);
            this.Controls.Add(this.lblDestination);
            this.Controls.Add(this.tbDestinationFolder);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lstStatus);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.tbFolderPath);
            this.Controls.Add(this.pbCompletion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkSelectAllRead);
            this.Controls.Add(this.chkSelectAllIgnore);
            this.Controls.Add(this.btnAddToIgnore);
            this.Controls.Add(this.btnAddToRead);
            this.Controls.Add(this.lstRead);
            this.Controls.Add(this.lstIgnore);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "Tibiacast Recording Reader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ListBox lstIgnore;
        private System.Windows.Forms.ListBox lstRead;
        private System.Windows.Forms.Button btnAddToRead;
        private System.Windows.Forms.Button btnAddToIgnore;
        private System.Windows.Forms.CheckBox chkSelectAllIgnore;
        private System.Windows.Forms.CheckBox chkSelectAllRead;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar pbCompletion;
        private System.Windows.Forms.TextBox tbFolderPath;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lstStatus;
        private System.Windows.Forms.FolderBrowserDialog dlgSelectFolder;
        private System.ServiceProcess.ServiceController serviceController1;
        private System.ServiceProcess.ServiceController serviceController2;
        private System.ServiceProcess.ServiceController serviceController3;
        private System.ServiceProcess.ServiceController serviceController4;
        private System.ServiceProcess.ServiceController serviceController5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbDestinationFolder;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.CheckBox chkSaveOnClose;
    }
}


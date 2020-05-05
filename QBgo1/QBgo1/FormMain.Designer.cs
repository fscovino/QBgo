namespace QBgo1
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.lblQbFile = new System.Windows.Forms.Label();
            this.gbxFiles = new System.Windows.Forms.GroupBox();
            this.btnInvFolder = new System.Windows.Forms.Button();
            this.txbInvFolder = new System.Windows.Forms.TextBox();
            this.lblInvFolder = new System.Windows.Forms.Label();
            this.btnOrdersFolder = new System.Windows.Forms.Button();
            this.txbOrdersFolder = new System.Windows.Forms.TextBox();
            this.lblOrdersFolder = new System.Windows.Forms.Label();
            this.btnQbFile = new System.Windows.Forms.Button();
            this.txbQbFile = new System.Windows.Forms.TextBox();
            this.cboRefreshRate = new System.Windows.Forms.ComboBox();
            this.lblRefreshRate = new System.Windows.Forms.Label();
            this.lblMinutes = new System.Windows.Forms.Label();
            this.btnInventory = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.fbdFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.ofdQbFile = new System.Windows.Forms.OpenFileDialog();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.gbxFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblQbFile
            // 
            this.lblQbFile.AutoSize = true;
            this.lblQbFile.Location = new System.Drawing.Point(6, 41);
            this.lblQbFile.Name = "lblQbFile";
            this.lblQbFile.Size = new System.Drawing.Size(86, 13);
            this.lblQbFile.TabIndex = 0;
            this.lblQbFile.Text = "Quickbooks File:";
            this.lblQbFile.DoubleClick += new System.EventHandler(this.lblQbFile_DoubleClick);
            // 
            // gbxFiles
            // 
            this.gbxFiles.Controls.Add(this.btnInvFolder);
            this.gbxFiles.Controls.Add(this.txbInvFolder);
            this.gbxFiles.Controls.Add(this.lblInvFolder);
            this.gbxFiles.Controls.Add(this.btnOrdersFolder);
            this.gbxFiles.Controls.Add(this.txbOrdersFolder);
            this.gbxFiles.Controls.Add(this.lblOrdersFolder);
            this.gbxFiles.Controls.Add(this.btnQbFile);
            this.gbxFiles.Controls.Add(this.txbQbFile);
            this.gbxFiles.Controls.Add(this.lblQbFile);
            this.gbxFiles.Location = new System.Drawing.Point(12, 12);
            this.gbxFiles.Name = "gbxFiles";
            this.gbxFiles.Size = new System.Drawing.Size(468, 147);
            this.gbxFiles.TabIndex = 1;
            this.gbxFiles.TabStop = false;
            this.gbxFiles.Text = "FIles and Folders";
            // 
            // btnInvFolder
            // 
            this.btnInvFolder.Location = new System.Drawing.Point(387, 102);
            this.btnInvFolder.Name = "btnInvFolder";
            this.btnInvFolder.Size = new System.Drawing.Size(75, 23);
            this.btnInvFolder.TabIndex = 8;
            this.btnInvFolder.Text = "Browse";
            this.btnInvFolder.UseVisualStyleBackColor = true;
            this.btnInvFolder.Click += new System.EventHandler(this.btnInvFolder_Click);
            // 
            // txbInvFolder
            // 
            this.txbInvFolder.Location = new System.Drawing.Point(102, 105);
            this.txbInvFolder.Name = "txbInvFolder";
            this.txbInvFolder.Size = new System.Drawing.Size(279, 20);
            this.txbInvFolder.TabIndex = 7;
            // 
            // lblInvFolder
            // 
            this.lblInvFolder.AutoSize = true;
            this.lblInvFolder.Location = new System.Drawing.Point(6, 109);
            this.lblInvFolder.Name = "lblInvFolder";
            this.lblInvFolder.Size = new System.Drawing.Size(86, 13);
            this.lblInvFolder.TabIndex = 6;
            this.lblInvFolder.Text = "Inventory Folder:";
            // 
            // btnOrdersFolder
            // 
            this.btnOrdersFolder.Location = new System.Drawing.Point(387, 71);
            this.btnOrdersFolder.Name = "btnOrdersFolder";
            this.btnOrdersFolder.Size = new System.Drawing.Size(75, 23);
            this.btnOrdersFolder.TabIndex = 5;
            this.btnOrdersFolder.Text = "Browse";
            this.btnOrdersFolder.UseVisualStyleBackColor = true;
            this.btnOrdersFolder.Click += new System.EventHandler(this.btnOrdersFolder_Click);
            // 
            // txbOrdersFolder
            // 
            this.txbOrdersFolder.Location = new System.Drawing.Point(102, 71);
            this.txbOrdersFolder.Name = "txbOrdersFolder";
            this.txbOrdersFolder.Size = new System.Drawing.Size(279, 20);
            this.txbOrdersFolder.TabIndex = 4;
            // 
            // lblOrdersFolder
            // 
            this.lblOrdersFolder.AutoSize = true;
            this.lblOrdersFolder.Location = new System.Drawing.Point(6, 75);
            this.lblOrdersFolder.Name = "lblOrdersFolder";
            this.lblOrdersFolder.Size = new System.Drawing.Size(73, 13);
            this.lblOrdersFolder.TabIndex = 3;
            this.lblOrdersFolder.Text = "Orders Folder:";
            // 
            // btnQbFile
            // 
            this.btnQbFile.Location = new System.Drawing.Point(386, 37);
            this.btnQbFile.Name = "btnQbFile";
            this.btnQbFile.Size = new System.Drawing.Size(75, 23);
            this.btnQbFile.TabIndex = 2;
            this.btnQbFile.Text = "Browse";
            this.btnQbFile.UseVisualStyleBackColor = true;
            this.btnQbFile.Click += new System.EventHandler(this.btnQbFile_Click);
            // 
            // txbQbFile
            // 
            this.txbQbFile.Location = new System.Drawing.Point(102, 37);
            this.txbQbFile.Name = "txbQbFile";
            this.txbQbFile.Size = new System.Drawing.Size(279, 20);
            this.txbQbFile.TabIndex = 1;
            // 
            // cboRefreshRate
            // 
            this.cboRefreshRate.FormattingEnabled = true;
            this.cboRefreshRate.Items.AddRange(new object[] {
            "3",
            "5",
            "10",
            "15",
            "30",
            "60",
            "90",
            "120"});
            this.cboRefreshRate.Location = new System.Drawing.Point(114, 165);
            this.cboRefreshRate.Name = "cboRefreshRate";
            this.cboRefreshRate.Size = new System.Drawing.Size(58, 21);
            this.cboRefreshRate.TabIndex = 2;
            this.cboRefreshRate.SelectedIndexChanged += new System.EventHandler(this.cboRefreshRate_SelectedIndexChanged);
            // 
            // lblRefreshRate
            // 
            this.lblRefreshRate.AutoSize = true;
            this.lblRefreshRate.Location = new System.Drawing.Point(21, 169);
            this.lblRefreshRate.Name = "lblRefreshRate";
            this.lblRefreshRate.Size = new System.Drawing.Size(77, 13);
            this.lblRefreshRate.TabIndex = 3;
            this.lblRefreshRate.Text = "Refresh Every:";
            // 
            // lblMinutes
            // 
            this.lblMinutes.AutoSize = true;
            this.lblMinutes.Location = new System.Drawing.Point(179, 169);
            this.lblMinutes.Name = "lblMinutes";
            this.lblMinutes.Size = new System.Drawing.Size(44, 13);
            this.lblMinutes.TabIndex = 4;
            this.lblMinutes.Text = "Minutes";
            // 
            // btnInventory
            // 
            this.btnInventory.Location = new System.Drawing.Point(399, 162);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(75, 23);
            this.btnInventory.TabIndex = 5;
            this.btnInventory.Text = "Inventory";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(255, 192);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(225, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 192);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(225, 23);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // ofdQbFile
            // 
            this.ofdQbFile.FileName = "ofdQbFile";
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "QBgo - ";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 221);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnInventory);
            this.Controls.Add(this.lblMinutes);
            this.Controls.Add(this.lblRefreshRate);
            this.Controls.Add(this.cboRefreshRate);
            this.Controls.Add(this.gbxFiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 250);
            this.MinimumSize = new System.Drawing.Size(500, 250);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QBgo";
            this.gbxFiles.ResumeLayout(false);
            this.gbxFiles.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblQbFile;
        private System.Windows.Forms.GroupBox gbxFiles;
        private System.Windows.Forms.Button btnQbFile;
        private System.Windows.Forms.TextBox txbQbFile;
        private System.Windows.Forms.TextBox txbInvFolder;
        private System.Windows.Forms.Label lblInvFolder;
        private System.Windows.Forms.Button btnOrdersFolder;
        private System.Windows.Forms.TextBox txbOrdersFolder;
        private System.Windows.Forms.Label lblOrdersFolder;
        private System.Windows.Forms.Button btnInvFolder;
        private System.Windows.Forms.ComboBox cboRefreshRate;
        private System.Windows.Forms.Label lblRefreshRate;
        private System.Windows.Forms.Label lblMinutes;
        private System.Windows.Forms.Button btnInventory;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.FolderBrowserDialog fbdFolder;
        private System.Windows.Forms.OpenFileDialog ofdQbFile;
        private System.Windows.Forms.NotifyIcon notifyIcon;
    }
}


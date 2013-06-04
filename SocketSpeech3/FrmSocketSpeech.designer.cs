namespace SocketSpeech3
{
	partial class FrmSocketSpeech
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSocketSpeech));
			this.gbSpeech = new System.Windows.Forms.GroupBox();
			this.btnExplore = new System.Windows.Forms.Button();
			this.lblReadFile = new System.Windows.Forms.Label();
			this.lblVoice = new System.Windows.Forms.Label();
			this.txtFileToRead = new System.Windows.Forms.TextBox();
			this.cbVoices = new System.Windows.Forms.ComboBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnReadFile = new System.Windows.Forms.Button();
			this.btnSpeech = new System.Windows.Forms.Button();
			this.txtTextToSpeech = new System.Windows.Forms.TextBox();
			this.gbConsole = new System.Windows.Forms.GroupBox();
			this.pbSpeechProgress = new System.Windows.Forms.ProgressBar();
			this.chkAutoDisableSpRec = new System.Windows.Forms.CheckBox();
			this.txtConsole = new System.Windows.Forms.TextBox();
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.lblServerStarted = new System.Windows.Forms.ToolStripStatusLabel();
			this.lblClientConnected = new System.Windows.Forms.ToolStripStatusLabel();
			this.lblCurrentAddres = new System.Windows.Forms.ToolStripStatusLabel();
			this.lblCurrentInputPort = new System.Windows.Forms.ToolStripStatusLabel();
			this.lblCurrentOutputPort = new System.Windows.Forms.ToolStripStatusLabel();
			this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
			this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
			this.bgwSave = new System.ComponentModel.BackgroundWorker();
			this.gbSpeech.SuspendLayout();
			this.gbConsole.SuspendLayout();
			this.statusBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbSpeech
			// 
			this.gbSpeech.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbSpeech.Controls.Add(this.btnExplore);
			this.gbSpeech.Controls.Add(this.lblReadFile);
			this.gbSpeech.Controls.Add(this.lblVoice);
			this.gbSpeech.Controls.Add(this.txtFileToRead);
			this.gbSpeech.Controls.Add(this.cbVoices);
			this.gbSpeech.Controls.Add(this.btnSave);
			this.gbSpeech.Controls.Add(this.btnReadFile);
			this.gbSpeech.Controls.Add(this.btnSpeech);
			this.gbSpeech.Controls.Add(this.txtTextToSpeech);
			this.gbSpeech.Location = new System.Drawing.Point(13, 13);
			this.gbSpeech.Name = "gbSpeech";
			this.gbSpeech.Size = new System.Drawing.Size(554, 160);
			this.gbSpeech.TabIndex = 0;
			this.gbSpeech.TabStop = false;
			this.gbSpeech.Text = "Speech";
			// 
			// btnExplore
			// 
			this.btnExplore.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnExplore.Image = global::SocketSpeech3.Properties.Resources.SearchFolderHS;
			this.btnExplore.Location = new System.Drawing.Point(402, 131);
			this.btnExplore.Name = "btnExplore";
			this.btnExplore.Size = new System.Drawing.Size(23, 23);
			this.btnExplore.TabIndex = 5;
			this.btnExplore.UseVisualStyleBackColor = true;
			this.btnExplore.Click += new System.EventHandler(this.btnExplore_Click);
			// 
			// lblReadFile
			// 
			this.lblReadFile.AutoSize = true;
			this.lblReadFile.Location = new System.Drawing.Point(6, 136);
			this.lblReadFile.Name = "lblReadFile";
			this.lblReadFile.Size = new System.Drawing.Size(55, 13);
			this.lblReadFile.TabIndex = 4;
			this.lblReadFile.Text = "Read File:";
			// 
			// lblVoice
			// 
			this.lblVoice.AutoSize = true;
			this.lblVoice.Location = new System.Drawing.Point(7, 107);
			this.lblVoice.Name = "lblVoice";
			this.lblVoice.Size = new System.Drawing.Size(37, 13);
			this.lblVoice.TabIndex = 4;
			this.lblVoice.Text = "Voice:";
			// 
			// txtFileToRead
			// 
			this.txtFileToRead.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtFileToRead.Location = new System.Drawing.Point(68, 133);
			this.txtFileToRead.Name = "txtFileToRead";
			this.txtFileToRead.Size = new System.Drawing.Size(328, 20);
			this.txtFileToRead.TabIndex = 4;
			this.txtFileToRead.TextChanged += new System.EventHandler(this.txtFileToRead_TextChanged);
			// 
			// cbVoices
			// 
			this.cbVoices.FormattingEnabled = true;
			this.cbVoices.Location = new System.Drawing.Point(68, 104);
			this.cbVoices.Name = "cbVoices";
			this.cbVoices.Size = new System.Drawing.Size(247, 21);
			this.cbVoices.TabIndex = 1;
			this.cbVoices.SelectedIndexChanged += new System.EventHandler(this.cbVoices_SelectedIndexChanged);
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Enabled = false;
			this.btnSave.Location = new System.Drawing.Point(321, 102);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 2;
			this.btnSave.Text = "Save to...";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnReadFile
			// 
			this.btnReadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnReadFile.Enabled = false;
			this.btnReadFile.Location = new System.Drawing.Point(431, 131);
			this.btnReadFile.Name = "btnReadFile";
			this.btnReadFile.Size = new System.Drawing.Size(116, 23);
			this.btnReadFile.TabIndex = 6;
			this.btnReadFile.Text = "Read File";
			this.btnReadFile.UseVisualStyleBackColor = true;
			this.btnReadFile.Click += new System.EventHandler(this.btnReadFile_Click);
			// 
			// btnSpeech
			// 
			this.btnSpeech.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSpeech.Enabled = false;
			this.btnSpeech.Location = new System.Drawing.Point(402, 102);
			this.btnSpeech.Name = "btnSpeech";
			this.btnSpeech.Size = new System.Drawing.Size(145, 23);
			this.btnSpeech.TabIndex = 3;
			this.btnSpeech.Text = "Speech";
			this.btnSpeech.UseVisualStyleBackColor = true;
			this.btnSpeech.Click += new System.EventHandler(this.btnSpeech_Click);
			// 
			// txtTextToSpeech
			// 
			this.txtTextToSpeech.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtTextToSpeech.Location = new System.Drawing.Point(10, 20);
			this.txtTextToSpeech.Multiline = true;
			this.txtTextToSpeech.Name = "txtTextToSpeech";
			this.txtTextToSpeech.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtTextToSpeech.Size = new System.Drawing.Size(537, 76);
			this.txtTextToSpeech.TabIndex = 0;
			this.txtTextToSpeech.TextChanged += new System.EventHandler(this.txtTextToSpeech_TextChanged);
			// 
			// gbConsole
			// 
			this.gbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbConsole.Controls.Add(this.pbSpeechProgress);
			this.gbConsole.Controls.Add(this.chkAutoDisableSpRec);
			this.gbConsole.Controls.Add(this.txtConsole);
			this.gbConsole.Location = new System.Drawing.Point(13, 179);
			this.gbConsole.Name = "gbConsole";
			this.gbConsole.Size = new System.Drawing.Size(554, 145);
			this.gbConsole.TabIndex = 0;
			this.gbConsole.TabStop = false;
			this.gbConsole.Text = "Console";
			// 
			// pbSpeechProgress
			// 
			this.pbSpeechProgress.Location = new System.Drawing.Point(254, 122);
			this.pbSpeechProgress.Name = "pbSpeechProgress";
			this.pbSpeechProgress.Size = new System.Drawing.Size(294, 17);
			this.pbSpeechProgress.TabIndex = 22;
			this.pbSpeechProgress.Visible = false;
			// 
			// chkAutoDisableSpRec
			// 
			this.chkAutoDisableSpRec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkAutoDisableSpRec.AutoSize = true;
			this.chkAutoDisableSpRec.Location = new System.Drawing.Point(10, 122);
			this.chkAutoDisableSpRec.Name = "chkAutoDisableSpRec";
			this.chkAutoDisableSpRec.Size = new System.Drawing.Size(238, 17);
			this.chkAutoDisableSpRec.TabIndex = 8;
			this.chkAutoDisableSpRec.Text = "Automatically disable Sp-Rec when speaking";
			this.chkAutoDisableSpRec.UseVisualStyleBackColor = true;
			this.chkAutoDisableSpRec.CheckedChanged += new System.EventHandler(this.chkAutoDisableSpRec_CheckedChanged);
			// 
			// txtConsole
			// 
			this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtConsole.Location = new System.Drawing.Point(10, 20);
			this.txtConsole.Multiline = true;
			this.txtConsole.Name = "txtConsole";
			this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtConsole.Size = new System.Drawing.Size(537, 96);
			this.txtConsole.TabIndex = 7;
			this.txtConsole.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConsole_KeyDown);
			// 
			// statusBar
			// 
			this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblServerStarted,
            this.lblClientConnected,
            this.lblCurrentAddres,
            this.lblCurrentInputPort,
            this.lblCurrentOutputPort});
			this.statusBar.Location = new System.Drawing.Point(0, 327);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(579, 22);
			this.statusBar.TabIndex = 2;
			this.statusBar.Text = "statusStrip1";
			// 
			// lblServerStarted
			// 
			this.lblServerStarted.Name = "lblServerStarted";
			this.lblServerStarted.Size = new System.Drawing.Size(78, 17);
			this.lblServerStarted.Text = "Server Started";
			this.lblServerStarted.Visible = false;
			// 
			// lblClientConnected
			// 
			this.lblClientConnected.Name = "lblClientConnected";
			this.lblClientConnected.Size = new System.Drawing.Size(94, 17);
			this.lblClientConnected.Text = "Client Conncected";
			this.lblClientConnected.Visible = false;
			// 
			// lblCurrentAddres
			// 
			this.lblCurrentAddres.Name = "lblCurrentAddres";
			this.lblCurrentAddres.Size = new System.Drawing.Size(91, 17);
			this.lblCurrentAddres.Text = "255.255.255.255";
			this.lblCurrentAddres.Visible = false;
			// 
			// lblCurrentInputPort
			// 
			this.lblCurrentInputPort.Name = "lblCurrentInputPort";
			this.lblCurrentInputPort.Size = new System.Drawing.Size(93, 17);
			this.lblCurrentInputPort.Text = "Input Port: 65536";
			this.lblCurrentInputPort.Visible = false;
			// 
			// lblCurrentOutputPort
			// 
			this.lblCurrentOutputPort.Name = "lblCurrentOutputPort";
			this.lblCurrentOutputPort.Size = new System.Drawing.Size(101, 17);
			this.lblCurrentOutputPort.Text = "Output Port: 65536";
			this.lblCurrentOutputPort.Visible = false;
			// 
			// dlgSaveFile
			// 
			this.dlgSaveFile.Filter = "Wave Files|*.wav";
			this.dlgSaveFile.InitialDirectory = ".";
			this.dlgSaveFile.Title = "Save Text To Speech";
			// 
			// dlgOpenFile
			// 
			this.dlgOpenFile.Filter = "Text Files|*.txt|All Files|*.*";
			this.dlgOpenFile.Title = "Open File to Read";
			// 
			// bgwSave
			// 
			this.bgwSave.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwSave_DoWork);
			this.bgwSave.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwSave_RunWorkerCompleted);
			// 
			// FrmSocketSpeech
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(579, 349);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.gbConsole);
			this.Controls.Add(this.gbSpeech);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FrmSocketSpeech";
			this.Text = "SP-GEN - Socket Speech";
			this.Load += new System.EventHandler(this.FrmSocketSpeech_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FrmSocketSpeech_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FrmSocketSpeech_DragEnter);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSocketSpeech_FormClosing);
			this.gbSpeech.ResumeLayout(false);
			this.gbSpeech.PerformLayout();
			this.gbConsole.ResumeLayout(false);
			this.gbConsole.PerformLayout();
			this.statusBar.ResumeLayout(false);
			this.statusBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox gbSpeech;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnSpeech;
		private System.Windows.Forms.TextBox txtTextToSpeech;
		private System.Windows.Forms.GroupBox gbConsole;
		private System.Windows.Forms.TextBox txtConsole;
		private System.Windows.Forms.ComboBox cbVoices;
		private System.Windows.Forms.Label lblVoice;
		private System.Windows.Forms.StatusStrip statusBar;
		private System.Windows.Forms.ToolStripStatusLabel lblServerStarted;
		private System.Windows.Forms.ToolStripStatusLabel lblClientConnected;
		private System.Windows.Forms.ToolStripStatusLabel lblCurrentAddres;
		private System.Windows.Forms.ToolStripStatusLabel lblCurrentInputPort;
		private System.Windows.Forms.ToolStripStatusLabel lblCurrentOutputPort;
		private System.Windows.Forms.SaveFileDialog dlgSaveFile;
		private System.Windows.Forms.Label lblReadFile;
		private System.Windows.Forms.TextBox txtFileToRead;
		private System.Windows.Forms.Button btnReadFile;
		private System.Windows.Forms.Button btnExplore;
		private System.Windows.Forms.OpenFileDialog dlgOpenFile;
		private System.ComponentModel.BackgroundWorker bgwSave;
		private System.Windows.Forms.CheckBox chkAutoDisableSpRec;
		private System.Windows.Forms.ProgressBar pbSpeechProgress;
	}
}


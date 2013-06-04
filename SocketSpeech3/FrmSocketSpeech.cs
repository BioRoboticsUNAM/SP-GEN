using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Speech;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using Timer = System.Threading.Timer;
using Robotics;
using Robotics.API;
using Robotics.API.PrimitiveSharedVariables;
using Robotics.PacIto;

namespace SocketSpeech3
{
	public enum SpeechMode { Sync, Async };
	public delegate void SetTextboxTextCallback(TextBox textbox, string text);
	#region Delegates

	//private delegate void SetTextCallback(string text);
	public delegate bool SetTextCallback(string text);
	public delegate void StringEventHandler(string s);
	public delegate void VoidEventHandler();

	#endregion

	public partial class FrmSocketSpeech : Form, IMessageSource
	{
		#region Variables

		/// <summary>
		/// Indicates that the application is running on 64 bits machine
		/// </summary>
		private bool x64 = false;
	
		private StringEventHandler dlgConsole;
		private VoidEventHandler dlgUpdateVoiceCombo;
		private SetTextboxTextCallback dlgSetTextboxText;
		private SetTextboxTextCallback dlgAppendTextboxText;
		private DoubleEventHandler dlgUpdatePB;
		/// <summary>
		/// Indicates if module is busy
		/// </summary>
		private bool busy;
		/// <summary>
		/// Indicates if a sprec_na command is sent to SP-REC to disable
		/// speech recognition when the speech generator is working
		/// </summary>
		private bool autoDisableSpRec;
		/// <summary>
		/// Stores the previous state of the sp-Rec
		/// </summary>
		private bool spRecState;

		/// <summary>
		/// Speech generator
		/// </summary>
		private SpeechGenerator spGen;

		/// <summary>
		/// Audio player used to play audio files
		/// </summary>
		private IAudioPlayer audioPlayer;

		/// <summary>
		/// Command manager to manage commands
		/// </summary>
		private readonly PacItoCommandManager commandManager;
		/// <summary>
		/// Connection Manager used to manage connections
		/// </summary>
		private readonly ConnectionManager connectionManager;

		#region Command executers

		/// <summary>
		/// CommandExecuter for voice command
		/// </summary>
		private SyncCommandExecuter cexVoice;

		/// <summary>
		/// CommandExecuter for shutup command
		/// </summary>
		private SyncCommandExecuter cexShutUp;

		/// <summary>
		/// CommandExecuter for synchronous say command
		/// </summary>
		private AsyncCommandExecuter cexSay;
		private AsyncCommandExecuter cexSayCompatible;

		/// <summary>
		/// CommandExecuter for synchronous read command
		/// </summary>
		private CommandExecuter cexRead;
		private CommandExecuter cexReadCompatible;

		/// <summary>
		/// CommandExecuter for asynchronous say command
		/// </summary>
		private CommandExecuter cexAsay;

		/// <summary>
		/// CommandExecuter for asynchronous read command
		/// </summary>
		private CommandExecuter cexAread;

		/// <summary>
		/// CommandExecuter for synchronous play command
		/// </summary>
		private AsyncCommandExecuter cexPlay;

		/// <summary>
		/// CommandExecuter for asynchronous aplay command
		/// </summary>
		private AsyncCommandExecuter cexAplay;

		/// <summary>
		/// CommandExecuter for asynchronous playloop command
		/// </summary>
		private AsyncCommandExecuter cexPlayLoop;

		#endregion

		#endregion

		#region Constructor

		/// <summary>
		///  Initializes a new instance of FrmSocketSpeech
		/// </summary>
		public FrmSocketSpeech()
		{
			x64 = IntPtr.Size == 8;
			InitializeComponent();
			//if (x64 && Loq7SpeechGenerator.LoquendoInstalled)
			//if (Loq7SpeechGenerator.LoquendoInstalled)
			//	spGen = new Loq7SpeechGenerator();
			//else
				spGen = new SapiSpeechGenerator();
			spGen.SpeakStarted += new SpeakStartedEventHandler(spGen_SpeakStarted);
			spGen.SpeakCompleted += new SpeakCompletedEventHandler(spGen_SpeakCompleted);
			spGen.VoiceChanged += new VoiceChangedEventHandler(spGen_VoiceChanged);
			spGen.SpeakProgress += new SpeakProgressEventHandler(spGen_SpeakProgress);

			//if(!x64)
			audioPlayer = new AudioPlayer32();
			//else
			//	audioPlayer = new AudioPlayer64();

			TcpPortIn = 2052;
			TcpPortOut = 2052;
			TcpServerAddress = System.Net.IPAddress.Parse("127.0.0.1");

			commandManager = new PacItoCommandManager();
			commandManager.SharedVariablesLoaded += new SharedVariablesLoadedEventHandler(commandManager_SharedVariablesLoaded);
			commandManager.Started += new CommandManagerStatusChangedEventHandler(commandManager_Started);
			commandManager.Stopped += new CommandManagerStatusChangedEventHandler(commandManager_Stopped);
			connectionManager = new ConnectionManager(2052, commandManager);
			connectionManager.ClientConnected += new TcpClientConnectedEventHandler(connectionManager_ClientConnected);
			connectionManager.ClientDisconnected += new TcpClientDisconnectedEventHandler(connectionManager_ClientDisconnected);
			connectionManager.Connected += new TcpClientConnectedEventHandler(connectionManager_Connected);
			//connectionManager.DataReceived += new ConnectionManagerDataReceivedEH(connectionManager_DataReceived);
			connectionManager.Disconnected += new TcpClientDisconnectedEventHandler(connectionManager_Disconnected);

			cexAread = new SpgAreadCommandExecuter(spGen);
			cexAsay = new SpgAsayCommandExecuter(spGen);
			cexRead = new SpgReadCommandExecuter(spGen);
			cexSay = new SpgSayCommandExecuter(spGen);
			cexShutUp = new SpgShutUpCommandExecuter(spGen, audioPlayer);
			cexVoice = new SpgVoiceCommandExecuter(spGen);
			cexAplay = new SpgAplayCommandExecuter(audioPlayer);
			cexPlay = new SpgPlayCommandExecuter(audioPlayer);
			cexPlayLoop = new SpgPlayLoopCommandExecuter(audioPlayer);

			cexReadCompatible = new SpgReadCommandExecuter("read", spGen);
			cexSayCompatible = new SpgAsayCommandExecuter("say", spGen);

			commandManager.CommandExecuters.Add(cexAread);
			commandManager.CommandExecuters.Add(cexAsay);
			commandManager.CommandExecuters.Add(cexRead);
			commandManager.CommandExecuters.Add(cexSay);
			commandManager.CommandExecuters.Add(cexShutUp);
			commandManager.CommandExecuters.Add(cexVoice);
			commandManager.CommandExecuters.Add(cexAplay);
			commandManager.CommandExecuters.Add(cexPlay);
			commandManager.CommandExecuters.Add(cexPlayLoop);
			commandManager.CommandExecuters.Add(cexSayCompatible);
			commandManager.CommandExecuters.Add(cexReadCompatible);

			dlgConsole = new StringEventHandler(Console);
			dlgUpdateVoiceCombo = new VoidEventHandler(UpdateVoiceCombo);
			dlgSetTextboxText = new SetTextboxTextCallback(SetTextboxText);
			dlgAppendTextboxText = new SetTextboxTextCallback(AppendTextboxText);
			dlgUpdatePB = new DoubleEventHandler(UpdateProgressBar);

			AutoDisableSpRec = false;
			
		}

		#endregion

		#region Properties

		public bool AutoDisableSpRec
		{
			get { return autoDisableSpRec; }
			set
			{
				if (autoDisableSpRec == value) return;
				autoDisableSpRec = value;
				try
				{
					if (this.InvokeRequired)
					{
						if (!this.IsHandleCreated || this.IsDisposed || this.Disposing) return;
						Invoke( new VoidEventHandler(delegate(){chkAutoDisableSpRec.Checked = value; } ));
					}
					else chkAutoDisableSpRec.Checked = value;
				}catch{}
			}
		}

		public SpeechGenerator SpeechGenerator
		{
			get { return spGen; }
		}

		public string ModuleName
		{
			get { return connectionManager.ModuleName; }
		}

		#region Socket related

		/// <summary>
		/// Gets a value indicating if the module is woking in bidirectional mode
		/// </summary>
		public bool Bidirectional
		{
			get { return connectionManager.Bidirectional; }
		}

		/// <summary>
		/// Gets or sets the Tcp port for incoming data used by Tcp Server
		/// </summary>
		public int TcpPortIn
		{
			get { return connectionManager.PortIn; }
			set
			{
				if(connectionManager != null)
					connectionManager.PortIn = value;
				lblCurrentInputPort.Text = "Input port: " + value.ToString();
				//lblCurrentInputPort.Visible = true;
			}
		}

		/// <summary>
		/// Gets or sets the Tcp port for outgoing data used by Tcp Client
		/// </summary>
		public int TcpPortOut
		{
			get { return connectionManager.PortOut; }
			set
			{
				if (connectionManager != null)
					connectionManager.PortOut = value;
				lblCurrentOutputPort.Text = "Output port: " + value.ToString();
				//lblCurrentOutputPort.Visible = true;
			}
		}

		/// <summary>
		/// Gets or sets the IP Address of the remote computer to connect using the socket client
		/// </summary>
		public IPAddress TcpServerAddress
		{
			get { return connectionManager.TcpServerAddress; }
			set
			{
				if (connectionManager != null)
					connectionManager.TcpServerAddress = value;
				lblCurrentAddres.Text = "Server Address: " + value.ToString();
				//lblCurrentAddres.Visible = true;
			}
		}

		#endregion


		#endregion

		#region Methods

		/// <summary>
		/// Appends text to the console
		/// </summary>
		/// <param name="text">Text to append</param>
		private void Console(string text)
		{
			try
			{
				if (this.InvokeRequired)
				{
					if (!this.IsHandleCreated || this.IsDisposed || this.Disposing) return;
					this.BeginInvoke(dlgConsole, new object[] { text });
					return;
				}
				txtConsole.AppendText(text + "\r\n");
			}
			catch { }
		}

		private void AppendTextboxText(TextBox textbox, string text)
		{
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.IsDisposed || this.Disposing) return;
				this.BeginInvoke(dlgAppendTextboxText, textbox, text);
				return;
			}
			textbox.AppendText(text);
		}

		private void SetTextboxText(TextBox textbox, string text)
		{
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.IsDisposed || this.Disposing) return;
				this.BeginInvoke(dlgSetTextboxText, textbox, text);
				return;
			}
			textbox.Text = text;
		}

		private void UpdateProgressBar(double progress)
		{
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.IsDisposed || this.Disposing) return;
				this.BeginInvoke(dlgUpdatePB, progress);
				return;
			}
			pbSpeechProgress.Value = (int)progress;
		}

		private void UpdateVoiceCombo()
		{
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.IsDisposed || this.Disposing) return;
				this.BeginInvoke(dlgUpdateVoiceCombo);
				return;
			}
			cbVoices.Items.Clear();
			if ((spGen == null) || (spGen.VoiceNames == null) || (spGen.VoiceNames.Count < 1))
				return;
			for (int i = 0; i < spGen.VoiceNames.Count; ++i)
				cbVoices.Items.Add(spGen.VoiceNames[i]);
			if (spGen.SelectedVoiceName != (string)cbVoices.SelectedItem)
				cbVoices.SelectedItem = spGen.SelectedVoiceName;
		}

		#endregion

		#region Event Handler Functions

		#region Command Manager

		private void commandManager_SharedVariablesLoaded(CommandManager cmdMan)
		{
			string bbVoice;
			try
			{
				if (!cmdMan.SharedVariables.Contains("textToSpeech"))
					cmdMan.SharedVariables.Add(new StringSharedVariable("textToSpeech"));
				cmdMan.SharedVariables["textToSpeech"].Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteOthers);
				((StringSharedVariable)cmdMan.SharedVariables["textToSpeech"]).WriteNotification += new SharedVariableSubscriptionReportEventHadler<string>(textToSpeech_WriteNotification);

				if (!cmdMan.SharedVariables.Contains("currentVoice"))
					cmdMan.SharedVariables.Add(new StringSharedVariable("currentVoice"));
				bbVoice = ((StringSharedVariable)cmdMan.SharedVariables["currentVoice"]).Value;
				if (bbVoice != spGen.SelectedVoiceName)
				{
					if (spGen.VoiceNames.Contains(bbVoice))
						spGen.SelectedVoiceName = bbVoice;
					else
						((StringSharedVariable)cmdMan.SharedVariables["currentVoice"]).Write(spGen.SelectedVoiceName);
				}
				cmdMan.SharedVariables["currentVoice"].Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteOthers);
				((StringSharedVariable)cmdMan.SharedVariables["currentVoice"]).WriteNotification += new SharedVariableSubscriptionReportEventHadler<string>(currentVoice_WriteNotification);
			}
			catch { }
			commandManager.Ready = true;
		}

		private void textToSpeech_WriteNotification(SharedVariableSubscriptionReport<string> report)
		{
			if (String.IsNullOrEmpty(report.Value))
				return;
			//commandManager.BeginCommandExecution("spg_shutup", report.Value);
			commandManager.BeginCommandExecution("spg_asay", report.Value);
		}

		private void currentVoice_WriteNotification(SharedVariableSubscriptionReport<string> report)
		{
			if (String.IsNullOrEmpty(report.Value))
				return;
			commandManager.BeginCommandExecution("spg_voice", report.Value);
		}

		private void commandManager_Stopped(CommandManager commandManager)
		{
			Console("SocketSpeech 3.0 Stopped");
		}

		private void commandManager_Started(CommandManager commandManager)
		{
			Console("SocketSpeech 3.0 Started");
			Console("Loading Voices");

			lblCurrentOutputPort.Visible = !connectionManager.Bidirectional;
			lblCurrentAddres.Visible = !connectionManager.Bidirectional;
			lblServerStarted.Visible = true;
			lblCurrentInputPort.Visible = true;

			if ((spGen.VoiceNames == null) || (spGen.VoiceNames.Count < 1))
			{
				Console("No Voices Installed");
				commandManager.Stop();
				Application.Exit();
			}
			UpdateVoiceCombo();
			Console("Voices Loaded");

			//commandManager.Ready = true;
		}

		#endregion

		#region Connection Manager

		/// <summary>
		/// Manages the Disconnected event of the output socket
		/// </summary>
		/// <param name="s">Socket used for connection</param>
		private void connectionManager_Disconnected(EndPoint ep)
		{
			Console("Disconnected from " + ep.ToString());
			lblClientConnected.Visible = false;
		}

		/// <summary>
		/// Manages the DataReceived event of the input socket
		/// </summary>
		/// <param name="p">Received data</param>
		private void connectionManager_DataReceived(ConnectionManager cnnMan, TcpPacket packet)
		{
			string stringReceived = packet.DataString.Trim();
			Console("Rcv: " + "[" + packet.SenderIP.ToString() + "] " + stringReceived);
		}

		/// <summary>
		/// Manages the Connected event of the output socket
		/// </summary>
		/// <param name="s">Socket used for connection</param>
		private void connectionManager_Connected(Socket s)
		{
			Console("Connected to " + s.RemoteEndPoint.ToString());
			lblClientConnected.Visible = true;
		}

		/// <summary>
		/// Manages the ClientDisconnected event of the input socket
		/// </summary>
		/// <param name="s">Socket used for connection</param>
		private void connectionManager_ClientDisconnected(EndPoint ep)
		{
			try
			{
				Console("" + ep.ToString() + " disconnected from local server");
			}
			catch { Console("Client 0.0.0.0:0 disconnected from local server"); }
		}

		/// <summary>
		/// Manages the ClientConnected event of the input socket
		/// </summary>
		/// <param name="s">Socket used for connection</param>
		private void connectionManager_ClientConnected(Socket s)
		{
			Console(s.RemoteEndPoint.ToString() + " connected to local server");
		}

		#endregion

		#region SP-GEN

		private void spGen_SpeakCompleted(SpeechGenerator sender, string spokenString, SpeakOperationResult result)
		{
			try
			{
				if (autoDisableSpRec && spRecState)
				//if (autoDisableSpRec)
					commandManager.RobotSpeechRecognizer.Enabled = spRecState;
					//commandManager.RobotSpeechRecognizer.Enabled = true;
				
			}
			catch { }
			switch(result)
			{
				case SpeakOperationResult.Error:
					Console("\tError");
					break;

				case SpeakOperationResult.Canceled:
					Console("\tCanceled");
					break;

				default:
					Console("\tDone!");
					break;
			}
		}

		private void spGen_SpeakStarted(SpeechGenerator sender, string textToSay)
		{
			try
			{
				if (autoDisableSpRec)
				{
					spRecState = commandManager.RobotSpeechRecognizer.Enabled;
					//if(spRecState)
						commandManager.RobotSpeechRecognizer.Enabled = false;
				}
			}
			catch { }
			Console("Say: \"" + textToSay + "\"");
			pbSpeechProgress.Value = 0;
		}

		private void spGen_SpeakProgress(SpeechGenerator sender, double percentage)
		{
			UpdateProgressBar(100 * percentage);
		}

		private void spGen_VoiceChanged(SpeechGenerator sender, string voiceName)
		{
			UpdateVoiceCombo();
		}

		#endregion

		#region Key Management

		private void txtConsole_KeyDown(object sender, KeyEventArgs e)
		{
			/*
			if ((e.KeyCode != Keys.Up) && (e.KeyCode != Keys.Down) && (e.KeyCode != Keys.Left) && (e.KeyCode != Keys.Right)
				 && (e.KeyCode != Keys.PageDown) && (e.KeyCode != Keys.PageUp) && (e.KeyCode != Keys.Home)
				 && (e.KeyCode != Keys.End) && (e.KeyCode != Keys.Back) && (e.KeyCode != Keys.Delete) && (e.KeyCode != Keys.ShiftKey)
				)
				e.SuppressKeyPress = true;
			*/
		}

		#endregion

		#region Form Controls

		private void btnSave_Click(object sender, EventArgs e)
		{
			dlgSaveFile.FileName = (txtTextToSpeech.Text.Length >= 8 ? txtTextToSpeech.Text.Substring(0, 8) : txtTextToSpeech.Text) + ".wav";
			if (dlgSaveFile.ShowDialog() == DialogResult.OK)
			{
				btnSave.Enabled = false;
				bgwSave.RunWorkerAsync();
			}
		}

		private void btnReadFile_Click(object sender, EventArgs e)
		{
			try
			{
				if(!System.IO.File.Exists(txtFileToRead.Text)) return;
				spGen.SpeakAsync(System.IO.File.ReadAllText(txtFileToRead.Text));
			}
			catch { }
		}

		private void txtFileToRead_TextChanged(object sender, EventArgs e)
		{
			try
			{
				btnReadFile.Enabled = System.IO.File.Exists(txtFileToRead.Text);
			}
			catch { }
		}

		private void FrmSocketSpeech_DragDrop(object sender, DragEventArgs e)
		{
			// make sure they're actually dropping files (not text or anything else)
			if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
			{
				// allow them to continue
				// (without this, the cursor stays a "NO" symbol
				e.Effect = DragDropEffects.All;
				txtFileToRead.Text = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
			}

		}

		private void FrmSocketSpeech_DragEnter(object sender, DragEventArgs e)
		{
			// Check if the Dataformat of the data can be accepted
			// (we only accept file drops from Explorer, etc.)
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy; // Okay
			else
				e.Effect = DragDropEffects.None; // Unknown data, ignore it
		}

		private void btnExplore_Click(object sender, EventArgs e)
		{
			if (dlgOpenFile.ShowDialog() == DialogResult.OK)
				txtFileToRead.Text = dlgOpenFile.FileName;
		}

		private void bgwSave_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			if (!spGen.SaveTextToWav(txtTextToSpeech.Text, dlgSaveFile.FileName))
			{
				MessageBox.Show("Can't save file", "Export to Wav", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void bgwSave_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			btnSave.Enabled = true;
		}

		private void chkAutoDisableSpRec_CheckedChanged(object sender, EventArgs e)
		{
			AutoDisableSpRec = chkAutoDisableSpRec.Checked;
		}

		private void FrmSocketSpeech_Load(object sender, EventArgs e)
		{
			connectionManager.Start();
			commandManager.Start();
			//UpdateVoiceCombo();
		}

		private void btnSpeech_Click(object sender, EventArgs e)
		{
			commandManager.BeginCommandExecution("spg_asay", txtTextToSpeech.Text);
		}

		private void txtTextToSpeech_TextChanged(object sender, EventArgs e)
		{
			btnSave.Enabled = btnSpeech.Enabled = (txtTextToSpeech.Text.Length > 0);
		}

		private void cbVoices_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (spGen.SelectedVoiceName != (string)cbVoices.SelectedItem)
				spGen.SelectedVoiceName = (string)cbVoices.SelectedItem;
		}

		private void FrmSocketSpeech_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (connectionManager.IsRunning)
				connectionManager.Stop();
			commandManager.Stop();
		}

		#endregion

		#endregion

		#region IMessageSource Members

		public void ReceiveResponse(Response response)
		{
			return;
		}

		#endregion

	}
}


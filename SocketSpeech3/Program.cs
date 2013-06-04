using System;
using System.Net;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SocketSpeech3
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			PronunciationRule[] pRules;
			FrmSocketSpeech frmSocketSpeech;
			SpeechGenerator spGen;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			pRules = PronunciationRule.LoadFromXml(Application.StartupPath + "\\Pronunciation.xml");
			frmSocketSpeech = new FrmSocketSpeech();
			spGen = frmSocketSpeech.SpeechGenerator;
			AddDefaultPronunciationRules(spGen);
			if(pRules != null)
				spGen.Pronunciation.AddRange(pRules);
			spGen.Pronunciation.Sort();
			spGen.Pronunciation.RemoveDuplicates();
			if (args.Length > 0)
				parseArgs(frmSocketSpeech, args);
			Application.Run(frmSocketSpeech);
			PronunciationRule.SaveToXml(spGen.Pronunciation.ToArray(), Application.StartupPath + "\\Pronunciation.xml");
		}

		private static void AddDefaultPronunciationRules(SpeechGenerator spGen)
		{
			spGen.Pronunciation.Add("robot", "row-bought");
			spGen.Pronunciation.Add("_", " ", ReplaceMode.Normal, -1);
			spGen.Pronunciation.Add("stop", "stup");
		}

		private static void parseArgs(FrmSocketSpeech frmSocketSpeech, string[] args)
		{
			int resultInt;
			//byte resultByte;
			//double resultDouble;
			IPAddress resultIP;
			for (int i = 0; i < args.Length; ++i)
			{

				switch (args[i].ToLower())
				{
					case "-a":
						if (++i > args.Length) return;
						if (IPAddress.TryParse(args[i], out resultIP))
							frmSocketSpeech.TcpServerAddress = resultIP;
						break;

					case "-h":
					case "--h":
					case "-help":
					case "--help":
					case "/h":
						showHelp();
						break;

					case "-r":
						if (++i > args.Length) return;
						if (Int32.TryParse(args[i], out resultInt) && (resultInt >= 0))
							frmSocketSpeech.TcpPortIn = resultInt;
						break;

					case "-w":
						if (++i > args.Length) return;
						if (Int32.TryParse(args[i], out resultInt) && (resultInt >= 0))
							frmSocketSpeech.TcpPortOut = resultInt;
						break;

					case "-v":
						if (++i > args.Length) return;
						if (Int32.TryParse(args[i], out resultInt) && (resultInt >= 0))
							frmSocketSpeech.SpeechGenerator.SelectedVoiceIndex = resultInt;
						else
						{
							try
							{
								frmSocketSpeech.SpeechGenerator.SelectedVoiceName = args[i];
							}
							catch { }
						}
						break;
				}
			}
		}

		private static void showHelp()
		{
			// -a 127.0.0.1 -r 2001 -w 2000 -sim
			Console.WriteLine("SocketSpeech Help");
			Console.WriteLine("-autosockets\tSocket autoconnect");
			Console.WriteLine("-a\t\tTcp server Address");
			Console.WriteLine("-r\t\tTcp input port (server)");
			Console.WriteLine("-w\t\tTcp output port (client)");
			Console.WriteLine("-v\t\tSelected voice by index or name");
		}
	}
}
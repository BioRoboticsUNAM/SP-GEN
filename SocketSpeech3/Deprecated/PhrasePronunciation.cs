using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace SocketSpeech3
{
	[Serializable]
	[XmlRoot("phrasePronunciation")]
	public class PhrasePronunciation
	{
		#region Variables

		private string phrase;

		private string phrasePattern;

		private string pronunciation;

		#endregion

		#region Constructor

		public PhrasePronunciation()
		{
			this.Phrase = " ";
			this.Pronunciation = " ";
		}

		public PhrasePronunciation(string phrase, string pronunciation)
		{
			this.Phrase = phrase;
			this.Pronunciation = pronunciation;
		}

		#endregion

		#region Properties

		[XmlElement("phrase")]
		public string Phrase
		{
			get { return this.phrase; }
			set
			{
				this.phrase = value;
				this.phrasePattern = Regex.Escape(phrase);
			}
		}

		[XmlElement("pronunciation")]
		public string Pronunciation
		{
			get { return this.pronunciation; }
			set { this.pronunciation = value; }
		}

		#endregion

		#region Methods

		public void ReplaceOnString(ref string str)
		{
			if (String.IsNullOrEmpty(str))
				return;
			str = Regex.Replace(str, this.phrasePattern, this.pronunciation, RegexOptions.IgnoreCase);
		}

		#endregion

		#region Static Members

		private static ReaderWriterLock rwPronunciations = new ReaderWriterLock();

		private static List<PhrasePronunciation> pronunciations = new List<PhrasePronunciation>();
		private static SortedList<string, PhrasePronunciation> pronunciationsByPhrase = new SortedList<string, PhrasePronunciation>(StringComparer.InvariantCultureIgnoreCase);

		public static bool AddPronunciation(string phrase, string pronunciation)
		{
			return false;
		}

		public static bool RemovePronunciation(string phrase)
		{
			return false;
		}

		public static void LoadPronunciationsList(string filePath)
		{
			XmlSerializer serializer;
			XmlRootAttribute root;
			FileStream fs;

			root = new XmlRootAttribute("pronunciationList");
			root.Namespace = null;
			root.DataType = null;
			serializer = new XmlSerializer(typeof(List<PhrasePronunciation>), root);
			fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);

			rwPronunciations.AcquireWriterLock(-1);
			pronunciationsByPhrase.Clear();
			try
			{
				pronunciations = (List<PhrasePronunciation>)serializer.Deserialize(fs);
			}
			catch
			{
				pronunciations = new List<PhrasePronunciation>();
				pronunciations.Add(new PhrasePronunciation("robot", "row bought"));
			}

			for (int i = 0; i < pronunciations.Count; ++i)
			{
				if (!pronunciationsByPhrase.ContainsKey(pronunciations[i].Phrase))
					pronunciationsByPhrase.Add(pronunciations[i].Phrase, pronunciations[i]);
				else
				{
					pronunciations.RemoveAt(i);
					--i;
				}
			}

			rwPronunciations.ReleaseWriterLock();
			fs.Close();
		}

		public static void SavePronunciationsList(string filePath)
		{
			XmlSerializer serializer;
			XmlRootAttribute root;
			FileStream fs;

			root = new XmlRootAttribute("pronunciationList");
			root.Namespace = null;
			root.DataType = null;
			serializer = new XmlSerializer(typeof(List<PhrasePronunciation>), root);
			fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

			rwPronunciations.AcquireReaderLock(-1);
			serializer.Serialize(fs, pronunciations);
			rwPronunciations.ReleaseReaderLock();
			fs.Close();
		}

		#endregion

		public static string ReplacePronunciations(string str)
		{
			string s = String.Copy(str);

			rwPronunciations.AcquireReaderLock(-1);
			foreach (PhrasePronunciation pp in pronunciations)
			{
				pp.ReplaceOnString(ref s);
			}
			rwPronunciations.ReleaseReaderLock();
			return s;
		}
	}
}

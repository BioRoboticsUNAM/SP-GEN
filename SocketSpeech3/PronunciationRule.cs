using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;


namespace SocketSpeech3
{
	/// <summary>
	/// Enumerates the replacement modes for pronunciation rules 
	/// </summary>
	public enum ReplaceMode
	{
		/// <summary>
		/// Occurences are replaced as found
		/// </summary>
		Normal = 0,
		/// <summary>
		/// Only space-delimited ocurrences are replaced
		/// </summary>
		FullWords = 1
	}

	/// <summary>
	/// Represents a rule of pronunciation
	/// </summary>
	[Serializable]
	[XmlRoot("pronunciationRule")]
	[XmlType(IncludeInSchema = false, TypeName = "pronunciationRule")]
	public class PronunciationRule : IComparable<PronunciationRule>
	{
		#region Variables

		/// <summary>
		/// The base word or phrase
		/// </summary>
		private string word;

		/// <summary>
		/// The pronunciation string of the base word of phrase
		/// </summary>
		private string pronunciation;

		/// <summary>
		/// The replacement mode for pronunciation rules
		/// </summary>
		private ReplaceMode replaceMode;

		/// <summary>
		/// An integer value used to sort the pronuniation rules
		/// </summary>
		private int priority;

		/// <summary>
		/// A regular expression used to match the base word
		/// </summary>
		private Regex rxPronunciationReplacer;

		/// <summary>
		/// The evaluator function which will do the replacement of the matched strings
		/// </summary>
		private MatchEvaluator evaluator;

		/// <summary>
		/// Regular expression used to validate pronunciations
		/// </summary>
		private static Regex rxPronunciationValidator = new Regex(@"[\'\-\w\s\d]+", RegexOptions.Compiled);

		/// <summary>
		/// Regular expression used to validate words or phrases
		/// </summary>
		private static Regex rxWordValidator = new Regex(@"\S+(\s+\S+)*", RegexOptions.Compiled);

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of PronunciationRule
		/// </summary>
		public PronunciationRule()
		{
			this.word = null;
			this.pronunciation = null;
			this.rxPronunciationReplacer = null;
			this.priority = 0;
			this.replaceMode = ReplaceMode.FullWords;
			evaluator = new MatchEvaluator(Evaluator);
		}

		/// <summary>
		/// Initializes a new instance of PronunciationRule
		/// </summary>
		/// <param name="word">The base word or phrase</param>
		/// <param name="pronunciation">The pronunciation string of the base word of phrase</param>
		public PronunciationRule(string word, string pronunciation):
			this(word, pronunciation, ReplaceMode.FullWords, 0){}

		/// <summary>
		/// Initializes a new instance of PronunciationRule
		/// </summary>
		/// <param name="word">The base word or phrase</param>
		/// <param name="pronunciation">The pronunciation string of the base word of phrase</param>
		/// <param name="replaceMode">The replacement mode for pronunciation rules</param>
		public PronunciationRule(string word, string pronunciation, ReplaceMode replaceMode):
			this(word, pronunciation, replaceMode, 0){}

		/// <summary>
		/// Initializes a new instance of PronunciationRule
		/// </summary>
		/// <param name="word">The base word or phrase</param>
		/// <param name="pronunciation">The pronunciation string of the base word of phrase</param>
		/// <param name="priority">An integer value used to sort the pronuniation rules</param>
		public PronunciationRule(string word, string pronunciation, int priority) :
			this(word, pronunciation, ReplaceMode.FullWords, priority) { }

		/// <summary>
		/// Initializes a new instance of PronunciationRule
		/// </summary>
		/// <param name="word">The base word or phrase</param>
		/// <param name="pronunciation">The pronunciation string of the base word of phrase</param>
		/// <param name="replaceMode">The replacement mode for pronunciation rules</param>
		/// <param name="priority">An integer value used to sort the pronuniation rules</param>
		public PronunciationRule(string word, string pronunciation, ReplaceMode replaceMode, int priority):this()
		{
			this.Word = word;
			this.Pronunciation = pronunciation;
			this.ReplaceMode = replaceMode;
			this.Priority = priority;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the base word or phrase
		/// </summary>
		[XmlElement("word")]
		public string Word
		{
			get { return this.word; }
			set
			{
				if (this.replaceMode == ReplaceMode.Normal)
				{
					this.word = value;
					return;
				}

				if (!rxWordValidator.IsMatch(value))
					throw new ArgumentException("Invalid base word/phrase");
				this.word = value;

				string pattern = Regex.Escape(word);
				this.rxPronunciationReplacer = new Regex(@"(^|\s+)" + word + @"(\s+|$)", RegexOptions.Compiled);
			}
		}

		/// <summary>
		/// Gets or sets the pronunciation string of the base word of phrase
		/// </summary>
		[XmlElement("pronunciation")]
		public string Pronunciation
		{
			get { return this.pronunciation; }
			set
			{
				if ((this.replaceMode != ReplaceMode.Normal) && !rxPronunciationValidator.IsMatch(value))
					throw new ArgumentException("Invalid pronunciation");
				this.pronunciation = value;
				
			}
		}

		/// <summary>
		/// Gets or sets the priority of the pronuniation rule.
		/// Rules with a lower number executes first 
		/// </summary>
		[XmlAttribute(AttributeName = "priority")]
		[DefaultValue(0)]
		public int Priority
		{
			get { return priority; }
			set { priority = value; }
		}

		/// <summary>
		/// Gets ot sets the replacement modes for pronunciation rules
		/// </summary>
		[XmlAttribute(AttributeName = "replaceMode")]
		[DefaultValue(1)]
		public ReplaceMode ReplaceMode
		{
			get { return this.replaceMode; }
			set { this.replaceMode = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Replaces all ocurrences of the base word/phrase in the input sentence string by the pronunciation string
		/// </summary>
		/// <param name="sentence">The input sentence to replace pronunciation words</param>
		/// <returns>The input sentence with all the ocurrences of the base word/phrase replaced by the pronunciation string</returns>
		public string ReplaceWordByPronunciation(string sentence)
		{
			if (this.replaceMode == ReplaceMode.Normal)
				return sentence.Replace(this.word, this.pronunciation);

			if (rxPronunciationReplacer == null)
				return sentence;
			return rxPronunciationReplacer.Replace(sentence, evaluator);
		}

		private string Evaluator(Match match)
		{
			return match.Value.Replace(word, pronunciation);
		}

		public override string ToString()
		{
			string pString;

			pString = (this.Priority < 0) ?
				"-" + (this.Priority * -1).ToString().PadLeft(3, '0') :
				this.Priority.ToString().PadLeft(4, '0');

			return "P" +pString+ ": \"" + this.Word + "\" => \"" + this.Pronunciation + "\" | Mode: " + this.ReplaceMode.ToString();
		}

		/// <summary>
		/// Gets an array of pronunciation rules loaded from an XML file
		/// </summary>
		/// <param name="path">Path to the xml file</param>
		/// <returns>An array of pronunciation rules</returns>
		public static PronunciationRule[] LoadFromXml(string path)
		{
			XmlSerializer serializer;
			XmlRootAttribute root;
			FileStream fs;
			PronunciationRule[] rules;
			XmlReaderSettings settings;

			root = new XmlRootAttribute("pronunciations");
			root.Namespace = null;
			root.DataType = null;

			settings = new XmlReaderSettings();
			settings.IgnoreWhitespace = false;

			fs = null;
			try
			{
				serializer = new XmlSerializer(typeof(PronunciationRule[]), root);
				fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
				using (XmlReader reader = XmlReader.Create(fs, settings))
				{
					rules = (PronunciationRule[])serializer.Deserialize(reader);
				}
			}
			catch { return null; }
			finally { if(fs != null) fs.Close(); }

			return rules;
		}

		/// <summary>
		/// Saves an array of pronunciation rules into an XML file
		/// </summary>
		/// <param name="path">Path to the xml file</param>
		/// <returns>An array of pronunciation rules</returns>
		public static void SaveToXml(PronunciationRule[] rules, string path)
		{
			XmlSerializer serializer;
			XmlRootAttribute root;
			FileStream fs = null;
			XmlSerializerNamespaces ns;
			XmlWriterSettings settings;

			root = new XmlRootAttribute("pronunciations");
			root.Namespace = null;
			root.DataType = null;
			
			ns = new XmlSerializerNamespaces();
			ns.Add(String.Empty, String.Empty);

			settings = new XmlWriterSettings();
			settings.Encoding = System.Text.Encoding.UTF8;
			settings.Indent = true;
			settings.IndentChars = "\t";
			settings.NewLineHandling = NewLineHandling.Entitize;

			try
			{
				fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
				serializer = new XmlSerializer(typeof(PronunciationRule[]), root);

				using (XmlWriter writer = XmlWriter.Create(fs, settings))
				{
					serializer.Serialize(writer, rules, ns);
				}
			}
			catch { }
			finally
			{ 
				if(fs!= null)
				fs.Close();
			}
		}

		#endregion

		#region IComparable<PronunciationRule> Members

		public int CompareTo(PronunciationRule other)
		{
			if (other == null)
				return this.Priority.CompareTo(Int32.MaxValue);
			if (this.Priority != other.Priority)
				return this.Priority.CompareTo(other.Priority);

			return this.Word.CompareTo(other.Word);
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SocketSpeech3
{
	/// <summary>
	/// Implements a List of PronunciationRule objects
	/// </summary>
	public class PronunciationRuleList :List<PronunciationRule>
	{
		#region Properties
		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the PronunciationRuleList class that is empty and has the default initial capacity.
		/// </summary>
		public PronunciationRuleList() : base() { }

		/// <summary>
		/// Initializes a new instance of the PronunciationRuleList class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
		/// </summary>
		/// <param name="collection">The collection whose elements are copied to the new list.
		/// </param>
		public PronunciationRuleList(IEnumerable<PronunciationRule> collection) : base(collection) { }

		/// <summary>
		/// Initializes a new instance of the PronunciationRuleList class that is empty and has the specified initial capacity.
		/// </summary>
		/// <param name="capacity">The number of elements that the new list can initially store.</param>
		public PronunciationRuleList(int capacity) : base(capacity) { }

		#endregion

		#region Methods

		/// <summary>
		/// Adds a new PronunciationRule object to the PronunciationRuleList
		/// </summary>
		/// <param name="word">The base word or phrase</param>
		/// <param name="pronunciation">The pronunciation string of the base word of phrase</param>
		public void Add(string word, string pronunciation)
		{
			int ix;

			ix = IndexOf(word);
			if (ix != -1)
				this.RemoveAt(ix);
			base.Add(new PronunciationRule(word, pronunciation));
		}

		/// <summary>
		/// Adds a new PronunciationRule object to the PronunciationRuleList
		/// </summary>
		/// <param name="word">The base word or phrase</param>
		/// <param name="pronunciation">The pronunciation string of the base word of phrase</param>
		/// <param name="replaceMode">The replacement mode for pronunciation rules</param>
		/// <param name="priority">An integer value used to sort the pronuniation rules</param>
		public void Add(string word, string pronunciation, ReplaceMode replaceMode, int priority)
		{
			int ix;

			ix = IndexOf(word);
			if (ix != -1)
				this.RemoveAt(ix);
			base.Add(new PronunciationRule(word, pronunciation, replaceMode, priority));
		}

		/// <summary>
		/// Determines whether a word is in the PronunciationRuleList collection
		/// </summary>
		/// <param name="word">The base word or phrase to search</param>
		/// <returns>true if the provided word is in the PronunciationRuleList collection, false otherwise</returns>
		public bool Contains(string word)
		{
			return IndexOf(word) != -1;
		}

		/// <summary>
		/// Searches for the specified word and returns the zero-based index of the first occurrence within the entire PronunciationRuleList.
		/// </summary>
		/// <param name="word">The base word/phrase to locate in the PronunciationRuleList. The value can be null reference</param>
		/// <returns>The zero-based index of the first occurrence of item within the entire PronunciationRuleList, if found; otherwise, –1.</returns>
		public int IndexOf(string word)
		{
			return this.IndexOf(word, 0);
		}

		/// <summary>
		/// Searches for the specified word and returns the zero-based index of the first occurrence within the range of elements in the PronunciationRuleList that extends from the specified index to the last element.
		/// </summary>
		/// <param name="word">The base word/phrase to locate in the PronunciationRuleList. The value can be null reference</param>
		/// <param name="index">The zero-based starting index of the search</param>
		/// <returns>The zero-based index of the first occurrence of word within the range of elements in the PronunciationRuleList that extends from index to the last element, if found; otherwise, –1.</returns>
		public int IndexOf(string word, int index)
		{
			if ((index < 0) || (index > this.Count))
				throw new ArgumentOutOfRangeException();
			for (int i = index; i < this.Count; ++i)
			{
				if (String.Compare(this[i].Word, word, true) == 0)
					return i;
			}
			return -1;
		}

		/// <summary>
		/// Removes all duplicate replacements from the PronunciationRuleList
		/// </summary>
		public void RemoveDuplicates()
		{
			int iow;

			this.Sort();
			for (int i = 0; i < this.Count-1; ++i)
			{
				do
				{
					iow = this.IndexOf(this[i].Word, i + 1);
					if (iow != -1)
						this.RemoveAt(iow);
				} while (iow != -1);
			}
		}

		#endregion
	}
}

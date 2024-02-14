using System;
using System.Collections.Generic;
using System.Linq;

namespace Counter.Core
{
    ///<summary>
    /// Provides sequential word counting functionality.
    ///</summary>
    public class WordCounter : IWordCounter
    {
        ///<summary>
        /// Gets the the array of char delimiters.
        ///</summary>
        protected virtual char[] Delimiters => new[] { ' ', '\r', '\n', '\r', '\t', '.', ',', ';', ':', '!', '?', '(', ')', '[', ']', '{', '}', '\"', '\'', '-', '_', '/', '\\', '@', '#', '%', '&', '*' };

        #region Utilities

        /// <summary>
        /// Updates the word frequencies in the provided dictionary based on the input collection of words.
        /// If a word is already present in the dictionary, its frequency is incremented; otherwise, a new entry is added with a frequency of 1.
        /// </summary>
        /// <param name="frequencyDictionary">The dictionary containing word frequencies to be updated.</param>
        /// <param name="words">The collection of words to process and update frequencies.</param>
        /// <returns>The updated word frequency dictionary.</returns>
        protected virtual IDictionary<string, int> UpdateWordFrequencyDictionary(IDictionary<string, int> frequencyDictionary, IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                if (frequencyDictionary.TryGetValue(word, out var value))
                    frequencyDictionary[word] = ++value;
                else
                    frequencyDictionary[word] = 1;
            }

            return frequencyDictionary;
        }

        ///<summary>
        /// Splits the given content into words.
        ///</summary>
        ///<param name="content">The content to split into words.</param>
        ///<returns>An array of words.</returns>
        protected virtual string[] GetWords(string content)
        {
            return content.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);

            // for performance using predefined content delimiters and not regex
            //return Regex.Split(content, @"[\s\p{P}]")
            //            .Where(w => !string.IsNullOrEmpty(w))
            //            .ToArray();
        }

        /// <summary>
        /// Extracts the remaining word from an array of words and a given chunk, considering punctuation for proper word concatenation.
        /// </summary>
        /// <param name="words">An array of words.</param>
        /// <param name="chunk">The string chunk to analyze.</param>
        /// <returns>The remaining word, with appended punctuation if the last character of the chunk is a punctuation mark.</returns>
        protected virtual string GetRemainingWord(string[] words, string chunk)
        {
            var remainingWord = words[^1];

            // If the last character of the chunk is a punctuation mark, append it to the remaining word.
            // For example, if the ending word of current chunk is "of " and the next chunk starts with "the".
            // not appending the punctuation would result in a new word "ofthe".
            if (Delimiters.Contains(chunk[^1]))
                remainingWord += chunk[^1];

            return remainingWord;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Counts the frequencies of words in the given content and updates the provided frequency dictionary.
        /// </summary>
        /// <param name="content">The content in which word frequencies will be counted.</param>
        /// <param name="frequencyDictionary">An existing dictionary to store word frequencies. If null, a new dictionary is created.</param>
        /// <returns>A dictionary containing word frequencies.</returns>
        public virtual IDictionary<string, int> CountWordFrequencies(string content, IDictionary<string, int> frequencyDictionary)
        {
            frequencyDictionary ??= new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(content))
                return frequencyDictionary;

            var words = GetWords(content);

            frequencyDictionary = UpdateWordFrequencyDictionary(frequencyDictionary, words);

            return frequencyDictionary;
        }

        /// <summary>
        /// Updates the word frequencies in the provided dictionary based on the input string chunk,
        /// previous remaining word, and returns the remaining word for the next processing iteration.
        /// </summary>
        /// <param name="chunk">The input string chunk to analyze.</param>
        /// <param name="frequencyDictionary">The dictionary containing word frequencies to be updated.</param>
        /// <param name="previousRemainingWord">The remaining word from the previous iteration.</param>
        /// <param name="remainingWord">Out parameter to store the remaining word for the next iteration.</param>
        /// <returns>The updated word frequency dictionary.</returns>
        public virtual IDictionary<string, int> CountWordFrequenciesFromChunk(string chunk,
            IDictionary<string, int> frequencyDictionary, string previousRemainingWord, out string remainingWord)
        {
            remainingWord = "";

            frequencyDictionary ??= new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(chunk))
                return frequencyDictionary;

            var words = GetWords(previousRemainingWord + chunk);

            remainingWord = GetRemainingWord(words, chunk);

            frequencyDictionary = UpdateWordFrequencyDictionary(frequencyDictionary, words[..^1]);

            return frequencyDictionary;
        }

        #endregion
    }
}

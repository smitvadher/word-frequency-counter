using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Counter.Core
{
    ///<summary>
    /// Provides parallel word counting functionality.
    ///</summary>
    public class ParallelWordCounter : WordCounter
    {
        #region Utilities

        /// <summary>
        /// Updates the word frequencies in the provided dictionary based on the input collection of words.
        /// If a word is already present in the dictionary, its frequency is incremented; otherwise, a new entry is added with a frequency of 1.
        /// </summary>
        /// <param name="frequencyDictionary">The dictionary containing word frequencies to be updated.</param>
        /// <param name="words">The collection of words to process and update frequencies.</param>
        /// <returns>The updated word frequency dictionary.</returns>
        protected override IDictionary<string, int> UpdateWordFrequencyDictionary(IDictionary<string, int> frequencyDictionary, IEnumerable<string> words)
        {
            var dict = (ConcurrentDictionary<string, int>)frequencyDictionary;

            Parallel.ForEach(words,
            word =>
            {
                dict.AddOrUpdate(word, 1, (_, count) => count + 1);
            });

            return dict;
        }

        #endregion

        #region Methods

        ///<summary>
        /// Counts the frequencies of words in the given content in parallel.
        ///</summary>
        ///<param name="content">The content in which word frequencies will be counted.</param>
        ///<param name="frequencyDictionary">An existing dictionary to store word frequencies. If null, a new concurrent dictionary is created.</param>
        ///<returns>A dictionary containing word frequencies.</returns>
        ///<remarks>
        /// The method uses Parallel.ForEach to count word frequencies in parallel using a ConcurrentDictionary.
        ///</remarks>
        public override IDictionary<string, int> CountWordFrequencies(string content, IDictionary<string, int> frequencyDictionary)
        {
            frequencyDictionary ??= new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (!(frequencyDictionary is ConcurrentDictionary<string, int> dict))
                throw new Exception("Concurrent dictionary is required to calculate word frequencies parallel.");

            if (string.IsNullOrEmpty(content))
                return dict;

            var words = GetWords(content);

            dict = (ConcurrentDictionary<string, int>)UpdateWordFrequencyDictionary(dict, words);

            return dict;
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
        public override IDictionary<string, int> CountWordFrequenciesFromChunk(string chunk,
            IDictionary<string, int> frequencyDictionary, string previousRemainingWord, out string remainingWord)
        {
            remainingWord = "";

            frequencyDictionary ??= new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            if (!(frequencyDictionary is ConcurrentDictionary<string, int> dict))
                throw new Exception("Concurrent dictionary is required to calculate word frequencies parallel.");

            if (string.IsNullOrEmpty(chunk))
                return dict;

            var words = GetWords(previousRemainingWord + chunk);

            remainingWord = GetRemainingWord(words, chunk);

            dict = (ConcurrentDictionary<string, int>)UpdateWordFrequencyDictionary(dict, words[..^1]);

            return dict;
        }

        #endregion
    }
}

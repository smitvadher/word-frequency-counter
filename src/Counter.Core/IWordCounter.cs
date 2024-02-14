using System.Collections.Generic;

namespace Counter.Core
{
    ///<summary>
    /// Represents an interface for word counting operations.
    ///</summary>
    public interface IWordCounter
    {
        /// <summary>
        /// Counts the frequencies of words in the given content and updates the provided frequency dictionary.
        /// </summary>
        /// <param name="content">The content in which word frequencies will be counted.</param>
        /// <param name="frequencyDictionary">An existing dictionary to store word frequencies. If null, a new dictionary is created.</param>
        /// <returns>A dictionary containing word frequencies.</returns>
        IDictionary<string, int> CountWordFrequencies(string content, IDictionary<string, int> frequencyDictionary);

        /// <summary>
        /// Updates the word frequencies in the provided dictionary based on the input string chunk,
        /// previous remaining word, and returns the remaining word for the next processing iteration.
        /// </summary>
        /// <param name="chunk">The input string chunk to analyze.</param>
        /// <param name="frequencyDictionary">The dictionary containing word frequencies to be updated.</param>
        /// <param name="previousRemainingWord">The remaining word from the previous iteration.</param>
        /// <param name="remainingWord">Out parameter to store the remaining word for the next iteration.</param>
        /// <returns>The updated word frequency dictionary.</returns>
        IDictionary<string, int> CountWordFrequenciesFromChunk(string chunk,
            IDictionary<string, int> frequencyDictionary, string previousRemainingWord, out string remainingWord);
    }
}

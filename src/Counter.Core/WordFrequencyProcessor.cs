using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Counter.Core
{
    ///<summary>
    /// Processes word frequencies in a file.
    ///</summary>
    public class WordFrequencyProcessor
    {
        #region Fields

        private readonly IFileProvider _fileProvider;
        private readonly IWordCounter _wordCounter;

        #endregion

        #region Ctor

        ///<summary>
        /// Initializes a new instance of the WordFrequencyProcessor class.
        ///</summary>
        ///<param name="fileProvider">The file provider for reading and writing files.</param>
        ///<param name="wordCounter">The word counter for counting word frequencies.</param>
        public WordFrequencyProcessor(IFileProvider fileProvider, IWordCounter wordCounter)
        {
            _fileProvider = fileProvider;
            _wordCounter = wordCounter;
        }

        #endregion

        #region Methods

        ///<summary>
        /// Asynchronously processes a file to count word frequencies and writes the result to another file.
        ///</summary>
        ///<param name="inputFilePath">The path of the input file.</param>
        ///<param name="outputFilePath">The path of the output file.</param>
        ///<remarks>
        /// Validates input file paths, reads the content of the input file, counts word frequencies,
        /// formats the result, and writes it to the output file.
        ///</remarks>
        public async Task ProcessFileAsync(string inputFilePath, string outputFilePath)
        {
            if (string.IsNullOrEmpty(inputFilePath))
                throw new ArgumentNullException(nameof(inputFilePath));

            if (string.IsNullOrEmpty(outputFilePath))
                throw new ArgumentNullException(nameof(outputFilePath));

            var sw = new Stopwatch();
            sw.Start();

            IDictionary<string, int> wordFrequencies = null;

            var previousRemainingWord = "";
            await foreach (var line in _fileProvider.ReadFileChunksAsync(inputFilePath, bufferIterations: 10))
                wordFrequencies = _wordCounter.CountWordFrequenciesFromChunk(line, wordFrequencies, previousRemainingWord, out previousRemainingWord);

            if (!string.IsNullOrEmpty(previousRemainingWord))
                wordFrequencies = _wordCounter.CountWordFrequencies(previousRemainingWord, wordFrequencies);

            if (wordFrequencies != null)
            {
                var outputContent = GetOutputFileContent(wordFrequencies);
                await _fileProvider.WriteFileAsync(outputFilePath, outputContent);
            }

            sw.Stop();
            Console.WriteLine($"Time {sw.Elapsed}");
        }

        #endregion

        #region Protected methods and props

        ///<summary>
        /// Gets the format string for formatting output lines.
        ///</summary>
        ///<remarks>
        /// The format string includes placeholders for the word and its frequency.
        ///</remarks>
        private string OutputLineFormat => "{0},{1}";

        ///<summary>
        /// Formats word frequencies into a string for writing to the output file.
        ///</summary>
        ///<param name="wordFrequencies">The dictionary containing word frequencies.</param>
        ///<returns>The formatted content for the output file.</returns>
        ///<remarks>
        /// The method orders the word frequencies and formats them using the specified format string.
        ///</remarks>
        private string GetOutputFileContent(IDictionary<string, int> wordFrequencies)
        {
            var lines = wordFrequencies.OrderByDescending(pair => pair.Value)
                .ThenBy(pair => pair.Key)
                .Select(x => string.Format(OutputLineFormat, x.Key, x.Value));

            return string.Join(Environment.NewLine, lines);
        }

        #endregion
    }
}

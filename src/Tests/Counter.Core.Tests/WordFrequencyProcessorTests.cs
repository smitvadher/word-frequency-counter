using Moq;

namespace Counter.Core.Tests
{
    public class WordFrequencyProcessorTests
    {
        [Fact]
        public async Task ProcessFileAsync_EmptyInputPath_ThrowsArgumentNullException()
        {
            // Arrange
            var inputFilePath = "";
            var outputFilePath = "TestFiles\\OutputFile1.txt";

            var fileProviderMock = new Mock<IFileProvider>();
            var wordCounterMock = new Mock<IWordCounter>();
            var wordFrequencyProcessor = new WordFrequencyProcessor(fileProviderMock.Object, wordCounterMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => wordFrequencyProcessor.ProcessFileAsync(inputFilePath, outputFilePath));
        }

        [Fact]
        public async Task ProcessFileAsync_EmptyOutputPath_ThrowsArgumentNullException()
        {
            // Arrange
            var inputFilePath = "TestFiles\\InputFile1.txt";
            var outputFilePath = "";

            var fileProviderMock = new Mock<IFileProvider>();
            var wordCounterMock = new Mock<IWordCounter>();
            var wordFrequencyProcessor = new WordFrequencyProcessor(fileProviderMock.Object, wordCounterMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => wordFrequencyProcessor.ProcessFileAsync(inputFilePath, outputFilePath));
        }

        [Fact]
        public async Task ProcessFileAsync_ValidPaths_ProcessesFileAndWritesResult()
        {
            // Arrange
            var inputFilePath = "TestFiles\\InputFile1.txt";
            var outputFilePath = "TestFiles\\OutputFile1.txt";

            var fileProviderMock = new Mock<IFileProvider>();
            var wordCounterMock = new Mock<IWordCounter>();
            var wordFrequencyProcessor = new WordFrequencyProcessor(fileProviderMock.Object, wordCounterMock.Object);

            #region Mock setup

            fileProviderMock.Setup(fp => fp.ReadFileChunksAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns((new[] { "This is a sample sentence. Sample sentence for testing." }).ToAsyncEnumerable());

            var remainingWord = "testing";
            wordCounterMock.Setup(wc => wc.CountWordFrequenciesFromChunk(It.IsAny<string>(),
                    It.IsAny<IDictionary<string, int>>(),
                    It.IsAny<string>(),
                    out remainingWord))
                .Returns(new Dictionary<string, int>
                {
                    { "This", 1 },
                    { "is", 1 },
                    { "a", 1 },
                    { "sample", 2 },
                    { "sentence", 2 },
                    { "for", 1 },
                });

            wordCounterMock.Setup(wc => wc.CountWordFrequencies(It.IsAny<string>(), It.IsAny<IDictionary<string, int>>()))
                .Returns(new Dictionary<string, int> {
                    { "This", 1 },
                    { "is", 1 },
                    { "a", 1 },
                    { "sample", 2 },
                    { "sentence", 2 },
                    { "for", 1 },
                    // above dictionary is result of CountWordFrequenciesFromChunk method
                    // below word is last remaining word from the last chunk
                    { "testing", 1 }
                });

            const string expectedContent = "sample,2\r\nsentence,2\r\na,1\r\nfor,1\r\nis,1\r\ntesting,1\r\nThis,1";
            string capturedFilePath = null;
            string capturedContent = null;

            fileProviderMock.Setup(fp => fp.WriteFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((path, content) =>
                {
                    capturedFilePath = path;
                    capturedContent = content;
                })
                .Returns(Task.CompletedTask);

            #endregion

            // Act
            await wordFrequencyProcessor.ProcessFileAsync(inputFilePath, outputFilePath);

            // Verify
            fileProviderMock.Verify(fp => fp.WriteFileAsync(outputFilePath, It.IsAny<string>()), Times.Once);

            // Assert
            Assert.Equal(expectedContent, capturedContent);
            Assert.Equal(outputFilePath, capturedFilePath);
        }
    }
}

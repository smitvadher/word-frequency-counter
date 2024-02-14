using System.Collections.Concurrent;

namespace Counter.Core.Tests
{
    public class ParallelWordCounterTests
    {
        #region Tests

        [Fact]
        public void CountWordFrequencies_NullContent_NullDictionary_ReturnsEmptyDictionary()
        {
            // Arrange
            var wordCounter = new ParallelWordCounter();

            // Act
            var result = wordCounter.CountWordFrequencies(null, null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void CountWordFrequencies_NotConcurrentDictionary_ThrowsException()
        {
            // Arrange
            var wordCounter = new ParallelWordCounter();
            var content = "test";
            var expectedErrorMessage = "Concurrent dictionary is required to calculate word frequencies parallel.";

            // Act and Assert
            var ex = Assert.Throws<Exception>(() => wordCounter.CountWordFrequencies(content, new Dictionary<string, int>()));
            Assert.Equal(expectedErrorMessage, ex.Message);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void CountWordFrequencies_ValidContent_ReturnsWordFrequencies(string content, IDictionary<string, int> expectedResult)
        {
            // Arrange
            var parallelWordCounter = new ParallelWordCounter();

            // Act
            var result = parallelWordCounter.CountWordFrequencies(content, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Count, result.Count);
            Assert.True(expectedResult.All(kv => result.ContainsKey(kv.Key) && result[kv.Key] == kv.Value));
        }


        [Fact]
        public void CountWordFrequenciesFromChunk_NullChunk_NullDictionary_ReturnsEmptyDictionary()
        {
            // Arrange
            var wordCounter = new ParallelWordCounter();

            // Act
            var result = wordCounter.CountWordFrequenciesFromChunk(null, null, "", out _);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void CountWordFrequenciesFromChunk_NullChunk_WithDictionary_ReturnsSameDictionary()
        {
            // Arrange
            var wordCounter = new ParallelWordCounter();
            var fd = new ConcurrentDictionary<string, int>(
                new Dictionary<string, int>
                {
                    { "The", 2 },
                });

            // Act
            var result = wordCounter.CountWordFrequenciesFromChunk(null, fd, "", out _);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fd, result);
        }

        [Fact]
        public void CountWordFrequenciesFromChunk_NotConcurrentDictionary_ThrowsException()
        {
            // Arrange
            var wordCounter = new ParallelWordCounter();
            var content = "test";
            var expectedErrorMessage = "Concurrent dictionary is required to calculate word frequencies parallel.";

            // Act and Assert
            var ex = Assert.Throws<Exception>(() => wordCounter.CountWordFrequenciesFromChunk(content, new Dictionary<string, int>(), "", out _));
            Assert.Equal(expectedErrorMessage, ex.Message);
        }

        [Theory]
        [MemberData(nameof(TestChunkData))]
        public void CountWordFrequenciesFromChunk_ValidData_ReturnsWordFrequenciesAndRemainingWord(string chunk,
            string previousRemainingWord,
            IDictionary<string, int> frequencyDictionary,
            string expectedRemainingWord,
            IDictionary<string, int> expectedFrequencyDictionary)
        {
            // Arrange
            var wordCounter = new ParallelWordCounter();

            // Act
            var result = wordCounter.CountWordFrequenciesFromChunk(chunk,
                new ConcurrentDictionary<string, int>(frequencyDictionary),
                previousRemainingWord,
                out var remainingWord);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedRemainingWord, remainingWord);
            Assert.True(expectedFrequencyDictionary.All(kv => result.ContainsKey(kv.Key) && result[kv.Key] == kv.Value));
        }

        #endregion

        #region Test data

        public static IEnumerable<object[]> TestData =>
            new List<object[]>
            {
                new object[]
                {
                    "This is a sample sentence. Sample sentence for testing.",
                    new Dictionary<string, int>
                    {
                        { "This", 1 },
                        { "is", 1 },
                        { "a", 1 },
                        { "sample", 2 },
                        { "sentence", 2 },
                        { "for", 1 },
                        { "testing", 1 }
                    }
                },
                new object[]
                {
                    "\\test\\, test, \"test\", test?, test. test ([{!?:;'&_\n}]) \t test$, 123, 123, 456",
                    new Dictionary<string, int>
                    {
                        { "test", 6 },
                        { "test$", 1 },
                        { "123", 2 },
                        { "456", 1 },
                    }
                },
            };

        public static IEnumerable<object[]> TestChunkData =>
            new List<object[]>
            {
                new object[]
                {
                    "The quick brown fox",          // chunk
                    "",                             // previous remaining word
                    new Dictionary<string, int>(),  // frequency dictionary
                    "fox",                          // expected remaining word
                    new Dictionary<string, int>     // expected result frequency dictionary
                    {
                        { "The", 1 },
                        { "quick", 1 },
                        { "brown", 1 },
                    },
                },
                new object[]
                {
                    // below "fox." is remaining word
                    // so it wont coming in output count
                    "e quick brown fox.",
                    "Th",
                    new Dictionary<string, int>
                    {
                        { "The", 1 },
                        { "brown", 1 },
                    },
                    "fox.",
                    new Dictionary<string, int>
                    {
                        { "The", 2 },
                        { "quick", 1 },
                        { "brown", 2 }
                    },
                },
            };

        #endregion
    }
}

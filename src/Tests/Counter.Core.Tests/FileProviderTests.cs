using System.Text;

namespace Counter.Core.Tests
{
    public class FileProviderTests
    {
        [Fact]
        public async Task ReadFileAsync_InvalidFileName_ThrowsFileNotFoundException()
        {
            // Arrange
            var fileProvider = new FileProvider();
            var filePath = "TestFiles\\InvalidInputFile1.txt";

            // Act and Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => fileProvider.ReadFileAsync(filePath));
        }

        [Fact]
        public async Task ReadFileAsync_InvalidPath_DirectoryNotFoundException()
        {
            // Arrange
            var fileProvider = new FileProvider();
            var filePath = "TestFiles\\InvalidPath\\InputFile1.txt";

            // Act and Assert
            await Assert.ThrowsAsync<DirectoryNotFoundException>(() => fileProvider.ReadFileAsync(filePath));
        }

        [Fact]
        public async Task ReadFileAsync_ValidPath_ReturnsFileContent()
        {
            // Arrange
            var fileProvider = new FileProvider();
            var filePath = "TestFiles\\InputFile1.txt";
            var expected = await File.ReadAllTextAsync(filePath);

            // Act
            var result = await fileProvider.ReadFileAsync(filePath);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ReadFileChunksAsync_InvalidFileName_ThrowsFileNotFoundException()
        {
            // Arrange
            var fileProvider = new FileProvider();
            var filePath = "TestFiles\\InvalidInputFile1.txt";

            // Act and Assert
            await Assert.ThrowsAsync<FileNotFoundException>(async () => new List<string>(await fileProvider.ReadFileChunksAsync(filePath).ToListAsync()));
        }

        [Fact]
        public async Task ReadFileChunksAsync_InvalidPath_DirectoryNotFoundException()
        {
            // Arrange
            var fileProvider = new FileProvider();
            var filePath = "TestFiles\\InvalidPath\\InputFile1.txt";

            // Act and Assert
            await Assert.ThrowsAsync<DirectoryNotFoundException>(async () => new List<string>(await fileProvider.ReadFileChunksAsync(filePath).ToListAsync()));
        }

        [Fact]
        public async Task ReadFileChunksAsync_WithValidFilePath_ReturnsCorrectNumberOfChunks()
        {
            // Arrange
            var fileProvider = new FileProvider();
            var filePath = "TestFiles\\InputFile1.txt";
            var bufferSize = 1;
            var bufferIterations = 3;
            var expectedChunkSize = bufferSize * bufferIterations;

            // Act
            var chunks = await fileProvider.ReadFileChunksAsync(filePath, bufferSize, bufferIterations).ToListAsync();

            // Assert
            // Last chunk might have 
            Assert.True(chunks[..^1].All(c => Encoding.UTF8.GetBytes(c).Length == expectedChunkSize));
        }

        [Fact]
        public async Task ReadFileLines_InvalidFileName_ThrowsFileNotFoundException()
        {
            // Arrange
            var fileProvider = new FileProvider();
            var filePath = "TestFiles\\InvalidInputFile1.txt";

            // Act and Assert
            await Assert.ThrowsAsync<FileNotFoundException>(async () => new List<string>(await fileProvider.ReadFileLinesAsync(filePath).ToListAsync()));
        }

        [Fact]
        public async Task ReadFileLines_InvalidPath_DirectoryNotFoundException()
        {
            // Arrange
            var fileProvider = new FileProvider();
            var filePath = "TestFiles\\InvalidPath\\InputFile1.txt";

            // Act and Assert
            await Assert.ThrowsAsync<DirectoryNotFoundException>(async () => new List<string>(await fileProvider.ReadFileLinesAsync(filePath).ToListAsync()));
        }

        [Fact]
        public async Task ReadFileLines_ValidPath_ReturnsFileContentAsync()
        {
            // Arrange
            var fileProvider = new FileProvider();
            var filePath = "TestFiles\\InputFile1.txt";
            var expected = File.ReadAllText(filePath);

            // Act
            var result = string.Join(Environment.NewLine, await fileProvider.ReadFileLinesAsync(filePath).ToListAsync());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteFileAsync_InvalidPath_DirectoryNotFoundException()
        {
            // Arrange
            var fileProvider = new FileProvider();
            var filePath = "TestFiles\\InvalidPath\\OutputFile.txt";

            // Act and Assert
            await Assert.ThrowsAsync<DirectoryNotFoundException>(() => fileProvider.WriteFileAsync(filePath, "Test content"));
        }

        [Fact]
        public async Task WriteFileAsync_ValidPath_WritesToFile()
        {
            // Arrange
            var fileProvider = new FileProvider();
            var filePath = "TestFiles\\OutputFile.txt";
            var expected = "Test content";

            // Act
            await fileProvider.WriteFileAsync(filePath, expected);

            // Assert
            Assert.True(File.Exists(filePath));
            Assert.Equal(expected, await File.ReadAllTextAsync(filePath));

            // Clean up
            File.Delete(filePath);
        }
    }
}
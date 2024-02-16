using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Counter.Core
{
    ///<summary>
    /// Provides IO functionality.
    ///</summary>
    public class FileProvider : IFileProvider
    {
        ///<summary>
        /// Asynchronously reads the content of a file.
        ///</summary>
        ///<param name="filePath">The path of the file to read.</param>
        ///<returns>The content of the file as a string.</returns>
        public async Task<string> ReadFileAsync(string filePath)
        {
            const int bufferSize = 4096;
            var stringBuilder = new StringBuilder();

            await using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.Asynchronous))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    var buffer = new char[bufferSize];
                    int bytesRead;

                    while ((bytesRead = await streamReader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        stringBuilder.Append(buffer, 0, bytesRead);
                    }
                }
            }

            return stringBuilder.ToString();
        }

        ///<summary>
        /// Asynchronously reads the content of a file in chunks.
        ///</summary>
        ///<param name="filePath">The path of the file to read.</param>
        ///<param name="bufferSize">The size of the buffer used for reading (default is 4096).</param>
        ///<param name="bufferIterations">The number of times to read the file with the same buffer size before yielding a chunk (default is 1).</param>
        ///<returns>An asynchronous enumerable of file content chunks as strings.</returns>
        public async IAsyncEnumerable<string> ReadFileChunksAsync(string filePath, int bufferSize = 4096, int bufferIterations = 1)
        {
            await using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                             bufferSize, FileOptions.Asynchronous))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    var stringBuilder = new StringBuilder();
                    var chunkCount = 0;
                    var buffer = new char[bufferSize];

                    int bytesRead;
                    while ((bytesRead = await streamReader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        stringBuilder.Append(buffer, 0, bytesRead);
                        chunkCount++;

                        if (chunkCount != bufferIterations) continue;

                        chunkCount = 0;
                        var result = stringBuilder.ToString();
                        stringBuilder.Clear();
                        yield return result;
                    }

                    yield return stringBuilder.ToString();
                }
            }
        }

        ///<summary>
        /// Asynchronously reads the lines from a file.
        ///</summary>
        ///<param name="filePath">The path of the file to read.</param>
        ///<returns>An IEnumerable of strings representing the lines of the file.</returns>
        public async IAsyncEnumerable<string> ReadFileLinesAsync(string filePath)
        {
            const int bufferSize = 4096;
            await using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.Asynchronous))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    while (!streamReader.EndOfStream)
                    {
                        var line = await streamReader.ReadLineAsync();
                        if (line != null)
                            yield return line;
                    }
                }
            }
        }

        ///<summary>
        /// Asynchronously writes content to a file.
        ///</summary>
        ///<param name="filePath">The path of the file to write to.</param>
        ///<param name="content">The content to write to the file.</param>
        public async Task WriteFileAsync(string filePath, string content)
        {
            const int bufferSize = 4096;
            await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None,
                             bufferSize, FileOptions.Asynchronous))
            {
                await using (var streamWriter = new StreamWriter(fileStream))
                {
                    var index = 0;
                    var length = content.Length;

                    while (index < length)
                    {
                        var remaining = length - index;
                        var chunkSize = Math.Min(remaining, bufferSize);

                        var buffer = content.ToCharArray(index, chunkSize);
                        await streamWriter.WriteAsync(buffer, 0, buffer.Length);

                        index += chunkSize;
                    }
                }
            }
        }
    }
}

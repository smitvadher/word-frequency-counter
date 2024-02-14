using System.Collections.Generic;
using System.Threading.Tasks;

namespace Counter.Core
{
    ///<summary>
    /// Represents an interface for file related operations.
    ///</summary>
    public interface IFileProvider
    {
        ///<summary>
        /// Asynchronously reads the content of a file.
        ///</summary>
        ///<param name="filePath">The path of the file to read.</param>
        ///<returns>The content of the file as a string.</returns>
        Task<string> ReadFileAsync(string filePath);

        ///<summary>
        /// Asynchronously reads the content of a file in chunks.
        ///</summary>
        ///<param name="filePath">The path of the file to read.</param>
        ///<param name="bufferSize">The size of the buffer used for reading (default is 4096).</param>
        ///<param name="bufferIterations">The number of times to read the file with the same buffer size before yielding a chunk (default is 1).</param>
        ///<returns>An asynchronous enumerable of file content chunks as strings.</returns>
        IAsyncEnumerable<string> ReadFileChunksAsync(string filePath, int bufferSize = 4096, int bufferIterations = 1);

        ///<summary>
        /// Asynchronously reads the lines from a file.
        ///</summary>
        ///<param name="filePath">The path of the file to read.</param>
        ///<returns>An IEnumerable of strings representing the lines of the file.</returns>
        IAsyncEnumerable<string> ReadFileLinesAsync(string filePath);

        ///<summary>
        /// Asynchronously writes content to a file.
        ///</summary>
        ///<param name="filePath">The path of the file to write to.</param>
        ///<param name="content">The content to write to the file.</param>
        Task WriteFileAsync(string filePath, string content);
    }
}

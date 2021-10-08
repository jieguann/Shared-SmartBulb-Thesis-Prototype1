using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Piglet
{
    /// <summary>
    /// Utility methods for reading/writing streams.
    /// </summary>
    public static class StreamUtil
    {
        /// <summary>
        /// If an operation in an IEnumerator method takes longer
        /// than this, yield after the operation is complete.
        /// </summary>
        public const int MILLISECONDS_PER_YIELD = 10;

        /// <summary>
        /// Number of bytes per read/write operation.
        /// </summary>
        public const int BLOCK_SIZE = 4096;

        /// <summary>
        /// Copy the bytes in the source stream to the dest stream.
        /// </summary>
        static public IEnumerator CopyStreamEnum(Stream source, Stream dest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            byte[] buffer = new byte[BLOCK_SIZE];
            int bytesRead;

            while ((bytesRead = source.Read(
                       buffer, 0, BLOCK_SIZE)) > 0)
            {
                if (stopwatch.ElapsedMilliseconds
                    > MILLISECONDS_PER_YIELD)
                {
                    yield return null;
                    stopwatch.Restart();
                }

                dest.Write(buffer, 0, bytesRead);

                if (stopwatch.ElapsedMilliseconds
                    > MILLISECONDS_PER_YIELD)
                {
                    yield return null;
                    stopwatch.Restart();
                }
            }
        }

        /// <summary>
        /// Return an IEnumerable&lt;byte&gt; for the data of a Stream
        /// (e.g. a FileStream created by File.OpenRead(path)).
        /// </summary>
        /// <param name="source">
        /// The input Stream.
        /// </param>
        /// <param name="blockSize">
        /// Number of bytes the method attempts to read per I/O
        /// operation. This only affects the performance of the
        /// method, not the values returned by the
        /// IEnumerable&lt;byte&gt;.
        /// </param>
        /// <returns>
        /// An IEnumerable&lt;byte&gt; over the data from the stream.
        /// </returns>
        static public IEnumerable<byte> Read(Stream source, int blockSize=BLOCK_SIZE)
        {
            var buffer = new byte[blockSize];
            int bytesRead;

            while ((bytesRead = source.Read(buffer, 0, blockSize)) > 0)
            {
                for (int i = 0; i < bytesRead; ++i)
                    yield return buffer[i];
            }
        }
    }
}
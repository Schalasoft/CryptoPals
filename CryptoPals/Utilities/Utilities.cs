﻿using System.IO;

namespace CryptoPals
{
    /// <summary>
    /// A utility class used to convert bytes
    /// </summary>
    class ByteConverter
    {
        /// <summary>
        /// Consume and return bytes from a memory stream
        /// </summary>
        /// <param name="stream">The memory stream to consume bytes from</param>
        /// <param name="length">The number of bytes to consume</param>
        /// <returns>A byte array containing the number of bytes specified from the current position of the memory stream</returns>
        public static byte[] GetBytes(MemoryStream stream, int length)
        {
            // Consume bytes from the memory stream
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);

            return bytes;
        }
    }
}


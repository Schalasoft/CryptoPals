namespace CryptoPals.Interfaces
{
    /// <summary>
    /// ECB/CBC Detection Oracle
    /// </summary>
    interface IChallenge11 : IChallenge
    {
        /// <summary>
        /// Generates a random key of the specified size
        /// </summary>
        /// <param name="length">The size of the byte array to create</param>
        /// <returns>A byte array of the specified size containing random bytes in the range 0-256</returns>
        public byte[] GenerateRandomASCIIBytes(int length);

        /// <summary>
        /// Create a byte array with the bytes to insert put before or after the original set of bytes
        /// </summary>
        /// <param name="bytesOriginal">The original bytes</param>
        /// <param name="bytesToInsert">The bytes to insert</param>
        /// <param name="insertBefore">Whether to insert the bytes before, or after, the original bytes</param>
        /// <returns>A byte array with the bytes to insert added before/after the original bytes</returns>
        public byte[] InsertBytes(byte[] bytesOriginal, byte[] bytesToInsert, bool insertBefore);
    }
}

﻿namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Detect AES in ECB mode
    /// </summary>
    interface IChallenge8 : IChallenge
    {
        /// <summary>
        /// Detects if the bytes have been ECB encrypted
        /// </summary>
        /// <param name="bytes">The bytes to check</param>
        /// <returns>True if the bytes have been ECB encrypted</returns>
        public bool IsECBEncrypted(byte[] bytes);

        /// <summary>
        /// Get the amount of blocks that have been repeated in the provided bytes, based on the provided block size
        /// </summary>
        /// <param name="bytes">The bytes to get the repeated block count of</param>
        /// <param name="blockSize">The size of the blocks to check</param>
        /// <returns>An integer representing the repeated block count for the input bytes</returns>
        public int GetRepeatedBlockCount(byte[] bytes, int blockSize = 0);

        /// <summary>
        /// Get the repeated block count for each line
        /// </summary>
        /// <param name="lines">The lines to get repeated counts of</param>
        /// <param name="blockSize">The size of the block to use</param>
        /// <returns>An int array containing the repeated block count for each line</returns>
        public int[] GetRepeatedBlockCounts(byte[] bytes, int blockSize);
    }
}
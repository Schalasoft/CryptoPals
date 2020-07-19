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
        /// <param name="blockSize">The size of the blocks to use for detection</param>
        /// <returns>True if the bytes have been ECB encrypted</returns>
        public bool IsECBEncrypted(byte[] bytes, int blockSize);
    }
}

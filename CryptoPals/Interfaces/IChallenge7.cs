namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Encrypt/Decrypt AES ECB (that has been Base64-encoded)
    /// </summary>
    interface IChallenge7 : IChallenge
    {
        /// <summary>
        /// Encrypt/Decrypt bytes using AES in ECB mode
        /// </summary>
        /// <param name="encrypt">Whether we are encrypting or decrypting</param>
        /// <param name="bytes">The bytes to encrypt/decrypt</param>
        /// <param name="key">The key to use for encrypting/decrypting</param>
        /// <returns>The encrypted/decrypted data</returns>
        public byte[] AES_ECB(bool encrypt, byte[] bytes, byte[] key);
    }
}

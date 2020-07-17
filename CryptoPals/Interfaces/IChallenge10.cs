namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Encrypt/Decrypt using AES CBC (Advanced Encryption Standard Cipher Block Chaining Mode)
    /// </summary>
    interface IChallenge10 : IChallenge
    {
        /// <summary>
        /// Encrypt/Decrypt using AES CBC
        /// </summary>
        /// <param name="encrypt">Whether to encrypt or decrypt</param>
        /// <param name="bytes">The bytes to encrypt/decrypt</param>
        /// <param name="key">The key used to encrypt/decrypt</param>
        /// <param name="iv">The initialization vector</param>
        /// <returns></returns>
        public byte[] AES_CBC(bool encrypt, byte[] bytes, byte[] key, byte[] iv);
    }
}

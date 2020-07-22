namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Repeating-key XOR
    /// </summary>
    interface IChallenge5 : IChallenge
    {
        /// <summary>
        /// Sequentially XOR each byte in the key against each byte in the bytes
        /// </summary>
        /// <param name="bytes">The bytes to XOR</param>
        /// <param name="key">The key to XOR</param>
        /// <returns>The result of XORing the bytes with the key bytes</returns>
        public byte[] RepeatingKeyXOR(byte[] bytes, byte[] key);
    }
}

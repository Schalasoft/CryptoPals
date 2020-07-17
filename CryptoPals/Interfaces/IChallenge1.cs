namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Convert a Hex string to Base64
    /// </summary>
    interface IChallenge1 : IChallenge
    {
        /// <summary>
        /// Convert Hex bytes to a string (no dashes)
        /// </summary>
        /// <param name="bytes">The byte representation of hex bytes</param>
        /// <returns>Hex string</returns>
        public string HexBytesToString(byte[] bytes);

        /// <summary>
        /// Converts Hex string to its byte representation
        /// </summary>
        /// <param name="text">The hex string to convert to bytes</param>
        /// <returns>The byte representation of the hex string</returns>
        public byte[] HexStringToBytes(string text);
    }
}

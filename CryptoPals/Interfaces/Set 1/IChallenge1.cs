namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Convert a Hex string to Base64
    /// </summary>
    public interface IChallenge1 : IChallenge
    {
        /// <summary>
        /// Convert Hex bytes to a string (no dashes)
        /// </summary>
        /// <param name="bytes">The byte representation of hex bytes</param>
        /// <returns>The hex string representation of the bytes</returns>
        public string HexBytesToString(byte[] bytes);

        /// <summary>
        /// Converts Hex string to its byte representation
        /// </summary>
        /// <param name="text">The hex string to convert to bytes</param>
        /// <returns>The byte representation of the hex string</returns>
        public byte[] HexStringToBytes(string text);

        /// <summary>
        /// Convert a hex string to Base64
        /// </summary>
        /// <param name="hex">The hex string</param>
        /// <returns>The Base64 representation of the input hex string</returns>
        public string HexStringToBase64(string hex);
    }
}

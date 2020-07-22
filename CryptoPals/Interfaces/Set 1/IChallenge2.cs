namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Fixed XOR
    /// </summary>
    interface IChallenge2 : IChallenge
    {
        /// <summary>
        /// XOR two bytes
        /// </summary>
        /// <param name="a">The first byte</param>
        /// <param name="b">The second byte</param>
        /// <returns>The byte resulting from XORing 'a' and 'b'</returns>
        public byte XORByte(byte a, byte b);

        /// <summary>
        /// XOR two equal length byte arrays
        /// </summary>
        /// <param name="a">The first byte array</param>
        /// <param name="b">The second byte array</param>
        /// <returns>The byte array resulting from XORing 'a' and 'b'</returns>
        public byte[] XORByteArray(byte[] a, byte[] b);
    }
}

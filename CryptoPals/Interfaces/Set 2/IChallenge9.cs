namespace CryptoPals.Interfaces
{
    /// <summary>
    /// PKCS#7 padding
    /// </summary>
    interface IChallenge9 : IChallenge
    {
        /// <summary>
        /// Pad input bytes to the specified number of bytes
        /// </summary>
        /// <param name="bytes">The bytes to pad, if none provided this method doubles as a way to construct a byte array filled with X</param>
        /// <param name="size">The length to pad the bytes up to</param>
        /// <param name="paddingByte">The byte to use as the padding, defaults to 0x04</param>
        /// <returns>The padded bytes</returns>
        public byte[] PadBytes(byte[] bytes, int size, byte paddingByte = (byte)0x04);
    }
}

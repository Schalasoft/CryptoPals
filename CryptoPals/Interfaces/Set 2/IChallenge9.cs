namespace CryptoPals.Interfaces
{
    /// <summary>
    /// PKCS#7 padding
    /// </summary>
    interface IChallenge9 : IChallenge
    {
        /// <summary>
        /// Pad input bytes to the specified number of bytes, if no bytes provided simply return identical bytes of the specified length using the padding byte
        /// </summary>
        /// <param name="bytes">The bytes to pad, if none provided this method doubles as a way to construct a byte array filled with X</param>
        /// <param name="size">The length to pad the bytes up to</param>
        /// <param name="paddingByte">The byte to use as the padding, defaults to 0x04</param>
        /// <returns>The padded bytes</returns>
        public byte[] PadBytes(int size, byte[] bytes = null, byte paddingByte = (byte)0x04);

        /// <summary>
        /// Pads input bytes up to a multiple of the block size (if we have 14 bytes and we want it padded to 16, we will get a block of the original 14 bytes, padded with 2 bytes set to the padding byte)
        /// </summary>
        /// <param name="bytes">The bytes to pad</param>
        /// <param name="blockSize">The size of an individual block</param>
        /// <param name="paddingByte">The byte to use to add padding</param>
        /// <returns>The bytes provided padded with the specified padding byte added until we have a multiple of the block size</returns>
        public byte[] PadBytesToBlockSizeMultiple(byte[] bytes, int blockSize, byte paddingByte = (byte)0x04);
    }
}

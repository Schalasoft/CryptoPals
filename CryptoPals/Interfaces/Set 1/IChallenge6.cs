namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Break repeating-key XOR
    /// </summary>
    public interface IChallenge6 : IChallenge
    {
        /// <summary>
        /// Break bytes into blocks of the specified size
        /// </summary>
        /// <param name="bytes">The bytes to break into blocks</param>
        /// <param name="size">The size of the blocks to break the bytes into</param>
        /// <returns>The bytes brocken into blocks of the specified size</returns>
        public byte[][] CreateBlocks(byte[] bytes, int size);
    }
}

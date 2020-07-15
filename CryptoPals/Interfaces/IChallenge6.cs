namespace CryptoPals.Interfaces
{
    interface IChallenge6 : IChallenge
    {
        public byte[][] CreateBlocks(byte[] bytes, int size);
    }
}

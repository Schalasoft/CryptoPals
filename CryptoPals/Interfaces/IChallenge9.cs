namespace CryptoPals.Interfaces
{
    interface IChallenge9 : IChallenge
    {
        public byte[] PadBlock(byte[] bytes, int length);
    }
}

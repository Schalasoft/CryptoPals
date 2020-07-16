namespace CryptoPals.Interfaces
{
    interface IChallenge9 : IChallenge
    {
        public byte[] PadBytes(byte[] bytes, int size, byte paddingByte = (byte)0x04);
    }
}

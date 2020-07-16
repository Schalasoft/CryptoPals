namespace CryptoPals.Interfaces
{
    interface IChallenge2 : IChallenge
    {
        public byte XORByte(byte a, byte b);
        public byte[] XORByteArray(byte[] a, byte[] b);
    }
}

namespace CryptoPals.Interfaces
{
    interface IChallenge7 : IChallenge
    {
        public byte[] Decrypt(byte[] data, byte[] key);
    }
}

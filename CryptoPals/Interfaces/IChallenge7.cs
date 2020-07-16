namespace CryptoPals.Interfaces
{
    interface IChallenge7 : IChallenge
    {
        public byte[] AES_ECB(bool encrypt, byte[] data, byte[] key);
    }
}

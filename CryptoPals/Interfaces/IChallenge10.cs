namespace CryptoPals.Interfaces
{
    interface IChallenge10 : IChallenge
    {
        public byte[] AES_CBC(bool encrypt, byte[] bytes, byte[] key, byte[] iv);
    }
}

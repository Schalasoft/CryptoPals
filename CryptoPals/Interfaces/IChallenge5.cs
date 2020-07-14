namespace CryptoPals.Interfaces
{
    interface IChallenge5 : IChallenge
    {
        public byte[] RepeatingKeyXOR(byte[] bytes, byte[] key);
    }
}

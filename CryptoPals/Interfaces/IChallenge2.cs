namespace CryptoPals.Interfaces
{
    interface IChallenge2 : IChallenge
    {
        public byte XOR(byte a, byte b);
    }
}

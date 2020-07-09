namespace CryptoPals.Interfaces
{
    interface IChallenge2 : IChallenge
    {
        public string HexBytesToString(byte[] bytes);
        public byte[] HexStringToBytes(string hex);
        public byte XOR(byte a, byte b);
    }
}

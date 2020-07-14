namespace CryptoPals.Interfaces
{
    interface IChallenge1 : IChallenge
    {
        public string HexBytesToString(byte[] bytes);
        public byte[] HexStringToBytes(string text);
    }
}

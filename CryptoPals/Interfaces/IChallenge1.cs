namespace CryptoPals.Interfaces
{
    interface IChallenge1 : IChallenge
    {
        public byte[] HexStringToBytes(string text);
    }
}

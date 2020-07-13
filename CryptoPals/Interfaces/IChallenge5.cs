namespace CryptoPals.Interfaces
{
    interface IChallenge5 : IChallenge
    {
        public string RepeatingKeyXOR(string text, string key, bool hex = false);
    }
}

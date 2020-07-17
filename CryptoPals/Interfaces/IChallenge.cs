namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Base Interface for a Challenge
    /// </summary>
    interface IChallenge
    {
        /// <summary>
        /// Solve the challenge
        /// </summary>
        /// <param name="input">The input/problem string</param>
        /// <returns>The solution string</returns>
        public string Solve(string input);
    }
}

namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Detect single-character XOR
    /// </summary>
    public interface IChallenge4 : IChallenge
    {
        /// <summary>
        /// Splits a given input string by a separator and returns a string array containing the lines created
        /// </summary>
        /// <param name="text">The text containing the lines to split</param>
        /// <param name="separator">Optional parameter: the separator to use, defaults to the separator defined in the Constants class</param>
        /// <returns>A string array containing the lines from the input text</returns>
        public string[] SplitTextIntoLines(string text, string separator);
    }
}

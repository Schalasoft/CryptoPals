using System;
using System.Collections.Generic;

namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Find single character key used to XOR text
    /// </summary>
    public interface IChallenge3 : IChallenge
    {
        /// <summary>
        /// XOR decrypt bytes against each ASCII character and return a KVP containing the key, score, and resutling decoded text
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>The max scoring Key Value Pair (the most likely character used to XOR encrypt)</returns>
        public KeyValuePair<int, Tuple<double, string>> SingleKeyXORBruteForce(byte[] bytes);

        /// <summary>
        /// Format a Key Value Pair for output so we can see the output, key, and score, as well as any additional information
        /// appended to the end
        /// </summary>
        /// <param name="kvp">The Key Value Pair containing the key, score, and decoded output text</param>
        /// <param name="additionalInformation">Any additional information to append to the end</param>
        /// <returns>A string containing the key, score and decoded output, all formatted together</returns>
        public string FormatOutput(KeyValuePair<int, Tuple<double, string>> kvp, string additionalInformation = "");
    }
}

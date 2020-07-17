using System;
using System.Collections.Generic;

namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Find single character key used to XOR text
    /// </summary>
    interface IChallenge3 : IChallenge
    {
        /// <summary>
        /// XOR decrypt bytes against each ASCII character and return a KVP containing the key, score, and resutling decoded text
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>The max scoring Key Value Pair (the most likely character used to XOR encrypt)</returns>
        public KeyValuePair<int, Tuple<double, string>> SingleKeyXORBruteForce(byte[] bytes);

        /// <summary>
        /// Format a Key Value Pair for output so we can see the output, key, and score
        /// </summary>
        /// <param name="kvp">The Key Value Pair containing the key, score, and output text</param>
        /// <param name="additionalInformation">Any additional information to append to the end</param>
        /// <returns>A string containing the output, key, and score formatted together</returns>
        public string FormatOutput(KeyValuePair<int, Tuple<double, string>> kvp, string additionalInformation = "");
    }
}

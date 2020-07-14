using System;
using System.Collections.Generic;

namespace CryptoPals.Interfaces
{
    interface IChallenge3 : IChallenge
    {
        public KeyValuePair<int, Tuple<double, string>> SingleKeyXORBruteForce(byte[] bytes);
        public string FormatOutput(KeyValuePair<int, Tuple<double, string>> kvp, string additionalInformation = "");
    }
}

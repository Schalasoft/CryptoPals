using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoPals.Sets
{
    class Challenge3 : IChallenge3, IChallenge
    {
        /*
        The hex encoded string:
        1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736
        ... has been XOR'd against a single character. 
        Find the key, decrypt the message.
        You can do this by hand. But don't: write code to do it for you.
        How? Devise some method for "scoring" a piece of English plaintext. 
        Character frequency is a good metric. 
        Evaluate each output and choose the one with the best score.
        */

        // Reuse previous challenge functionality
        IChallenge1 challenge1 = (IChallenge1)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge1);
        IChallenge2 challenge2 = (IChallenge2)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge2);

        // Letter Frequency values taken from 
        // https://en.wikipedia.org/wiki/Letter_frequency
        // https://en.wikipedia.org/wiki/English_punctuation#Frequency
        // http://www.macfreek.nl/memory/Letter_Distribution
        private Dictionary<char, float> letterFrequencyTable = new Dictionary<char, float>
        {
            {'\"',.0267f},
            {'\'',.0243f},
            {'.',.06530f},
            {',',.06130f},
            {'-',.01530f},
            {'?',.05600f},
            {':',.03400f},
            {';',.03400f},
            {'!',.03300f},
            {' ',.13000f},
            {'a',.08167f},
            {'b',.01492f},
            {'c',.02782f},
            {'d',.04253f},
            {'e',.12702f},
            {'f',.02228f},
            {'g',.02015f},
            {'h',.06094f},
            {'i',.06094f},
            {'j',.00153f},
            {'k',.00772f},
            {'l',.04025f},
            {'m',.02406f},
            {'n',.06749f},
            {'o',.07507f},
            {'p',.01929f},
            {'q',.00095f},
            {'r',.05987f},
            {'s',.06327f},
            {'t',.09056f},
            {'u',.02758f},
            {'v',.00978f},
            {'w',.02360f},
            {'x',.00150f},
            {'y',.01974f},
            {'z',.00074f}
        };

        // Solve
        public string Solve(string input)
        {
            // Input is hex so convert hex string to bytes
            byte[] bytes = challenge1.HexStringToBytes(input);

            // Get likely XOR key
            KeyValuePair<int, Tuple<double, string>> maxScoringItem = SingleKeyXORBruteForce(bytes);

            // Format output
            string output = FormatOutput(maxScoringItem);

            // Return combined output so we can see output, and key, and score
            return output;
        }

        // Decode text against a single key and score it
        public KeyValuePair<int, Tuple<double, string>> DecodeAndScore(int index, byte[] bytes)
        {
            // Get byte representation of key (0-255)
            byte key = (byte)index;

            // XOR each byte of input text against the key byte
            byte[] decodedBytes = new byte[bytes.Length];
            for (int j = 0; j < bytes.Length; j++)
            {
                decodedBytes[j] = challenge2.XORByte(bytes[j], key);
            }

            // Convert to string, lowercasing each character
            string decoded = Encoding.ASCII.GetString(decodedBytes);

            // Calculate score using the letter frequency table, use lowercase of input string as table is lowercase
            string decodedLowercase = decoded.ToLower();
            double score = GetScore(decodedLowercase);

            // Return KVP containing the key (index), score, and decoded text
            return new KeyValuePair<int, Tuple<double, string>>(index, new Tuple<double, string>(score, decoded));
        }

        // Format a Key Value Pair for output so we can see the output, key, and score
        public string FormatOutput(KeyValuePair<int, Tuple<double, string>> kvp, string additionalInformation = "")
        {
            string deducedKey = kvp.Key.ToString();
            string maxScore = kvp.Value.Item1.ToString();
            string output = kvp.Value.Item2.Replace("\r", "").Replace("\n", "");

            return $"{output}{Environment.NewLine}Key    : {deducedKey}{Environment.NewLine}Score  : {maxScore}{additionalInformation}";
        }

        // Given an input string, XOR decrypt it against each ASCII character and return a KVP containing the key, score, and decoded text
        public KeyValuePair<int, Tuple<double, string>> SingleKeyXORBruteForce(byte[] bytes)
        {
            // Decode string with each key (using 0-255 int values as key)
            // Key          : Int/char to use as cypher
            // Tuple Item 1 : Score
            // Tuple Item 2 : Decoded text
            Dictionary<int, Tuple<double, string>> data = new Dictionary<int, Tuple<double, string>>();
            for (int i = 0; i <= 255; i++)
            {
                KeyValuePair<int, Tuple<double, string>> chunk = DecodeAndScore(i, bytes);
                data.Add(chunk.Key, chunk.Value);
            }

            // Get the KVP with the max score
            KeyValuePair<int, Tuple<double, string>> maxScoringItem = data.FirstOrDefault(x => x.Value.Item1 == data.Values.Max(x => x.Item1));

            return maxScoringItem;
        }

        // Get the max score of a given string using the letter frequency table
        private double GetScore(string text)
        {
            double score = 0;
            foreach(char character in text)
            {
                for (int k = 0; k < letterFrequencyTable.Count; k++)
                {
                    score += letterFrequencyTable.FirstOrDefault(x => x.Key.Equals(character)).Value;
                }
            }
            return score;
        }
    }
}
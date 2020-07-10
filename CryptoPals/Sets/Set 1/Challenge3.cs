using CryptoPals.Factories;
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
        IChallenge2 challenge2 = (IChallenge2)ChallengeFactory.InitializeChallenge(Enumerations.ChallengeEnum.Challenge2);

        // Letter Frequency values taken from 
        // Alphabet:        http://pi.math.cornell.edu/~mec/2003-2004/cryptography/subs/frequencies.html
        // Space frequency: http://www.macfreek.nl/memory/Letter_Distribution
        Dictionary<char, float> letterFrequencyTable = new Dictionary<char, float>
        {
            {' ',18.00f},
            {'E',12.02f},
            {'T',9.10f},
            {'A',8.12f},
            {'O',7.68f},
            {'I',7.31f},
            {'N',6.95f},
            {'S',6.28f},
            {'R',6.02f},
            {'H',5.92f},
            {'D',4.32f},
            {'L',3.98f},
            {'U',2.88f},
            {'C',2.71f},
            {'M',2.61f},
            {'F',2.30f},
            {'Y',2.11f},
            {'W',2.09f},
            {'G',2.03f},
            {'P',1.82f},
            {'B',1.49f},
            {'V',1.11f},
            {'X',0.17f},
            {'Q',0.11f},
            {'J',0.10f},
            {'Z',0.07f}
        };


        public string Solve(string input)
        {
            // Get the max scored cypher attempt
            KeyValuePair<int, Tuple<double, string>> maxScoringItem = GetMaxScoringItem(input);

            // Format output
            string deducedKey = maxScoringItem.Key.ToString();
            string maxScore = maxScoringItem.Value.Item1.ToString();
            string output = maxScoringItem.Value.Item2;

            // Return combined output so we can see output, and key, and score
            return $"{output}{Environment.NewLine}Key    : {deducedKey}{Environment.NewLine}Score  : {maxScore}";
        }

        // Given an input string, XOR decrypt it against each ASCII character and return a KVP containing the key, score, and decoded text
        public KeyValuePair<int, Tuple<double, string>> GetMaxScoringItem(string input)
        {
            // Decode string with each key (using 0-255 int values as key)
            // Key          : Int/char to use as cypher
            // Tuple Item 1 : Score
            // Tuple Item 2 : Decoded text
            Dictionary<int, Tuple<double, string>> data = new Dictionary<int, Tuple<double, string>>();
            for (int i = 0; i <= 255; i++)
            {
                KeyValuePair<int, Tuple<double, string>> chunk = DecodeAndScore(i, input);
                data.Add(chunk.Key, chunk.Value);
            }

            // Get the KVP with the max score
            KeyValuePair<int, Tuple<double, string>> maxScoringItem = data.FirstOrDefault(x => x.Value.Item1 == data.Values.Max(x => x.Item1));

            return maxScoringItem;
        }

        // Decode text against a single key and score it
        private KeyValuePair<int, Tuple<double, string>> DecodeAndScore(int index, string text)
        {
            // Get byte representation of key (0-255)
            byte key = (byte)index;

            // Get byte representation of input text
            byte[] bytes = challenge2.HexStringToBytes(text);

            // XOR each byte of input text against the key byte
            byte[] decodedBytes = new byte[bytes.Length];
            for (int j = 0; j < bytes.Length; j++)
            {
                decodedBytes[j] = challenge2.XOR(bytes[j], key);
            }

            // Decode the hex string as ASCII
            string decoded = Encoding.ASCII.GetString(decodedBytes).ToUpper();

            // Calculate score using the letter frequency table
            double score = GetScore(decoded);

            // Return KVP containing the key (index), score, and decoded text
            return new KeyValuePair<int, Tuple<double, string>>(index, new Tuple<double, string>(score, decoded));
        }

        // Get the max score of a given string using the letter frequency table
        private double GetScore(string text)
        {
            double score = 0;
            for (int j = 0; j < text.Length; j++)
            {
                for (int k = 0; k < letterFrequencyTable.Count; k++)
                {
                    score += letterFrequencyTable.FirstOrDefault(x => x.Key == text[j]).Value;
                }
            }
            return score;
        }
    }
}
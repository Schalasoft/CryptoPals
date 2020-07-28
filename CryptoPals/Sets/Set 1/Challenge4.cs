using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using CryptoPals.Managers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CryptoPals.Sets
{
    /// <inheritdoc cref="IChallenge4"/>
    class Challenge4 : IChallenge4, IChallenge
    {
        /*
        Detect single-character XOR
        One of the 60-character strings in this file has been encrypted by single-character XOR.
        Find it.
        (Your code from #3 should help.)
        */

        // Reuse previous challenge functionality
        IChallenge1 challenge1 = (IChallenge1)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge1);
        IChallenge3 challenge3 = (IChallenge3)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge3);

        ///<inheritdoc />
        public string Solve(string input)
        {
            // Get individual lines
            string[] lines = SplitTextIntoLines(input, Environment.NewLine);

            return SolveFast(lines);
            //return SolveSlow(lines);
        }

        /// <summary>
        /// Format a Key Value Pair(key, score, decoded output), with its line number and missing byte count
        /// </summary>
        /// <param name="item">The Key Value Pair containing the ASCII key, score, and decoded output text</param>
        /// <param name="lineNumber">The line number associated with the Key Value Pair</param>
        /// <param name="missingByteCount">The number of missing bytes for the line number</param>
        /// <returns>A formatted string contaning the KVP contents (key, score, decoded output), its line number, and it's missing byte count</returns>
        private string FormatOutput(KeyValuePair<int, Tuple<double, string>> kvp, int lineNumber, int missingByteCount)
        {
            return challenge3.FormatOutput(kvp, $"{Environment.NewLine}Line   : {lineNumber}{Environment.NewLine}Bytes  : {missingByteCount} missing"); ;
        }

        /// <summary>
        /// Go through each line and find the line with the most missing bytes
        /// This is likely to be the XOR encoded line as XOR removes bits
        /// </summary>
        /// <param name="lines">The lines/strings to check against</param>
        /// <returns>A formatted string containing the line with the most bytes missing, including the number of missing bytes</returns>
        private string SolveFast(string[] lines)
        {
            // Iterate lines and find the one with the most missing bytes
            int mostMissingBytes = 0;
            string mostMissingBytesLine = "";
            int mostMissingBytesLineNumber = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                // Find the count of missing bytes
                int missingBytes = GetMissingByteCountForHexString(lines[i]);

                // If this line has more missing bytes than the previous max, update the max and save a reference to the line
                if(missingBytes > mostMissingBytes)
                {
                    mostMissingBytes = missingBytes;
                    mostMissingBytesLine = lines[i];
                    mostMissingBytesLineNumber = i;
                }
            }

            // Get line as bytes
            byte[] bytes = challenge1.HexStringToBytes(mostMissingBytesLine);

            // Get the max scoring cypher for the line with the most missing bytes
            KeyValuePair<int, Tuple<double, string>> maxScoringItem = challenge3.SingleKeyXORBruteForce(bytes);

            // Format the output
            return FormatOutput(maxScoringItem, mostMissingBytesLineNumber, mostMissingBytes);
        }

        /// <summary>
        /// Go through each line, XOR decode and choose the line with the highest score (much slower than SolveFast)
        /// </summary>
        /// <param name="lines">The lines/strings to check against</param>
        /// <returns>A string containing the key, score and decoded output, and line number, all formatted together</returns>
        private string SolveSlow(string[] lines)
        {
            // Decode lines
            double maxLineScore = 0;
            int maxScoringLineNumber = 0;
            KeyValuePair<int, Tuple<double, string>> maxScoringLine = new KeyValuePair<int, Tuple<double, string>>();
            for (int i = 0; i < lines.Length; i++)
            {
                // Get line as bytes
                byte[] bytes = challenge1.HexStringToBytes(lines[i]);

                // Get the max scoring cypher for this line
                KeyValuePair<int, Tuple<double, string>> maxScoringItem = challenge3.SingleKeyXORBruteForce(bytes);

                // If this line has a higher score than the previous highest, update the max and store a reference
                if (maxScoringItem.Value.Item1 > maxLineScore)
                {
                    // Update the max score
                    maxLineScore = maxScoringItem.Value.Item1;

                    // Store a reference to the max scoring line
                    maxScoringLine = maxScoringItem;

                    // Reference to the line number
                    maxScoringLineNumber = i;
                }
            }

            // Format the output
            return challenge3.FormatOutput(maxScoringLine, $"{Environment.NewLine}Line   : {maxScoringLineNumber}"); ;
        }

        /// <summary>
        /// Determine the amount of byte values not expressed in a given string input
        /// </summary>
        /// <param name="text">The text to get the missing byte count of</param>
        /// <returns>The number of missing bytes (i.e. how many of the 256 byte values were not expressed by the text)</returns>
        private int GetMissingByteCountForHexString(string text)
        {
            // Convert the input hex text to bytes
            byte[] bytes = challenge1.HexStringToBytes(text);

            // Count the number of unique bytes expressed by the text
            List<byte> expressedBytes = new List<byte>();
            foreach(byte hextet in bytes)
            {
                if (!expressedBytes.Contains(hextet))
                    expressedBytes.Add(hextet);
            }

            // Calculate the amount of missing bytes from the ASCII value range (0 - 256)
            int maxByteCount = 256;
            int missingByteCount = maxByteCount - expressedBytes.Count;

            return missingByteCount;
        }

        ///<inheritdoc cref="IChallenge4"/>
        public string[] SplitTextIntoLines(string text, string separator = Constants.Separator)
        {
            // Unescape separator incase it includes slashes
            separator = Regex.Unescape(separator);

            // Split around separator
            return text.Split(separator);
        }
    }
}
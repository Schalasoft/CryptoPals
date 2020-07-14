using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoPals.Sets
{
    class Challenge4 : IChallenge
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

        // Solve the challenge
        public string Solve(string input)
        {
            // Get individual lines
            string[] lines = SplitTextIntoLines(input);

            return SolveFast(lines);
            //return SolveSlow(lines);
        }

        // Go through each line and find the line with the most missing bytes, which is likely to be the XOR encoded line as XOR removes bits
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

            // Get the max scoring cypher for the line with the most missing bytes
            KeyValuePair<int, Tuple<double, string>> maxScoringItem = challenge3.SingleKeyXORBruteForce(mostMissingBytesLine, true);

            // Format the output
            string output = challenge3.FormatOutput(maxScoringItem, $"{Environment.NewLine}Line   : {mostMissingBytesLineNumber}{Environment.NewLine}Bytes  : {mostMissingBytes} missing");

            return output;
        }

        // Go through each line, XOR decode and choose the line with the highest score
        private string SolveSlow(string[] lines)
        {
            // Decode lines
            double maxLineScore = 0;
            int maxScoringLineNumber = 0;
            KeyValuePair<int, Tuple<double, string>> maxScoringLine = new KeyValuePair<int, Tuple<double, string>>();
            for (int i = 0; i < lines.Length; i++)
            {
                // Get the max scoring cypher for this line
                KeyValuePair<int, Tuple<double, string>> maxScoringItem = challenge3.SingleKeyXORBruteForce(lines[i], true);

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
            string output = challenge3.FormatOutput(maxScoringLine, $"{Environment.NewLine}Line   : {maxScoringLineNumber}");

            return output;
        }

        // Determine the amount of byte values not expressed in a given string input
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

        // Split text into lines based on newline characters
        private string[] SplitTextIntoLines(string text)
        {
            return text.Split(Environment.NewLine);
        }
    }
}
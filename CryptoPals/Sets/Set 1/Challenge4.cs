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
        IChallenge3 challenge3 = (IChallenge3)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge3);

        // Solve the challenge
        public string Solve(string input)
        {
            return SolveFast(input); // todo GetMissingByteCountForString is not giving the expected result, look at this later
            //return SolveSlow(input);
        }

        // Go through each line and find the line with the most missing bytes, which is likely to be the XOR encoded line as XOR removes bits
        private string SolveFast(string input)
        {
            // Get individual lines
            string[] lines = input.Split("\r\n");

            // Iterate lines and find the one with the most missing bytes
            int mostMissingBytes = 0;
            string mostMissingBytesLine = "";
            int mostMissingBytesLineNumber = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                // Find the count of missing bytes
                int missingBytes = GetMissingByteCountForString(lines[i]);

                // If this line has more missing bytes than the previous max, update the max and save a reference to the line
                if(missingBytes > mostMissingBytes)
                {
                    mostMissingBytes = missingBytes;
                    mostMissingBytesLine = lines[i];
                    mostMissingBytesLineNumber = i;
                }
            }

            // Get the max scoring cypher for the line with the most missing bytes
            KeyValuePair<int, Tuple<double, string>> maxScoringItem = challenge3.GetMaxScoringItem(mostMissingBytesLine);

            // Format the output
            string output = challenge3.FormatOutput(maxScoringItem, $"{Environment.NewLine}Line   : {mostMissingBytesLineNumber}{Environment.NewLine}Bytes  : {mostMissingBytes} missing");

            return output;
        }

        // Go through each line, XOR decode and choose the line with the highest score
        private string SolveSlow(string input)
        {
            // Get individual lines
            string[] lines = input.Split("\r\n");

            // Decode lines
            double maxLineScore = 0;
            int maxScoringLineNumber = 0;
            KeyValuePair<int, Tuple<double, string>> maxScoringLine = new KeyValuePair<int, Tuple<double, string>>();
            for (int i = 0; i < lines.Length; i++)
            {
                // Get the max scoring cypher for this line
                KeyValuePair<int, Tuple<double, string>> maxScoringItem = challenge3.GetMaxScoringItem(lines[i]);

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
        private int GetMissingByteCountForString(string input)
        {
            // Count the number of unique bytes expressed by the text
            List<byte> expressedBytes = new List<byte>();
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            foreach (byte b in bytes)
            {
                if (!expressedBytes.Contains(b))
                    expressedBytes.Add(b);
            }

            // Calculate the amount of missing bytes from the ASCII value range (0 - 256)
            int maxByteCount = 256;
            int missingByteCount = maxByteCount - expressedBytes.Count;

            return missingByteCount;
        }
    }
}
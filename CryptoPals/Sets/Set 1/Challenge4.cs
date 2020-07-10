using CryptoPals.Factories;
using CryptoPals.Interfaces;

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
        IChallenge3 challenge3 = (IChallenge3)ChallengeFactory.InitializeChallenge(Enumerations.ChallengeEnum.Challenge3);

        // Solve the challenge
        public string Solve(string input)
        {
            //return SolveFast(input);
            return SolveSlow(input);
        }

        // Go through each line and find the line with the most missing bytes, which is likely to be the XOR encoded line
        private string SolveFast(string input)
        {
            return "";
        }
        
        // Go through each line, XOR decode and choose the line with the highest score
        private string SolveSlow(string input)
        {
            // Get individual lines
            string[] lines = input.Split("\r\n");

            // Decode lines
            string[] decodedLines = new string[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                decodedLines[i] = challenge3.GetMaxScoringItem(lines[i]).Value.Item2;
            }

            string output = "";

            return output;
        }
    }
}
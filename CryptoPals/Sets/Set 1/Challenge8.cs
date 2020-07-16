using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using Org.BouncyCastle.Crypto.Paddings;
using System;
using System.Linq;

namespace CryptoPals.Sets
{
    class Challenge8 : IChallenge
    {
        /*
        Detect AES in ECB mode
        In this file are a bunch of hex-encoded ciphertexts.

        One of them has been encrypted with ECB.

        Detect it.

        Remember that the problem with ECB is that it is stateless and deterministic; the same 16 byte plaintext block will always produce the same 16 byte ciphertext.
        */

        // Reuse previous challenge functionality
        IChallenge1 challenge1 = (IChallenge1)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge1);
        IChallenge6 challenge6 = (IChallenge6)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge6);

        public string Solve(string input)
        {
            // Split hex encoded input into lines
            string[] lines = input.Split(Environment.NewLine);

            // Go through each line
            int[] repeatedBlockCounts = new int[lines.Length];
            int size = 16;
            for (int i = 0; i < lines.Length; i++)
            {
                // Get the hex line as bytes
                byte[] bytes = challenge1.HexStringToBytes(lines[i]);

                // Break into blocks (size of 16 as the challenge hinted at this number)
                byte[][] blocks = challenge6.CreateBlocks(bytes, size);

                // Get the number of distinct blocks, due to being a 2d array we use LINQ select to effectively flatten the arrays for use with Distinct
                int distinctBlocks = blocks.Select(x => string.Join(",", x)).Distinct().ToArray().Length;

                // Get the amount of repeated blocks we find by subtracting the amount of blocks from the number of distinct blocks
                int repeatedBlockCount = blocks.Length - distinctBlocks;

                // Add the amount of repeated blocks for this line to an array (to identify the line index with the most repetitions)
                repeatedBlockCounts[i] = repeatedBlockCount;
            }

            // Get the AES encrypted line by getting the index of the line with the most repeated block
            int mostRepetitions = repeatedBlockCounts.Max();
            int encryptedLineIndex = Array.FindIndex(repeatedBlockCounts, x => x == mostRepetitions) + 1; // Add 1 as we count lines from 1 not 0

            // Format the output
            string output = $"Index  : {encryptedLineIndex}{Environment.NewLine}Dupes  : {mostRepetitions}{Environment.NewLine}{lines[encryptedLineIndex]}";

            return output;
        }
    }
}

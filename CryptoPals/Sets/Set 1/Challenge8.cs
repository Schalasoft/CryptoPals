using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
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
            for(int i = 0; i < lines.Length; i++)
            {
                // Get the line/hex as bytes
                byte[] bytes = challenge1.HexStringToBytes(lines[0]);

                // Break into blocks (16 as the challenge hinted at this number)
                int size = 16;
                byte[][] blocks = challenge6.CreateBlocks(bytes, size);

                // Count the amount of repeated blocks we find
                int repeatedBlockCount = 0;
                for (int j = 0; j < blocks.Length; j++)
                {
                    for (int k = 0; k < blocks.Length; k++)
                    {
                        // Check if the block matches another block, exclude matching the block against itself
                        if (j != k && Enumerable.SequenceEqual(blocks[j], blocks[k]))
                        {
                            repeatedBlockCount++;
                        } 
                    }
                }

                // Add the amount of repeated blocks for this line to an array (to find the line index)
                repeatedBlockCounts[i] = repeatedBlockCount;
            }

            // Get the AES encrypted line by getting the index of the line with the most repeated block
            int mostRepetitions = repeatedBlockCounts.Max();
            int encryptedLineIndex = Array.FindIndex(repeatedBlockCounts, x => x == mostRepetitions);

            // Format the output
            string output = $"{lines[encryptedLineIndex]}{Environment.NewLine}Index  : {encryptedLineIndex}{Environment.NewLine}Dupes  : {mostRepetitions}";

            return output;
        }
    }
}

using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Linq;
using System.Text;

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
        IChallenge7 challenge7 = (IChallenge7)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge7);

        public string Solve(string input)
        {
            // Split hex encoded input into lines
            string[] lines = input.Split(Environment.NewLine);

            // Go through each line
            int[] repeatedChunkCounts = new int[lines.Length];
            for(int i = 0; i < lines.Length; i++)
            {
                // Get the line/hex as bytes
                byte[] bytes = challenge1.HexStringToBytes(lines[0]);

                // Break into blocks (16 as the challenge hinted at this number)
                int size = 16;
                byte[][] blocks = challenge6.CreateBlocks(bytes, size);

                // Count the amount of repeated chunks we find
                int repeatedChunkCount = 0;
                for (int j = 0; j < blocks.Length; j++)
                {
                    for (int k = 0; k < blocks.Length; k++)
                    {
                        // Check if the block matches another block, exclude matching the block against itself
                        if (j != k && Enumerable.SequenceEqual(blocks[j], blocks[k]))
                        {
                            repeatedChunkCount++;
                        } 
                    }
                }

                // Add the amount of repeated chunks for this line to the array of line chunk counts (to find the line index)
                repeatedChunkCounts[i] = repeatedChunkCount;
            }

            // Get the AES encrypted line by getting the index of the line with the most repeated chunks
            int mostRepetitions = repeatedChunkCounts.Max();
            int encryptedLineIndex = Array.FindIndex(repeatedChunkCounts, x => x == mostRepetitions);

            string output = $"{lines[encryptedLineIndex]}{Environment.NewLine}Index  : {encryptedLineIndex}{Environment.NewLine}Dupes  : {mostRepetitions}";

            return output;
        }
    }
}

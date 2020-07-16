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
            // Split input into lines
            string[] lines = input.Split(Environment.NewLine);

            // Split each line into blocks of a particular size to find repeating patterns
            int[] repeatedChunkCounts = new int[lines.Length];
            for(int i = 0; i < lines.Length; i++)
            {
                // Get lines hex string as bytes
                byte[] bytes = challenge1.HexStringToBytes(lines[0]);

                // Count the amount of repeated chunks we find in a line
                int repeatedChunkCount = 0;

                // Break text up into blocks (16 as the challenge hinted at this number)
                int size = 16;
                byte[][] blocks = challenge6.CreateBlocks(bytes, size);

                // Check if there are any recurring patterns in the blocks
                for(int j = 0; j < blocks.Length; j++)
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

                // Add the amount of repeated chunks for this line to the array containing the repeat counts for all lines
                repeatedChunkCounts[i] = repeatedChunkCount;
            }

            // Get the AES encrypted line by getting the index of the line with the most repetitions
            int encryptedLine = Array.FindIndex(repeatedChunkCounts, x => x == repeatedChunkCounts.Max());

            // Decrypt
            //byte[] decrypted = challenge7.Decrypt(encryptedBytes, key);
            //string output = Encoding.ASCII.GetString(decrypted);

            string output = "";

            return output;
        }
    }
}

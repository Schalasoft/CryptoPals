﻿using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using CryptoPals.Managers;
using System;
using System.Linq;

namespace CryptoPals.Sets
{
    ///<inheritdoc cref="IChallenge8"/>
    class Challenge8 : IChallenge8, IChallenge
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
        IChallenge4 challenge4 = (IChallenge4)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge4);
        IChallenge6 challenge6 = (IChallenge6)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge6);

        /// <inheritdoc />
        public string Solve(string input)
        {
            // Split hex encoded input into lines
            string[] lines = challenge4.SplitTextIntoLines(input, Environment.NewLine);

            // Go through each line and get the repeated block count
            int[] repeatedBlockCounts = new int[lines.Length];
            int blockSize = 16;
            for (int i = 0; i < lines.Length; i++)
            {
                // Convert hex encoded input lines to bytes
                byte[] bytes = challenge1.HexStringToBytes(lines[i]);

                // Go through each line and get the repeated block count
                repeatedBlockCounts[i] = GetRepeatedBlockCount(bytes, blockSize);
            }

            // Get the AES encrypted line by getting the index of the line with the most repeated block
            int mostRepetitions = repeatedBlockCounts.Max();
            int encryptedLineIndex = Array.FindIndex(repeatedBlockCounts, x => x == mostRepetitions) + 1; // Add 1 as we count lines from 1 not 0

            return FormatOutput(encryptedLineIndex, mostRepetitions, lines[encryptedLineIndex]);
        }

        /// <summary>
        /// Format the output
        /// </summary>
        /// <param name="lineIndex">The line index of the ECB encrypted text</param>
        /// <param name="repetitions">How many block repetitions were found</param>
        /// <param name="line">The encrypted line</param>
        /// <returns>The formatted output containing the line index, repetitions, and encrypted line</returns>
        private string FormatOutput(int lineIndex, int repetitions, string line)
        {
            return $"Index  : {lineIndex}{Environment.NewLine}Dupes  : {repetitions}{Environment.NewLine}{line}";
        }

        ///<inheritdoc cref="IChallenge8.GetRepeatedBlockCount(byte[], int)"/>
        public int GetRepeatedBlockCount(byte[] bytes, int blockSize = 0)
        {
            // Break into blocks (size of 16 as the challenge hinted at this number)
            byte[][] blocks = challenge6.CreateBlocks(bytes, blockSize);

            // Get the number of distinct blocks, due to being a 2d array we use LINQ select to effectively flatten the arrays for use with Distinct
            int distinctBlocks = blocks.Select(x => string.Join(",", x)).Distinct().ToArray().Length;

            // Return the amount of repeated blocks we find by subtracting the amount of distinct blocks from the total number of blocks
            return blocks.Length - distinctBlocks;
        }

        ///<inheritdoc cref="IChallenge8.IsECBEncrypted(byte[])"/>
        public bool IsECBEncrypted(byte[] bytes)
        {
            // Try multiple block sizes
            // The amount of block sizes to try
            int blockSizeAttempts = 20;
            for (int i = 0; i < blockSizeAttempts; i++)
            {
                // Start with a block size of 4, then try increasing block sizes (will start at 4 so this trys 4 to 24)
                int blockSize = 4 + i;

                // If we find any repeated blocks, assume it is ECB
                if (GetRepeatedBlockCount(bytes, blockSize) > 0)
                    return true;
            }
            
            return false;
        }
    }
}

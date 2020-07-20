﻿using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CryptoPals.Sets
{
    /// <inheritdoc cref="IChallenge12"/>
    class Challenge12 : IChallenge12, IChallenge
    {
        /*
        Byte-at-a-time ECB decryption (Simple)
        Copy your oracle function to a new function that encrypts buffers under ECB mode 
        using a consistent but unknown key (for instance, assign a single random key, once, 
        to a global variable).

        Now take that same function and have it append to the plaintext, BEFORE ENCRYPTING, 
        the following string:

        Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkg
        aGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBq
        dXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUg
        YnkK
        Spoiler alert.
        Do not decode this string now. Don't do it.

        Base64 decode the string before appending it. Do not base64 decode the string by hand; 
        make your code do it. The point is that you don't know its contents.

        What you have now is a function that produces:

        AES-128-ECB(your-string || unknown-string, random-key)
        It turns out: you can decrypt "unknown-string" with repeated calls to the oracle function!

        Here's roughly how:

        1) Feed identical bytes of your-string to the function 1 at a time --- start with 1 byte ("A"), 
        then "AA", then "AAA" and so on. Discover the block size of the cipher. You know it, 
        but do this step anyway.
        2) Detect that the function is using ECB. You already know, but do this step anyways.
        3) Knowing the block size, craft an input block that is exactly 1 byte short 
        (for instance, if the block size is 8 bytes, make "AAAAAAA"). 
        Think about what the oracle function is going to put in that last byte position.
        4) Make a dictionary of every possible last byte by feeding different strings to the oracle; 
        for instance, "AAAAAAAA", "AAAAAAAB", "AAAAAAAC", remembering the first block of each invocation.
        5) Match the output of the one-byte-short input to one of the entries in your dictionary. 
        You've now discovered the first byte of unknown-string.
        6) Repeat for the next byte.

        Congratulations.
        This is the first challenge we've given you whose solution will break real crypto. 
        Lots of people know that when you encrypt something in ECB mode, you can see penguins through it. 
        Not so many of them can decrypt the contents of those ciphertexts, and now you can. 
        If our experience is any guideline, this attack will get you code execution in security tests 
        about once a year.
        */

        // Reuse previous challenge functionality
        IChallenge7 challenge7   = (IChallenge7)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge7);
        IChallenge8 challenge8   = (IChallenge8)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge8);
        IChallenge9 challenge9   = (IChallenge9)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge9); // cdg debug
        IChallenge11 challenge11 = (IChallenge11)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge11);

        /// <inheritdoc />
        public string Solve(string input)
        {
            // Convert input to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(input);

            // The text to append after the input bytes
            string base64Text = "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK";

            // Base64 decode the text
            byte[] base64Bytes = Convert.FromBase64String(base64Text);

            // Generate a random key
            //byte[] key = challenge11.GenerateRandomASCIIBytes(16);
            byte[] key = challenge9.PadBytes(new byte[0], 16, 1); // CDG DEBUG

            // Append additional bytes after the input bytes
            byte[] appendedBytes = challenge11.InsertBytes(bytes, base64Bytes, false);

            // Encrypt the bytes with the key
            byte[] encryptedBytes = challenge7.AES_ECB(true, appendedBytes, key);

            // Encrypt the unknown string for decrypting
            byte[] encryptedUnknownBytes = challenge7.AES_ECB(true, base64Bytes, key);

            // Decrypt the entire text and find the unknown string, just output the last 500 characters of the data
            return DecryptUnknownString(encryptedBytes, encryptedUnknownBytes, key);//.Substring(encryptedBytes.Length - 500, 500); // cdg todo
        }

        private byte[] BuildMappingTable(int blockSize, char character, byte[] key)
        {
            int size = 128;
            byte[] mappings = new byte[size];
            for (int i = 0; i < size; i++)
            {
                // Encrypt the short block
                byte[] shortEncrypt = EncryptShortBlock(Convert.ToChar(i), blockSize, character, key);

                // Add it to the dictionary (the actual ASCII char as the key, and the encrypted char as the value)
                mappings[i] = shortEncrypt[shortEncrypt.Length - 1];
            }

            return mappings;
        }

        private string BuildShortBlock(char lastCharacter, int blockSize, char character)
        {
            // Create a block exactly 1 byte short of the block size and add the last unique character
            return $"{"".PadRight(blockSize - 1, character)}{lastCharacter}";
        }


        private int DetermineEncryptorBlockSize(char character, byte[] key)
        {
            // Feed identical bytes to encryptor
            int blockSize = 0;
            int rightShiftCount = 0;

            // Keep trying until we get a block size, or we've tried shifting the bytes by 10
            while (blockSize == 0 || rightShiftCount < 10)
            {
                // Try various block sizes
                for (int i = 4; i <= 128; i++)
                {
                    // Create a string of identical characters of the specified length
                    string text = "".PadRight(i, character);

                    // Convert text to bytes
                    byte[] checkBytes = Encoding.ASCII.GetBytes(text);

                    // Pad the bytes with random bytes up to at least 16 bytes or else the encryption will fail
                    if(checkBytes.Length < 16)
                        checkBytes = challenge11.InsertBytes(checkBytes, challenge11.GenerateRandomASCIIBytes(16 - checkBytes.Length), true);

                    // Encrypt the text
                    byte[] encryptedBytes = challenge7.AES_ECB(true, checkBytes, key);

                    // Right shift the bytes incase the bytes that were randomly added throw off the check
                    // We use random bytes but unique bytes would be a more ideal solution
                    if(rightShiftCount > 0) 
                        encryptedBytes = challenge11.InsertBytes(encryptedBytes, challenge11.GenerateRandomASCIIBytes(rightShiftCount), true);

                    // If we find repeated blocks then this loops iterator is the block size
                    int currentBlockSize = i / 2; // Block size should be half the text/byte size for check
                    if (challenge8.GetRepeatedBlockCount(encryptedBytes, currentBlockSize) > 0)
                    {
                        // Update the block size and set the flag to look for another larger block size
                        blockSize = currentBlockSize;
                        break;
                    }
                }

                // If we found a block size, break out of the while loop
                if (blockSize > 1)
                    break;

                // Increment the right shift count as we found nothing on our first attempt at all block sizes
                rightShiftCount++;
            }

            return blockSize;
        }

        private string DecryptUnknownString(byte[] bytes, byte[] unknownBytes, byte[] key)
        {
            // The character we are going to use for encryption
            char character = 'A';

            // Detect the block size of the cipher
            int blockSize = DetermineEncryptorBlockSize(character, key);

            // Detect if the function is using ECB
            bool isUsingECB = challenge8.IsECBEncrypted(bytes);

            // This will only work if the encryption is ECB
            string output = "";
            if (isUsingECB)
            {
                // Output the, now known, string
                output = GetUnknownString(bytes, unknownBytes, blockSize, character, key);
            }

            return output;
        }

        private byte[] EncryptShortBlock(char lastCharacter, int blockSize, char character, byte[] key)
        {
            // Create a block with all the same character, except the last byte which will be unique
            string blockText = BuildShortBlock(lastCharacter, blockSize, character);

            // Convert the short block to bytes
            byte[] blockBytes = Encoding.ASCII.GetBytes(blockText);

            // Encrypt the short block
            return challenge7.AES_ECB(true, blockBytes, key);
        }

        // cdg todo block size failed

        private string GetUnknownString(byte[] bytes, byte[] unknownBytes, int blockSize, char character, byte[] key)
        {
            // Build dictionary used to hold all possible last byte combinations for the identical strings ("AAAAAAAA", "AAAAAAAB", "AAAAAAAC" etc.)
            byte[] mappings = BuildMappingTable(blockSize, character, key);

            // Match the output of the short block to the dictionary key to get each character of the unknown string
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < unknownBytes.Length; i++)
            {
                // Encrypt a short block
                byte[] shortEncrypt = EncryptShortBlock(Convert.ToChar(unknownBytes[i]), blockSize, character, key);

                // Get the last byte of the encryption
                byte lastUnknownByte = shortEncrypt[shortEncrypt.Length - 1];

                // Get the match from the dictionary (taking the index to get the ASCII value)
                byte actualByte = (byte)Array.FindIndex(mappings, x => x.Equals(lastUnknownByte));

                // Get the match key (the character index/ascii byte value)
                char lastCharacter = Convert.ToChar(actualByte);

                // Append the final character of the match to the string builder for outputting
                // cdg todo we can't just check the first encrypted byte because it has to be at the 8th position
                stringBuilder.Append(lastCharacter);
            }

            return stringBuilder.ToString();
        }
    }
}

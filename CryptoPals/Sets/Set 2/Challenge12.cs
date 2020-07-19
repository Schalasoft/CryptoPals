using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
        IChallenge9 challenge9   = (IChallenge9)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge9);
        IChallenge11 challenge11 = (IChallenge11)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge11);

        /// <inheritdoc />
        public string Solve(string input)
        {
            // Convert input to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(input);

            // The text to append before the input bytes
            string base64Text = "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK";

            // Base64 decode the text
            byte[] base64Bytes = Convert.FromBase64String(base64Text);

            // Generate a random key
            byte[] key = challenge11.GenerateRandomASCIIBytes(16);

            // Append additional bytes after the input bytes
            byte[] appendedBytes = challenge11.InsertBytes(bytes, base64Bytes, false);

            // Encrypt the bytes with the key
            byte[] encryptedBytes = challenge7.AES_ECB(true, appendedBytes, key);

            // Decrypt the Base64 text
            return DecryptUnknownString(encryptedBytes, key);
        }

        private string DecryptUnknownString(byte[] bytes, byte[] key)
        {
            // The character we are going to use for encryption
            char character = 'A';

            // Feed identical bytes to encryptor
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i <= 16; i++)
            {
                // Create a string of identical characters of the specified length
                string text = "".PadRight(i, character);

                // Convert text to bytes
                byte[] checkBytes = Encoding.ASCII.GetBytes(text);

                // Pad the bytes up to the appropriate size CDG TODO this might be a problem...
                checkBytes = challenge9.PadBytes(checkBytes, 16);

                // Encrypt the text
                byte[] resultBytes = challenge7.AES_ECB(true, checkBytes, key);

                // Detect the block size of the cipher
                int blockSize = 4;//todo

                // Detect if the function is using ECB
                bool isUsingECB = challenge8.IsECBEncrypted(resultBytes);

                // Create a block exactly 1 byte short of the block size
                string shortBlockText = "".PadRight(blockSize - 1, character);

                // Convert the short block to bytes
                byte[] shortBlockBytes = Encoding.ASCII.GetBytes(shortBlockText);

                // Dictionary used to hold all possible last byte combinations for the identical strings ("AAAAAAAA", "AAAAAAAB", "AAAAAAAC" etc.)
                Dictionary<string, char> lastByteCombinations = PopulateLastByteCombinationsDictionary(shortBlockText, character);

                // Pad the bytes up to the appropriate size CDG TODO this might be a problem...
                shortBlockBytes = challenge9.PadBytes(shortBlockBytes, 16);

                // Encrypt the short block
                byte[] shortEncrypt = challenge7.AES_ECB(true, shortBlockBytes, key);

                // Convert the encryption to a string for matching against the dictionary
                string shortEncryptText = Encoding.ASCII.GetString(shortEncrypt);

                // Match the output of the short block to the dictionary key to get the first character of the unknown string
                stringBuilder.Append(lastByteCombinations[shortEncryptText]);
            }

            return stringBuilder.ToString();
        }


        private Dictionary<string, char> PopulateLastByteCombinationsDictionary(string block, char character)
        {
            Dictionary<string, char> combinations = new Dictionary<string, char>();
            for (int i = 0; i <= 256; i++)
            {
                // Create a string of identical characters of the specified length
                string text = block.PadRight(block.Length, character);

                // Create the character to append
                char lastCharacter = Convert.ToChar(i);

                // Add the last character
                text += lastCharacter;

                // Add it to the dictionary
                combinations.Add(text, lastCharacter);
            }
            return combinations;
        }
    }
}

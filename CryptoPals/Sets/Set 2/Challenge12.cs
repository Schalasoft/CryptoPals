using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using CryptoPals.Extension_Methods;

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
        IChallenge6 challenge6   = (IChallenge6)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge6);
        IChallenge7 challenge7   = (IChallenge7)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge7);
        IChallenge8 challenge8   = (IChallenge8)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge8);
        IChallenge9 challenge9   = (IChallenge9)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge9);
        IChallenge11 challenge11 = (IChallenge11)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge11);

        // The text to append after the input bytes
        static string base64Text = "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK";

        // Base64 decode the text
        byte[] unknownBytesEncrypted;

        // The random key (generated once)
        byte[] key;

        /// <inheritdoc />
        public string Solve(string input)
        {
            // Assign the key
            int keySize = 16;
            key = challenge11.GenerateRandomASCIIBytes(keySize);

            // Detect the block size of the cipher
            int blockSize = DetermineEncryptorBlockSize();

            // Detect if the encryptor is using ECB
            bool isUsingECB = IsEncryptorUsingECB(blockSize);

            string output = "";
            if(isUsingECB)
            {
                // Get the encrypted bytes we want to decrypt
                unknownBytesEncrypted = Oracle("");

                // Decrypt the unknown string
                output = DecryptUnknownString(unknownBytesEncrypted, blockSize);
            }

            return output;
        }

        /// <summary>
        /// Determines if the encryptor is using ECB, this is done by sending it 2 blocks worth of identical data, and seeing if we get back 2 identical cipher text blocks
        /// </summary>
        /// <param name="blockSize">The block size used by the encryptor</param>
        /// <returns>True if the encryptor is using AES ECB</returns>
        private bool IsEncryptorUsingECB(int blockSize)
        {
            // Construct 2 blocks worth of identical data
            string text = "".PadRight(blockSize * 2, 'A');

            // Encrypt the data
            byte[] encrypted = Oracle(text);

            // Check if the encrption contains 2 indentical blocks
            return challenge8.IsECBEncrypted(encrypted);
        }

        /// <summary>
        /// Determines the block size used by the encryptor
        /// </summary>
        /// <returns>The block size the ECB encryptor is using</returns>
        private int DetermineEncryptorBlockSize()
        {
            int maxBlockSize = 2000;

            // Do an initial encrypt on blank text
            char c = 'A'; // A char to use to fill the blocks
            string text = "012345678912345".PadRight(16, c);
            byte[] initialEcrypt = Oracle(text); 

            // Feed larger and larger sets of bytes to the encryptor until the size changes, then we have the block size
            int blockSize = 0;
            for (int i = 0; i <= maxBlockSize; i++)
            {
                // Add a character to the text to encrypt
                text += c;

                // Encrypt the text
                byte[] encrypted = Oracle(text);

                // If the size has changed, the block size is the difference between the two, then leave the loop
                if (encrypted.Length > initialEcrypt.Length)
                {
                    blockSize = encrypted.Length - initialEcrypt.Length;
                    break;
                }
            }

            return blockSize;
        }

        /// <summary>
        /// Encrypts a string, while also appending an unknown string to the end of it
        /// </summary>
        /// <param name="text">The string to encrypt (the unknown string is appended to this)</param>
        /// <returns>The encryption of the text provided, after an unknown string was appended to it</returns>
        public byte[] Oracle(string text)
        {
            // Add unknown bytes
            text += Convert.FromBase64String(base64Text).ToASCIIString();

            // Encrypt
            byte[] output = challenge7.AES_ECB(true, text.ToBytes(), key);

            // Pad the bytes so it is a multiple of the block size
            return challenge9.PadBytesToBlockSizeMultiple(output, 16); // The Oracle knows the block size, as the Oracle knows all!
        }

        /// <summary>
        /// Decrypts the encrypted bytes
        /// </summary>
        /// <param name="bytes">The encrypted bytes</param>
        /// <param name="blockSize">The size of the blocks used in decryption</param>
        /// <returns>The decrypted bytes as a string</returns>
        private string DecryptUnknownString(byte[] bytes, int blockSize)
        {
            // Split the unknown text in blocks using the encryptions block size
            byte[][] blocks = challenge6.CreateBlocks(bytes, blockSize);

            // Decrypt all the bytes (use an array of lists)
            List<byte> decryptedBytes = new List<byte>();
            for (int i = 0; i <= blocks.Length; i++)
            {
                // Decrypt each block 1 byte at a time
                for (int j = 0; j < blockSize; j++)
                {
                    // The start index of the block we want to decrypt
                    int blockIndex = i * blockSize;

                    // Decrypt each byte and add it to the list
                    byte decryptedByte = DecryptUnknownByte(blockIndex, blockSize, decryptedBytes);

                    // If we hit padding, exit
                    if (decryptedByte == (byte)4)
                        break;

                    // Copy the decrypted char to the list that holds all the decrypted blocks
                    decryptedBytes.Add(decryptedByte);
                }
            }
            
            // Return the decrypted bytes flattened as an ASCII string
            return decryptedBytes.ToArray().ToASCIIString();
        }

        // cdg todo 

        // refactor previous challenges to use extension methods
        // move cryptographic methods (AES and CBC encrypt etc. to Cryptography class in Utilities)

        /// <summary>
        /// Decrypts the next byte using previously decrypted bytes (or in the case of the first byte, a block of all the same byte)
        /// </summary>
        /// <param name="blockIndex">The starting index of the current block</param>
        /// <param name="blockSize">The size of the blocks used in decryption</param>
        /// <param name="decryptedBytes">The bytes already decrypted</param>
        /// <returns>The next decrypted byte</returns>
        private byte DecryptUnknownByte(int blockIndex, int blockSize, List<byte> decryptedBytes)
        {
            // Padding
            int padding = (blockSize - (decryptedBytes.Count % blockSize) - 1);

            // Get our previously decrypted bytes as a string
            string decryptedString = String.Join("", decryptedBytes.ToArray().ToASCIIString());

            // Create the block to encrypt that will be our target
            byte[] shortBlock = challenge9.PadBytes(padding);

            // Encrypt
            List<byte> encrypted = Oracle(shortBlock.ToASCIIString()).ToList();

            // If we cannot grab anymore in this block, we're done, return a padding byte
            if (encrypted.Count < blockIndex + blockSize)
                return (byte)4;

            // Get the encrypted block we are on
            byte[] target = encrypted.GetRange(blockIndex, blockSize).ToArray();

            // Try each byte it could be and get the one it actually is
            byte decryptedByte = new byte();
            for (int i = 0; i < 256; i++)
            {
                // Encrypt the guess
                byte[] guessBlocks = Oracle($"{shortBlock.ToASCIIString()}{decryptedString}{(char)i}");

                // Get the block we are on
                byte[] guessBlock = guessBlocks.ToList().GetRange(blockIndex, blockSize).ToArray();

                if (guessBlock.SequenceEqual(target))
                {
                    decryptedByte = (byte)i;
                    break;
                }
            }

            return decryptedByte;
        }
    }
}

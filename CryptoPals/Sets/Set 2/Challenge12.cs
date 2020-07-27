using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using CryptoPals.Extension_Methods;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Paddings;

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
        byte[] unknownBytesUnencrypted = Convert.FromBase64String(base64Text); // Only used by the Oracle

        /// <inheritdoc />
        public string Solve(string input)
        {
            // Generate a random key
            int keySize = 16;
            byte[] key = challenge11.GenerateRandomASCIIBytes(keySize);
            key = "YELLOW SUBMARINE".ToBytes(); // cdg debug

            // Detect the block size of the cipher
            int blockSize = DetermineEncryptorBlockSize();

            // Detect if the encryptor is using ECB
            bool isUsingECB = IsEncryptorUsingECB(blockSize);

            string output = "";
            if(isUsingECB)
            {
                // Get the encrypted bytes we want to decrypt
                unknownBytesEncrypted = Oracle(true, "", key);

                // Decrypt the unknown string
                output = DecryptUnknownString(unknownBytesEncrypted, blockSize, key);
            }

            return output;
        }

        // Move to challenge 9
        private byte[] PadBytesToBlockSizeMultiple(byte[] bytes, int blockSize, byte paddingByte = 4)
        {
            byte[] output;
            
            // The byte array needs resized
            if (bytes.Length % blockSize != 0)
            {
                // Determine the new size (the size of the bytes plus how many bytes we are missing, which is the block size minus the modulus remainder of the length of the bytes and the block size)
                int newSize = bytes.Length + ((blockSize) - bytes.Length % blockSize);

                // Create resized byte array
                output = challenge9.PadBytes(bytes, newSize, paddingByte);
            }
            else
            {
                // Doesn't need a resize, just output the bytes
                output = bytes;
            }

            return output;
        }

        private bool IsEncryptorUsingECB(int blockSize)
        {
            // Construct 2 blocks worth of identical data
            string text = "".PadRight(blockSize * 2, 'A');

            // Encrypt the data (just use a blank key)
            byte[] encrypted = Oracle(true, text, new byte[16]);

            // Check if the encrption contains 2 indentical blocks
            return challenge8.IsECBEncrypted(encrypted);
        }

        private int DetermineEncryptorBlockSize()
        {
            int maxBlockSize = 2000;

            // Just use a 128 bit blank key
            byte[] key = new byte[16];

            // Do an initial encrypt on blank text
            char c = 'A'; // A char to use to fill the blocks
            string text = "012345678912345".PadRight(16, c);
            byte[] initialEcrypt = Oracle(true, text, key); 

            // Feed larger and larger sets of bytes to the encryptor until the size changes, then we have the block size
            int blockSize = 0;
            for (int i = 0; i <= maxBlockSize; i++)
            {
                // Add a character to the text to encrypt
                text += c;

                // Encrypt the text
                byte[] encrypted = Oracle(true, text, key);

                // If the size has changed, the block size is the difference between the two, then leave the loop
                if (encrypted.Length > initialEcrypt.Length)
                {
                    blockSize = encrypted.Length - initialEcrypt.Length;
                    break;
                }
            }

            return blockSize;
        }

        public byte[] Oracle(bool encrypt, string text, byte[] key)
        {
            // Add unknown bytes
            text += unknownBytesUnencrypted.ToASCIIString();

            // Encrypt
            byte[] output = challenge7.AES_ECB(encrypt, text.ToBytes(), key);

            // Pad the bytes so it is a multiple of the block size
            return PadBytesToBlockSizeMultiple(output, 16); // The Oracle knows the block size, as the Oracle knows all!
        }

        private string DecryptUnknownString(byte[] bytes, int blockSize, byte[] key)
        {
            // Split the unknown text in blocks using the encryptions block size
            byte[][] blocks = challenge6.CreateBlocks(bytes, blockSize);

            // Decrypt all the bytes (use an array of lists)
            List<byte> decryptedBytes = new List<byte>();
            for (int i = 0; i < blocks.Length; i++)
            {
                // Variable to hold decrypted bytes from a block
                List<byte> currentBlock = new List<byte>();

                // Decrypt each block 1 byte at a time
                for (int j = 0; j < blockSize; j++)
                {
                    // Decrypt each byte and add it to the list
                    byte decryptedByte = DecryptUnknownByte(i, blockSize, key, currentBlock, decryptedBytes);
                    currentBlock.Add(decryptedByte);

                    // Copy the decrypted char to the list that holds all the decrypted blocks
                    decryptedBytes.Add(decryptedByte);
                }
            }
            
            // Return the decrypted bytes flattened as an ASCII string
            return decryptedBytes.ToArray().ToASCIIString();
        }

        private Dictionary<byte[], string> BuildMappingTable(int blockIndex, int blockSize, byte[] key, List<byte> currentBlock, List<byte> decryptedBytes)
        {
            int tableStart = 0;
            int tableEnd = 256;
            Dictionary<byte[], string> mappings = new Dictionary<byte[], string>();
            for (int i = tableStart; i < tableEnd; i++)
            {
                // First block: Build a block 1 byte short of a block, filled with As
                byte[] shortBlock = challenge9.PadBytes(null, blockSize, (byte)'A');

                // Add decrypted bytes to the short block for decrypting the final byte correctly (as AES decryption requires all preceding bytes be the same for the attack to work)
                int k = 1;
                for (int j = currentBlock.Count; j > 0; j--)
                {
                    shortBlock[shortBlock.Length - 1 - k++] = (byte)currentBlock[j - 1];
                }

                // Add a unique byte in the final byte position, this is what we will use to find what the decrypted character is
                shortBlock[shortBlock.Length - 1] = (byte)i;

                // Get the previously decrypted blocks and put insert before our current block
                if(decryptedBytes.Count >= 16)
                {
                    byte[] newShortBlock = new byte[shortBlock.Length + decryptedBytes.Count];
                    Array.Copy(decryptedBytes.ToArray(), 0, newShortBlock, 0, decryptedBytes.Count);

                    // Put current block at the end
                    Array.Copy(shortBlock, 0, newShortBlock, decryptedBytes.Count, shortBlock.Length);
                    shortBlock = newShortBlock;
                }

                // Get the short block as plain text (for the first block this will be decrypted text, subsequent blocks, encrypted)
                string plainText = shortBlock.ToASCIIString();

                // Encrypt the block
                shortBlock = Oracle(true, plainText, key);

                // Get the bytes to the correct block size
                byte[] outputBlock = new byte[blockSize];
                Array.Copy(shortBlock, blockIndex * blockSize, outputBlock, 0, outputBlock.Length);

                // Add it to the dictionary (the encrypted bytes as the key, and the plaintext as the value)
                mappings.Add(outputBlock, plainText);
            }

            return mappings;
        }

        // cdg todo 
        // comment the methods in this class
        // Decrypt N blocks and not just first block of unknown string

        // refactor previous challenges to use extension methods
        // move cryptographic methods (AES and CBC encrypt etc. to Cryptography class in Utilities)
        private byte DecryptUnknownByte(int blockIndex, int blockSize, byte[] key, List<byte> currentBlock, List<byte> decryptedBytes)
        {
            // Construct a short block to grab more and more of the unknown characters
            byte[] bytesToCheck;

            // 1 byte short of a block size
            byte[] shortBlock = new byte[blockSize - 1];

            // Fill block with As with 1 missing byte at the end
            shortBlock = challenge9.PadBytes(null, blockSize - 1 - currentBlock.Count, (byte)'A');

            // Get the previously decrypted blocks and put insert before our current block
            if (decryptedBytes.Count >= 16)
            {
                byte[] newShortBlock = new byte[shortBlock.Length + decryptedBytes.Count];
                Array.Copy(decryptedBytes.ToArray(), 0, newShortBlock, 0, decryptedBytes.Count);

                // Put current block at the end
                Array.Copy(shortBlock, 0, newShortBlock, decryptedBytes.Count, shortBlock.Length);
                shortBlock = newShortBlock;
            }

            // Encrypt block
            string targetPlainText = shortBlock.ToASCIIString();
            byte[] encryptedBytes = Oracle(true, targetPlainText, key);

            // Get the bytes to check (as the Oracle will return many blocks depending on the length of 'my string' and 'unknown string')
            bytesToCheck = new byte[blockSize];
            Array.Copy(encryptedBytes, blockIndex * blockSize, bytesToCheck, 0, blockSize);

            // Build dictionary used to hold all possible byte combinations for the missing byte (e.g. 4 size blocks "AAAA", "AAAB", "AAAC")
            Dictionary<byte[], string> mappings = BuildMappingTable(blockIndex, blockSize, key, currentBlock, decryptedBytes);

            // Get the match from the dictionary
            KeyValuePair<byte[], string> match = mappings.FirstOrDefault(x => x.Key.SequenceEqual(bytesToCheck));

            // Get the match key as a character (the final character of the plaintext in the match)
            return (byte)match.Value[match.Value.Length - 1];
        }
    }
}

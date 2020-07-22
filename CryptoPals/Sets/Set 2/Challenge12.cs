using CryptoPals.Enumerations;
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
        IChallenge9 challenge9   = (IChallenge9)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge9);
        IChallenge11 challenge11 = (IChallenge11)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge11);

        /// <inheritdoc />
        public string Solve(string input)
        {
            // Generate a random key
            int keySize = 16;
            //byte[] key = challenge11.GenerateRandomASCIIBytes(keySize);
            byte[] key = Encoding.ASCII.GetBytes("YELLOW SUBMARINE"); // cdg debug use same key
            input = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"; // cdg debug

            // Convert input to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            //bytes = challenge9.PadBytes(bytes, key.Length);

            // The text to append after the input bytes
            string base64Text = "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK";

            // Base64 decode the text
            byte[] base64Bytes = Convert.FromBase64String(base64Text);

            // Pad the Base64 bytes as a multiple of the key size as the encryptor cuts off text if not a multiple of the block size (just use key size (16), as the block size is 128 bits by default)
            //base64Bytes = challenge9.PadBytes(base64Bytes, key.Length);

            // Append additional bytes after the input bytes
            byte[] appendedBytes = challenge11.InsertBytes(bytes, base64Bytes, false);
            //appendedBytes = challenge9.PadBytes(base64Bytes, key.Length); // cdg debug pad appendeded bytes to multiple of block size so we don't lose data

            // Encrypt the bytes with the key
            byte[] encryptedBytes = challenge7.AES_ECB(true, appendedBytes, key);

            // The repeated character we are going to use for decryption
            char character = 'A';

            // Detect the block size of the cipher
            //int blockSize = DetermineEncryptorBlockSize(); // Generic
            int blockSize = DetermineBouncyCastleEncryptorBlockSize(); // Bouncy Castle proof of concept

            // Detect if the encryptor is using ECB
            bool isUsingECB = IsEncryptorUsingECB(blockSize);

            string output = "";
            if(isUsingECB)
            {
                // Decrypt the unknown bytes
                output = DecryptUnknownBytes(encryptedBytes, blockSize, character, key);
            }

            return output;
        }

        private bool IsEncryptorUsingECB(int blockSize)
        {
            // Construct 2 blocks worth of identical data
            string text = "".PadRight(blockSize * 2, 'A');

            // Convert to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(text);

            // Encrypt the data (just use a blank key)
            byte[] encrypted = challenge7.AES_ECB(true, bytes, new byte[blockSize]);

            // Check if the encrption contains 2 indentical blocks
            return challenge8.IsECBEncrypted(encrypted);
        }

        private Dictionary<byte[], string> BuildMappingTable(byte[] block, char character, byte[] key)
        {
            int size = 256;
            Dictionary<byte[], string> mappings = new Dictionary<byte[], string>();
            for (int i = 0; i < size; i++)
            {
                // Build a block with a unique byte in the final byte position
                //byte[] block = challenge9.PadBytes(new byte[0], blockSize, (byte)character);
                block[block.Length - 1] = (byte)i;
                string plainText = Encoding.ASCII.GetString(block);

                // Encrypt the block
                byte[] encrypt = challenge7.AES_ECB(true, block, key);

                // Add it to the dictionary (the encrypted bytes as the key, and the plaintext as the value)
                mappings.Add(encrypt, plainText);
            }

            return mappings;
        }

        
        private int DetermineBouncyCastleEncryptorBlockSize()
        {
            // Just use a 128 bit blank key
            byte[] key = new byte[16];

            // Due to how BouncyCastle works, simply attempting to encrypt text 
            // until we get back a non null, we can find the block size...
            byte[] encryption = null;
            int byteCount = 1;
            int blockSize = byteCount;
            while(encryption == null)
            {
                encryption = challenge7.AES_ECB(true, challenge9.PadBytes(new byte[0], byteCount), key);
                blockSize = byteCount++;
            }

            return blockSize;
        }

        private int DetermineEncryptorBlockSize()
        {
            // AES current maximum block size 128 bits(16 bytes)
            int maxBlockSize = 248;

            // Just use a 128 bit blank key
            byte[] key = new byte[16];

            // Do an initial encrypt on blank text
            char c = 'A'; // A char to use to fill the blocks
            string text = "".PadRight(16, c);
            byte[] initialEcrypt = challenge7.AES_ECB(true, Encoding.ASCII.GetBytes(text), key);

            // Feed larger and larger sets of bytes to the encryptor until the size changes, and we have the block size
            int blockSize = 0;
            for (int i = 0; i <= maxBlockSize; i++)
            {
                // Add a character to the text to encrypt
                text += c;

                // Encrypt the text
                byte[] encrypted = challenge7.AES_ECB(true, Encoding.ASCII.GetBytes(text), key);

                // If the size has changed, the block size is the difference between the two
                if (encrypted.Length > initialEcrypt.Length)
                {
                    blockSize = encrypted.Length - initialEcrypt.Length;
                    break;
                }
            }

            return blockSize;
        }

        private string DecryptUnknownBytes(byte[] encryptedBytes, int blockSize, char character, byte[] key)
        {
            // CDG DEBUG
            string debugText = "YELLOW SUBMARINEYELLOW SUBMARINE";
            //unknownBytes = challenge7.AES_ECB(true, Encoding.ASCII.GetBytes(debugText), key);

            // Match the output of the short block to the dictionary key to get each character of the unknown string
            List<char> decryptedCharacters = new List<char>();
            int knownBytes = 0;
            for (int i = 0; i < encryptedBytes.Length - 1; i++)
            {
                // Build the block
                byte[] block = challenge9.PadBytes(new byte[0], blockSize, (byte)character);
                block[block.Length - 1] = encryptedBytes[i];

                // Need to grab the previous decrypted so its like "AAAAAAAA21" where 1 is the first encrypted, 2 is current until the end
                int startIndex = block.Length - 2;
                foreach(char c in decryptedCharacters)
                {
                    block[startIndex--] = (byte)c;
                }

                // Encrypt the short block
                byte[] target = challenge7.AES_ECB(true, block, key);

                // Build dictionary used to hold all possible byte combinations for the missing byte ("AAAAAAAA", "AAAAAAAB", "AAAAAAAC" etc.)
                Dictionary<byte[], string> mappings = BuildMappingTable(block, character, key);

                // Get the match from the dictionary
                KeyValuePair<byte[], string> match = mappings.FirstOrDefault(x => x.Key.SequenceEqual(target));

                // Get the match key as a character (the final character of the plaintext in the match)
                char decryptedCharacter = match.Value[blockSize - 1 - knownBytes];

                // Add it to the string builder
                decryptedCharacters.Add(decryptedCharacter);

                if (knownBytes <= blockSize)
                    // Increment the byte index
                    knownBytes++;
                else
                    // Reset the byte index
                    knownBytes = 0;
            }

            return decryptedCharacters.ToString();
        }
    }
}

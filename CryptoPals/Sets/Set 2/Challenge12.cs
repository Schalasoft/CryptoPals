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
        IChallenge9 challenge9 = (IChallenge9)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge9);
        IChallenge11 challenge11 = (IChallenge11)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge11);

        /// <inheritdoc />
        public string Solve(string input)
        {
            // Generate a random key
            int keySize = 16;
            //byte[] key = challenge11.GenerateRandomASCIIBytes(keySize);
            byte[] key = Encoding.ASCII.GetBytes("YELLOW SUBMARINE"); // cdg debug use same key

            // Convert input to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            bytes = challenge9.PadBytes(bytes, key.Length);

            // The text to append after the input bytes
            string base64Text = "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK";

            // Base64 decode the text
            byte[] base64Bytes = Convert.FromBase64String(base64Text);

            // Pad the Base64 bytes as a multiple of the key size as the encryptor cuts off text if not a multiple of the block size (just use key size (16), as the block size is 128 bits by default)
            base64Bytes = challenge9.PadBytes(base64Bytes, key.Length);

            // Append additional bytes after the input bytes
            byte[] appendedBytes = challenge11.InsertBytes(bytes, base64Bytes, false);

            // Encrypt the bytes with the key
            byte[] encryptedBytes = challenge7.AES_ECB(true, appendedBytes, key);

            // The repeated character we are going to use for decryption
            char character = 'A';

            // Detect the block size of the cipher
            int blockSize = DetermineEncryptorBlockSize(character);

            // Detect if the function is using ECB
            // We could just feed 2 blocks worth of identical data
            // to see if we get 2 blocks of identical encrypted
            // but might as well reuse the ECB encrypted bytes function
            bool isUsingECB = challenge8.IsECBEncrypted(encryptedBytes);

            string output = "";
            if(isUsingECB)
            {
                // Get the unknown bytes from the combined bytes
                byte[] unknownBytes = GetUnknownBytes(bytes, encryptedBytes, key);

                // Decrypt the unknown bytes
                output = DecryptUnknownBytes(unknownBytes, blockSize, character, key);
            }

            return output;
        }

        private Dictionary<byte[], string> BuildMappingTable(int blockSize, char character, byte[] key)
        {
            int size = 256;
            Dictionary<byte[], string> mappings = new Dictionary<byte[], string>();
            for (int i = 0; i < size; i++)
            {
                // Build a block with a unique byte in the final byte position
                byte[] block = challenge9.PadBytes(new byte[0], blockSize, (byte)character);
                block[block.Length - 1] = (byte)i;
                string plainText = Encoding.ASCII.GetString(block);

                // Encrypt the block
                byte[] encrypt = challenge7.AES_ECB(true, block, key);

                // Add it to the dictionary (the encrypted bytes as the key, and the plaintext as the value)
                mappings.Add(encrypt, plainText);
            }

            return mappings;
        }

        private int DetermineEncryptorBlockSize(char character)
        {
            // AES current maximum block size 128 bits(16 bytes)
            int maxBlockSize = 248;

            // Just use a 128 bit blank key
            byte[] key = new byte[16]; 

            // Do an initial encrypt on blank text
            string text = "".PadRight(16, character);
            byte[] initialEcrypt = challenge7.AES_ECB(true, Encoding.ASCII.GetBytes(text), key);

            // Feed larger and larger sets of bytes to the encryptor until the size changes, and we have the block size
            int blockSize = 0;
            for (int i = 0; i <= maxBlockSize; i++)
            {
                // Add a character to the text to encrypt
                text += character;

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

        private byte[] GetUnknownBytes(byte[] bytes, byte[] encryptedBytes, byte[] key)
        {
            // Find the unknown text bytes by finding out how long our string encrypts to
            int knownBytesLength = challenge7.AES_ECB(true, bytes, key).Length;
            int unknownBytesLength = encryptedBytes.Length - knownBytesLength;
            byte[] unknownBytes = new byte[unknownBytesLength];
            Array.Copy(encryptedBytes, knownBytesLength, unknownBytes, 0, unknownBytesLength);

            return unknownBytes;
        }

        private string DecryptUnknownBytes(byte[] unknownBytes, int blockSize, char character, byte[] key)
        {
            // CDG DEBUG
            string debugText = "YELLOW SUBMARINEYELLOW SUBMARINE";
            unknownBytes = challenge7.AES_ECB(true, Encoding.ASCII.GetBytes(debugText), key);

            // Match the output of the short block to the dictionary key to get each character of the unknown string
            StringBuilder stringBuilder = new StringBuilder();
            int knownBytes = 0;
            for (int i = 0; i < unknownBytes.Length - 1; i++)
            {
                // Get a reference to the current encrypted byte
                byte encryptedByte = unknownBytes[i];

                // Build the block
                byte[] block = challenge9.PadBytes(new byte[0], blockSize, (byte)character);
                block[block.Length - 1] = encryptedByte;

                // Encrypt the short block
                byte[] target = challenge7.AES_ECB(true, block, key);

                // Build dictionary used to hold all possible byte combinations for the missing byte ("AAAAAAAA", "AAAAAAAB", "AAAAAAAC" etc.)
                Dictionary<byte[], string> mappings = BuildMappingTable(blockSize, character, key);

                // Get the match from the dictionary
                KeyValuePair<byte[], string> match = mappings.FirstOrDefault(x => x.Key.SequenceEqual(target));

                // Get the match key as a character (the final character of the plaintext in the match)
                char decryptedCharacter = match.Value[blockSize - 1 - knownBytes];

                // Add it to the string builder
                stringBuilder.Append(decryptedCharacter);

                if (knownBytes <= blockSize)
                    // Increment the byte index
                    knownBytes++;
                else
                    // Reset the byte index
                    knownBytes = 0;
            }

            return stringBuilder.ToString();
        }
    }
}

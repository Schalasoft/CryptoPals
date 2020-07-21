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
            // Convert input to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(input);

            // The text to append after the input bytes
            string base64Text = "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK";

            // Base64 decode the text
            byte[] base64Bytes = Convert.FromBase64String(base64Text);

            // Generate a random key
            int keySize = 16;
            //byte[] key = challenge11.GenerateRandomASCIIBytes(keySize);
            byte[] key = Encoding.ASCII.GetBytes("YELLOW SUBMARINE"); // cdg debug use same key

            // CDG DEBUG
            // Pad the Base64 bytes as a multiple of the key size so we don't lose any data
            base64Bytes = challenge9.PadBytes(base64Bytes, key.Length);

            // Append additional bytes after the input bytes
            byte[] appendedBytes = challenge11.InsertBytes(bytes, base64Bytes, false);

            // Pad the bytes as a multiple of the key size so we don't lose any data
            appendedBytes = challenge9.PadBytes(appendedBytes, key.Length);

            // Encrypt the bytes with the key
            byte[] encryptedBytes = challenge7.AES_ECB(true, appendedBytes, key);

            // Encrypt the unknown string for decrypting
            byte[] encryptedUnknownBytes = challenge7.AES_ECB(true, base64Bytes, key);

            // Decrypt the entire text and find the unknown string, just output the last 500 characters of the data
            return DecryptUnknownString(encryptedBytes, encryptedUnknownBytes, key);//.Substring(encryptedBytes.Length - 500, 500); // cdg todo
        }

        private Dictionary<byte[], string> BuildMappingTable(int blockSize, char character, byte[] key)
        {
            int size = 256;
            Dictionary<byte[], string> mappings = new Dictionary<byte[], string>();
            for (int i = 0; i < size; i++)
            {
                // Build a block with a unique byte in the final byte position
                byte[] block = BuildBlock(Convert.ToChar(i), blockSize, character);

                // Get the plaintext
                string plainText = Encoding.ASCII.GetString(block);

                // Encrypt the short block
                byte[] encrypt = EncryptShortBlock(Convert.ToChar(i), blockSize, character, key);

                // Add it to the dictionary (the encrypted bytes as the key, and the plaintext as the value)
                mappings.Add(encrypt, plainText);
            }

            return mappings;
        }

        private byte[] BuildBlock(char lastCharacter, int blockSize, char character)
        {
            // Create a block exactly 1 byte short of the block size and add the last unique character
            return Encoding.ASCII.GetBytes($"{"".PadRight(blockSize - 1, character)}{lastCharacter}");
        }


        private int DetermineEncryptorBlockSize(char character)
        {
            // AES current maximum block size 128 bits(16 bytes)
            int maxBlockSize = 16;

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
                    blockSize = encrypted.Length - initialEcrypt.Length;
            }

            return blockSize;
        }

        private string DecryptUnknownString(byte[] bytes, byte[] unknownBytes, byte[] key)
        {
            // The character we are going to use for encryption
            char character = 'A';

            // Detect the block size of the cipher
            int blockSize = DetermineEncryptorBlockSize(character);

            // Detect if the function is using ECB
            // We could just feed 2 blocks worth of identical data
            // to see if we get 2 blocks of identical encrypted
            // but might as well reuse the ECB encrypted bytes function
            bool isUsingECB = challenge8.IsECBEncrypted(bytes);

            // This will only work if the encryption is ECB
            string output = "";
            if (isUsingECB)
            {
                // Output the, now known, string
                output = GetUnknownString(unknownBytes, blockSize, character, key);
            }

            return output;
        }

        private byte[] EncryptShortBlock(char unknownCharacter, int blockSize, char character, byte[] key)
        {
            // Create a block with all the same character, except the last byte which will be unique
            byte[] blockBytes = BuildBlock(unknownCharacter, blockSize, character);

            // Encrypt the short block
            return challenge7.AES_ECB(true, blockBytes, key);
        }

        private string GetUnknownString(byte[] unknownBytes, int blockSize, char character, byte[] key)
        {
            blockSize = 32;
            // Match the output of the short block to the dictionary key to get each character of the unknown string
            byte[] decryptedBytes = new byte[unknownBytes.Length];
            for (int i = 0; i < unknownBytes.Length; i++)
            {
                // Get a reference to the current encrypted byte
                byte encryptedByte = unknownBytes[i];

                // Build a block (1 byte short of the block size)
                // It is -2 for my function as I re-use it to add a final character
                byte[] shortBlock = BuildBlock(character, blockSize - 2, character);

                // Encrypt the short block
                // cdg todo a short block returns null in this encryptor.... what to do
                byte[] target = challenge7.AES_ECB(true, shortBlock, key);

                // Build dictionary used to hold all possible byte combinations for the missing byte ("AAAAAAAA", "AAAAAAAB", "AAAAAAAC" etc.)
                Dictionary<byte[], string> mappings = BuildMappingTable(blockSize, character, key);

                // Get the match from the dictionary
                KeyValuePair<byte[], string> match = mappings.FirstOrDefault(x => x.Key.SequenceEqual(target));

                // Get the match key as a character (the final character of the plaintext in the match)
                char decryptedCharacter = match.Value[match.Value.Length - 1];
            }

            return Encoding.ASCII.GetString(decryptedBytes);
        }
    }
}

using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using CryptoPals.Extension_Methods;
using static CryptoPals.Utilities.Cryptography;

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
            byte[] key = "YELLOW SUBMARINE".ToBytes(); // cdg debug use same key
            input = "ABCDEFGHIJKLMNOP"; // cdg debug use the alphabet as 'my string' text

            // Convert input to bytes
            byte[] bytes = input.ToBytes();

            // The text to append after the input bytes
            string base64Text = "Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK";

            // Base64 decode the text
            byte[] base64Bytes = Convert.FromBase64String(base64Text);

            // Detect the block size of the cipher
            int blockSize = DetermineEncryptorBlockSize();

            // Detect if the encryptor is using ECB
            bool isUsingECB = IsEncryptorUsingECB(blockSize);

            string output = "";
            if(isUsingECB)
            {
                // Append additional bytes after the input bytes
                byte[] appendedBytes = challenge11.InsertBytes(bytes, base64Bytes, false);

                // Pad the bytes so it is a multiple of the block size
                appendedBytes = PadBytesToBlockSizeMultiple(appendedBytes, blockSize);

                // Encrypt the bytes with the key
                byte[] encryptedBytes = Oracle(true, appendedBytes.ToASCIIString(), key);

                // Decrypt the unknown bytes
                output = DecryptUnknownBytes(bytes, encryptedBytes, blockSize, key);
            }

            return output;
        }

        private byte[] PadBytesToBlockSizeMultiple(byte[] bytes, int blockSize, byte paddingByte = 4)
        {
            // Determine the new size (the size of the bytes plus how many bytes we are missing, which is the block size minus the modulus remainder of the length of the bytes and the block size)
            int newSize = bytes.Length + ((blockSize) - bytes.Length % blockSize);

            // Create resized byte array
            byte[] resizedBytes = challenge9.PadBytes(bytes, newSize, paddingByte);

            return resizedBytes;
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

        // Proof of concept
        private int DetermineBouncyCastleEncryptorBlockSize()
        {
            // Just use a 128 bit blank key
            byte[] key = new byte[16];

            // Due to how BouncyCastle works, simply attempting to encrypt text 
            // until we get back a non null, we can find the block size
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
            int maxBlockSize = 2000;

            // Just use a 128 bit blank key
            byte[] key = new byte[16];

            // Do an initial encrypt on blank text
            char c = 'A'; // A char to use to fill the blocks
            string text = "".PadRight(16, c);
            byte[] initialEcrypt = Oracle(true, text, key);

            // Feed larger and larger sets of bytes to the encryptor until the size changes, and we have the block size
            int blockSize = 0;
            for (int i = 0; i <= maxBlockSize; i++)
            {
                // Add a character to the text to encrypt
                text += c;

                // Encrypt the text
                byte[] encrypted = Oracle(true, text, key, 0);

                // If the size has changed, the block size is the difference between the two
                if (encrypted.Length > initialEcrypt.Length)
                {
                    blockSize = encrypted.Length - initialEcrypt.Length;
                    break;
                }
            }

            return blockSize;
        }

        // Thought I would try .Net AES Managed, turns out they both return null for text below the block size
        public byte[] Oracle(bool encrypt, string text, byte[] key, int crypto = 0, byte[] iv = null, CipherMode mode = CipherMode.ECB)
        {
            // crypto input variable meaning
            // 0 : Bouncy Castle
            // 1 : .Net AES Managed
            // 2 : AesCryptoServiceProvider

            byte[] output;
            switch (crypto)
            {
                case 0:
                    output = challenge7.AES_ECB(encrypt, text.ToBytes(), key);
                    break;

                case 1:
                    using (AesManaged aes = new AesManaged())
                    {
                        // Set the mode
                        aes.Mode = mode;

                        if (encrypt)
                        {
                            ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                                {
                                    using (StreamWriter sw = new StreamWriter(cs))
                                    {
                                        sw.Write(text);
                                        output = ms.ToArray();
                                    }
                                }
                            }
                        }
                        else
                        {
                            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
                            using (MemoryStream ms = new MemoryStream(text.ToBytes()))
                            {
                                // Create crypto stream    
                                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                                {
                                    // Read crypto stream    
                                    using (StreamReader reader = new StreamReader(cs))
                                    {
                                        output = reader.ReadToEnd().ToBytes();
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 2:
                    output = AES_128_ECB_Encrypt(text.ToBytes(), key);
                    break;

                default:
                    output = new byte[0];
                    break;
            }

            return output;
        }

        private Dictionary<byte[], string> BuildMappingTable(byte[] block, byte[] key)
        {
            int tableStart = 0;
            int tableEnd = 256;
            Dictionary<byte[], string> mappings = new Dictionary<byte[], string>();
            for (int i = tableStart; i < tableEnd; i++)
            {
                // Build a block with a unique byte in the final byte position (using the values in our passed in block to decrypt the whole block
                block[block.Length - 1] = (byte)i;
                string plainText = block.ToASCIIString();

                // Encrypt the block
                byte[] encrypt = Oracle(true, plainText, key);

                // Add it to the dictionary (the encrypted bytes as the key, and the plaintext as the value)
                mappings.Add(encrypt, plainText);
            }

            return mappings;
        }

        // cdg todo refactor previous challenges to use extension methods
        private string DecryptUnknownBytes(byte[] knownBytes, byte[] encryptedBytes, int blockSize, byte[] key)
        {
            // Match the output of the short block to the dictionary key to get each character of the unknown string
            List<char> decryptedCharacters = new List<char>();
            List<char> decryptedBlock = new List<char>();
            for (int i = 0; i < encryptedBytes.Length - 1; i++)
            {
                // Reset the decrypted block and store the result everytime we decrypt an entire block
                if (i % (blockSize) == 0)
                {
                    decryptedCharacters.AddRange(decryptedBlock);
                    decryptedBlock = new List<char>();
                }

                // Build the block
                byte[] block = challenge9.PadBytes(new byte[0], blockSize, (byte)'A');
                block[block.Length - 1] = (byte)knownBytes[knownBytes.Length - 1 - decryptedBlock.Count];

                // Need to grab the previous decrypted so its like "AAAAAAAA21" where 1 is the first encrypted, 2 is 2nd until the end of our decrypted characters
                int startIndex = block.Length - 2;
                foreach(char c in decryptedBlock)
                {
                    block[startIndex--] = (byte)c;
                }

                string s = block.ToASCIIString();

                // Encrypt the short block
                string targetPlainText = block.ToASCIIString();
                byte[] target = Oracle(true, targetPlainText, key);
                string targetCipherText = target.ToASCIIString();

                // Build dictionary used to hold all possible byte combinations for the missing byte ("AAAAAAAA", "AAAAAAAB", "AAAAAAAC" etc.)
                Dictionary<byte[], string> mappings = BuildMappingTable(block, key);

                // Get the match from the dictionary
                KeyValuePair<byte[], string> match = mappings.FirstOrDefault(x => x.Key.SequenceEqual(target));

                // Get the match key as a character (the final character of the plaintext in the match)
                char decryptedCharacter = match.Value[blockSize - 1];

                // Add it to the string builder
                decryptedBlock.Add(decryptedCharacter);
            }

            decryptedCharacters.Reverse();

            return new string(decryptedCharacters.ToArray());
        }

    }
}

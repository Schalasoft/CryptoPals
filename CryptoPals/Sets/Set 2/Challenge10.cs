using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Text;

namespace CryptoPals.Sets
{
    ///<inheritdoc cref="IChallenge10"/>
    class Challenge10 : IChallenge10, IChallenge
    {
        /*
        Implement CBC mode
        CBC mode is a block cipher mode that allows us to encrypt irregularly-sized messages,
        despite the fact that a block cipher natively only transforms individual blocks.

        In CBC mode, each ciphertext block is added to the next plaintext block before the next call to the cipher core.

        The first plaintext block, which has no associated previous ciphertext block, 
        is added to a "fake 0th ciphertext block" called the initialization vector, or IV.

        Implement CBC mode by hand by taking the ECB function you wrote earlier, 
        making it encrypt instead of decrypt (verify this by decrypting whatever you encrypt to test), 
        and using your XOR function from the previous exercise to combine them.

        The file here is intelligible (somewhat) when CBC decrypted against "YELLOW SUBMARINE" 
        with an IV of all ASCII 0 (\x00\x00\x00 &c)

        Don't cheat.
        Do not use OpenSSL's CBC code to do CBC mode, even to verify your results. 
        What's the point of even doing this stuff if you aren't going to learn from it?
        */

        // Reuse previous challenge functionality
        IChallenge2 challenge2 = (IChallenge2)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge2);
        IChallenge6 challenge6 = (IChallenge6)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge6);
        IChallenge7 challenge7 = (IChallenge7)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge7);
        IChallenge9 challenge9 = (IChallenge9)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge9);

        ///<inheritdoc />
        public string Solve(string input)
        {
            // Get input and key as bytes
            byte[] bytes = Convert.FromBase64String(input);
            byte[] key = Encoding.ASCII.GetBytes("YELLOW SUBMARINE");

            // Create Initialization Vector (a block the same size as the key/block but filled with ASCII 0 bytes)
            byte paddingByte = (byte)0x00;
            byte[] iv = challenge9.PadBytes(null, key.Length, paddingByte); // Could just let use a default byte array but this reads clearer

            // Decrypt
            byte[] decryptedBytes = AES_CBC(false, bytes, key, iv);

            // Convert decrypted bytes to string
            return Encoding.ASCII.GetString(decryptedBytes);
        }

        ///<inheritdoc cref="IChallenge10.AES_CBC(bool, byte[], byte[], byte[])"/>
        public byte[] AES_CBC(bool encrypt, byte[] bytes, byte[] key, byte[] iv)
        {
            // Encrypt & decrypt in the one function is a bit hard to read but reduces code duplication
            // If we are encrypting and the bytes are not divisible by the key, pad the bytes
            if (encrypt && bytes.Length % key.Length != 0)
               bytes = challenge9.PadBytes(bytes, key.Length);

            // Break input into blocks
            byte[][] blocks = challenge6.CreateBlocks(bytes, key.Length);

            // Iterate blocks, encrypting/decrypting the blocks in place
            byte[] previousBlock = iv;
            for (int i = 0; i < blocks.Length; i++)
            {
                // Get a reference to the current block
                byte[] block = new byte[key.Length];
                blocks[i].CopyTo(block, 0);

                // Replace unencrypted block with the encrypted block
                if (encrypt)
                {
                    // Encrypt
                    blocks[i] = AES_CBC_Encrypt(block, previousBlock, key);

                    // Update previous block
                    previousBlock = blocks[i];
                }
                else
                {
                    // Decrypt
                    blocks[i] = AES_CBC_Decrypt(block, previousBlock, key);

                    // Update previous block
                    previousBlock = block;
                }
            }

            // Return the blocks as a flattend array (2d to 1d)
            byte[] output = new byte[bytes.Length];
            int index = 0;
            foreach (byte[] byteArr in blocks)
                foreach (byte byt in byteArr)
                    output[index++] = byt;

            return output;
        }

        /// <summary>
        /// Encrypt a block using ECB
        /// </summary>
        /// <param name="block">The block to encrypt</param>
        /// <param name="previousBlock">The previous block for the encryption chain</param>
        /// <param name="key">The key to encrypt with</param>
        /// <returns>The block encrypted with the key and previous block</returns>
        private byte[] AES_CBC_Encrypt(byte[] block, byte[] previousBlock, byte[] key)
        {
            // XOR block against the previous block
            byte[] xorBlock = challenge2.XORByteArray(block, previousBlock);

            // ECB Encrypt
            return challenge7.AES_ECB(true, xorBlock, key);
        }

        /// <summary>
        /// Decrypt a block using ECB
        /// </summary>
        /// <param name="block">The block to decrypt</param>
        /// <param name="previousBlock">The previous block for the decryption chain</param>
        /// <param name="key">The key to decrypt with</param>
        /// <returns>The block decrypted with the key and previous block</returns>
        private byte[] AES_CBC_Decrypt(byte[] block, byte[] previousBlock, byte[] key)
        {
            // ECB Decrypt
            byte[] decryptedBlock = challenge7.AES_ECB(false, block, key);

            // XOR block against the previous block
            return challenge2.XORByteArray(decryptedBlock, previousBlock);
        }
    }
}

using CryptoPals.Factories;
using CryptoPals.Interfaces;
using System.Text;

namespace CryptoPals.Sets
{
    class Challenge10 : IChallenge
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
        IChallenge2 challenge2 = (IChallenge2)ChallengeFactory.InitializeChallenge(Enumerations.ChallengeEnum.Challenge2);
        IChallenge6 challenge6 = (IChallenge6)ChallengeFactory.InitializeChallenge(Enumerations.ChallengeEnum.Challenge6);
        IChallenge7 challenge7 = (IChallenge7)ChallengeFactory.InitializeChallenge(Enumerations.ChallengeEnum.Challenge7);
        IChallenge9 challenge9 = (IChallenge9)ChallengeFactory.InitializeChallenge(Enumerations.ChallengeEnum.Challenge9);

        // The size of the cipher blocks
        int blockSize = 16;

        public string Solve(string input)
        {
            // Get input and key as bytes
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            byte[] key = Encoding.ASCII.GetBytes("YELLOW SUBMARINE");

            // Create Initialization Vector (a block the same size as the cipher blocks but filled with ASCII 0 bytes)
            byte paddingByte = (byte)0x00;
            byte[] iv = challenge9.PadBlock(new byte[0], paddingByte, blockSize);

            // Decrypt
            byte[] decryptedBytes = AES_CBC(bytes, key, iv);

            // Convert decrypted bytes to string
            string output = Encoding.ASCII.GetString(decryptedBytes);

            return output;
        }

        // Decrypt using AES CBC (Advanced Encryption Standard Cipher Block Chaining Mode)
        private byte[] AES_CBC(byte[] bytes, byte[] key, byte[] iv)
        {
            // Break input into blocks
            byte[][] blocks = challenge6.CreateBlocks(bytes, blockSize);

            // Iterate blocks
            byte[] xor = new byte[bytes.Length];
            for (int i = 0; i < blocks.Length; i++)
            {
                // First block is XORed against the Initialization Vector
                if (i == 0)
                {
                    challenge2.FixedXOR(iv, blocks[i]).CopyTo(xor, i);
                }
                else // XOR current block against the previous block
                {
                    challenge2.FixedXOR(blocks[i-1], blocks[i]).CopyTo(xor, i * blockSize);
                }
            }

            return xor;
        }
    }
}

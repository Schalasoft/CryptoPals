using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections;
using System.Text;

namespace CryptoPals.Sets
{
    class Challenge11 : IChallenge
    {
        /*
        An ECB/CBC detection oracle
        Now that you have ECB and CBC working:

        Write a function to generate a random AES key; that's just 16 random bytes.

        Write a function that encrypts data under an unknown key --- that is, a function that generates a random key and encrypts under it.

        The function should look like:

        encryption_oracle(your-input)
        => [MEANINGLESS JIBBER JABBER]
        Under the hood, have the function append 5-10 bytes (count chosen randomly) before the plaintext and 5-10 bytes after the plaintext.

        Now, have the function choose to encrypt under ECB 1/2 the time, and under CBC the other half (just use random IVs each time for CBC). 
        Use rand(2) to decide which to use.

        Detect the block cipher mode the function is using each time. 
        You should end up with a piece of code that, pointed at a block box that might be encrypting ECB or CBC, tells you which one is happening.
        */

        // Create a random number generator (Random uses Environment.TickCount under the hood as the seed, this should be sufficiently random)
        private readonly Random random = new Random();

        // Reuse previous challenge functionality
        IChallenge6 challenge6   = (IChallenge6)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge6);
        IChallenge7 challenge7   = (IChallenge7)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge7);
        IChallenge9 challenge9   = (IChallenge9)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge9);
        IChallenge10 challenge10 = (IChallenge10)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge10);

        public string Solve(string input)
        {
            // Convert input string to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(input);

            // Encrypt data
            int keyLength = 16;
            byte[] encryptedBytes = EncryptWithUnknownKey(bytes, keyLength);

            // Break encrypted bytes into blocks the size of the key
            byte[][] blocks = challenge6.CreateBlocks(encryptedBytes, keyLength);

            // Determine encryption type that has been used on each block
            string[] types = DetectBlocksEncryptionType(blocks);

            // Get the formatted output
            string output = FormatOutput(types);

            return output;
        }

        // Detects the encryption type for a block of bytes
        private EncryptionTypeEnum DetectBlockEncryptionType(byte[] block)
        {
            // Determine encryption type being used
            EncryptionTypeEnum type = 0;

            return type;
        }

        // Create an array containing the encryption type of each block
        private string[] DetectBlocksEncryptionType(byte[][] blocks)
        {
            string[] types = new string[blocks.Length];
            for (int i = 0; i < blocks.Length; i++)
            {
                // Determine the block encryption type and return it as an enum
                types[i] = DetectBlockEncryptionType(blocks[i]).ToString();
            }

            return types;
        }

        // Encrypt bytes using ECB and CBC randomly for each block
        private byte[] EncryptBytesRandomly(byte[] bytes, int blockLength, byte[] key)
        {
            // Encrypt blocks with ECB or CBC randomly
            byte[] encryptedBytes = new byte[bytes.Length];
            for (int i = 0; i < encryptedBytes.Length / blockLength; i++)
            {
                // Get the unencrypted block
                byte[] unencryptedBlock = (byte[])new ArrayList(bytes).GetRange(i * blockLength, blockLength).ToArray(typeof(byte));

                // Encrypt the block
                byte[] encryptedBlock = unencryptedBlock;
                EncryptionTypeEnum encryptionType = (EncryptionTypeEnum)random.Next(1, 2);
                if (encryptionType.Equals(EncryptionTypeEnum.ECB))
                {
                    // Encrypt using ECB
                    encryptedBlock = challenge7.AES_ECB(true, unencryptedBlock, key);
                }
                else if (encryptionType.Equals(EncryptionTypeEnum.CBC))
                {
                    // Create an initialization vector (use random IV)
                    byte[] iv = GenerateRandomASCIIBytes(blockLength);

                    // Encrypt using CBC
                    encryptedBlock = challenge10.AES_CBC(true, unencryptedBlock, key, iv);
                }

                // Add the encrypted block to the encrytedBytes array for returning
                for (int j = 0; j < encryptedBlock.Length; j++)
                {
                    encryptedBytes[j + (i * blockLength)] = encryptedBlock[j];
                }
            }

            return encryptedBytes;
        }

        // Encrypts data under a randomly generated key, randomly using ECB or CBC for each block
        private byte[] EncryptWithUnknownKey(byte[] bytes, int keyLength)
        {
            // Create byte arrays containing 5-10 random bytes for inserting & after before the bytes to encrypt
            int insertBeforeCount = random.Next(5, 10);
            int insertAfterCount = random.Next(5, 10);
            byte[] beforeBytes = GenerateRandomASCIIBytes(insertBeforeCount);
            byte[] afterBytes = GenerateRandomASCIIBytes(insertAfterCount);

            // Construct byte array with the original bytes after adding the additional bytes
            byte[] bytesWithInserts;
            bytesWithInserts = InsertBytes(bytes, beforeBytes, true); // Append before
            bytesWithInserts = InsertBytes(bytes, afterBytes, false); // Append after

            // Pad the constructed byte array if it is not divisible by the key length
            if (bytesWithInserts.Length % keyLength != 0)
                bytesWithInserts = challenge9.PadBytes(bytesWithInserts, keyLength);

            // Create a random key
            byte[] key = GenerateRandomASCIIBytes(keyLength);

            // Encrypt the data where each block has been randomly encrypted with ECB or CBC
            byte[] encryptedBytes = EncryptBytesRandomly(bytesWithInserts, keyLength, key);

            return encryptedBytes;
        }

        // Format the output
        private string FormatOutput(string[] types)
        {
            // Build the output string from the types array
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < types.Length - 5; i++)
            {
                // Whitespace padding for output uniformity: will work up until 2 digit indexes (blocks in the 3 digits will be truncated from the output anyway)
                stringBuilder.Append($"Block {i,2} has been encrypted with: {types[i].ToString()}{Environment.NewLine}");
            }

            return stringBuilder.ToString();
        }

        // Generates a random key of the specified size
        private byte[] GenerateRandomASCIIBytes(int length)
        {
            // Create byte array to store encrypted key
            byte[] key = new byte[length];

            // Assign each part of the key to a random byte value (0-256 for full ASCII range)
            int min = 0;
            int max = 256;
            for(int i = 0; i < length; i++)
            {
                key[i] = (byte)random.Next(min, max);
            }
                
            return key;
        }

        // Create a byte array with the bytes to insert put before or after the original set of bytes
        private byte[] InsertBytes(byte[] bytesOriginal, byte[] bytesToInsert, bool insertBefore)
        {
            byte[] bytesWithInserts = new byte[bytesToInsert.Length + bytesOriginal.Length];

            if (insertBefore)
            {
                // Insert bytes before the original bytes
                bytesToInsert.CopyTo(bytesWithInserts, 0);
                bytesOriginal.CopyTo(bytesWithInserts, bytesToInsert.Length);
            }
            else
            {
                // Insert bytes after the original bytes
                bytesOriginal.CopyTo(bytesWithInserts, 0);
                bytesToInsert.CopyTo(bytesWithInserts, bytesOriginal.Length);
            }

            return bytesWithInserts;
        }
    }
}

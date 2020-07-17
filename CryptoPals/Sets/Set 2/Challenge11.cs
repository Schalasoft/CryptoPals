using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
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
            string[] types = new string[blocks.Length];
            for(int i = 0; i < blocks.Length; i++)
            {
                // Determine the block encryption type and return it as an enum
                EncryptionTypeEnum type = DetectBlockEncryptionType(encryptedBytes);

                // Store the type of encryption used for the block index
                types[i] = type.ToString();
            }

            // Build the output string from the types array
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < types.Length - 5; i++)
            {
                // Whitespace padding for output uniformity: will work up until 2 digit indexes (blocks in the 3 digits will be truncated from the output anyway)
                stringBuilder.Append($"Block {i,2} has been encrypted with: {types[i].ToString()}{Environment.NewLine}");
            }

            // Get the built string for output
            string output = stringBuilder.ToString();

            return output;
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

        // Encrypts data under a randomly generated key, randomly using ECB or CBC for each block
        private byte[] EncryptWithUnknownKey(byte[] bytes, int keyLength)
        {
            // Create byte arrays containing 5-10 random bytes for inserting & after before the bytes to encrypt
            int insertBeforeCount = random.Next(5, 10);
            int insertAfterCount = random.Next(5, 10);
            byte[] beforeBytes = GenerateRandomASCIIBytes(insertBeforeCount);
            byte[] afterBytes  = GenerateRandomASCIIBytes(insertAfterCount);

            // Populate byte array with the inserted before bytes, original bytes, and inserted after bytes
            byte[] bytesWithInserts = new byte[beforeBytes.Length + bytes.Length + afterBytes.Length];
            for(int i = 0; i < bytesWithInserts.Length; i++)
            {
                // Determine which byte to add
                byte byteToAdd;
                if (i < insertBeforeCount)
                    // Before byte
                    byteToAdd = beforeBytes[i]; // i is the same for both
                else if (i < insertBeforeCount + bytes.Length)
                    // Original byte
                    byteToAdd = bytes[i - insertBeforeCount]; // Normalize by subtracting the added before byte count
                else
                    // After byte
                    byteToAdd = afterBytes[i % (insertBeforeCount + bytes.Length)]; // Normalize by getting the remainder after the already added added bytes

                // Add the appropriate byte
                bytesWithInserts[i] = byteToAdd;
            }

            // Create a random key
            byte[] key = GenerateRandomASCIIBytes(keyLength);

            // Encrypt blocks with ECB or CBC randomly
            byte[] encryptedBytes = new byte[bytesWithInserts.Length];
            for (int i = 0; i < encryptedBytes.Length / keyLength; i++)
            {
                byte[] encryptedBlock = new byte[keyLength];
                int encryptionType = random.Next(1, 2);
                if (encryptionType.Equals(EncryptionTypeEnum.ECB))
                {
                    // Encrypt using ECB
                    encryptedBlock = challenge7.AES_ECB(true, bytesWithInserts, key);
                }
                else if (encryptionType.Equals(EncryptionTypeEnum.CBC))
                {
                    // Create an initialization vector (use random IV)
                    byte[] iv = GenerateRandomASCIIBytes(keyLength);

                    // Encrypt using CBC
                    encryptedBlock = challenge10.AES_CBC(true, bytesWithInserts, key, iv);
                }
                else
                {
                    // This is really unnecessary but we'll add it for posterity
                    // No valid encryption chosen, just output the unencrypted bytes
                    encryptedBlock = (byte[])new ArrayList(bytesWithInserts).GetRange(i * keyLength, keyLength).ToArray(typeof(byte));
                }

                // Add the encrypted block to the encrytedBytes array for returning
                for(int j = 0; j < encryptedBlock.Length; j++)
                {
                    encryptedBytes[j + (i * keyLength)] = encryptedBlock[j]; // CDG Missing last 12 bytes, something is wrong here
                }
            }

            return encryptedBytes;
        }

        // Detects the encryption type for a block of bytes
        private EncryptionTypeEnum DetectBlockEncryptionType(byte[] block)
        {
            // Determine encryption type being used
            EncryptionTypeEnum type = 0;

            return type;
        }
    }
}

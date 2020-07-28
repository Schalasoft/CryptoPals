using CryptoPals.Enumerations;
using CryptoPals.Extension_Methods;
using CryptoPals.Interfaces;
using CryptoPals.Managers;
using System;

namespace CryptoPals.Sets
{
    /// <inheritdoc cref="IChallenge11"/>
    class Challenge11 : IChallenge11, IChallenge
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

        // Variables to determine if the oracle is correct in its ECB/CBC detection

        // Reuse previous challenge functionality
        IChallenge7 challenge7   = (IChallenge7)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge7);
        IChallenge8 challenge8   = (IChallenge8)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge8);
        IChallenge9 challenge9   = (IChallenge9)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge9);
        IChallenge10 challenge10 = (IChallenge10)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge10);

        /// <inheritdoc />
        public string Solve(string input)
        {
            // Convert input string to bytes
            byte[] bytes = input.GetBytes();

            // Run Oracle against the input data multiple times (with different random byte keys)
            int totalTests = 50;
            int testFailCount = 0;
            for (int i = 0; i < totalTests; i++)
            {
                // Encrypt data using a random key
                int keySize = 16;
                Tuple<byte[], EncryptionTypeEnum> encryptionResult = EncryptWithUnknownKey(bytes, keySize);

                // Detect which encryption type was used
                EncryptionTypeEnum oracleEncryption = DetectEncryptionType(encryptionResult.Item1);
                
                // Get the actual encryption type that was used
                EncryptionTypeEnum actualEncryption = encryptionResult.Item2;

                // If the determination does not match, the test failed so increment the count
                if (!oracleEncryption.Equals(actualEncryption))
                {
                    testFailCount++;
                }
            }

            // Format the result of the tests and return it
            return FormatOutput(totalTests, testFailCount);
        }

        /// <summary>
        /// Return the type of encryption used on bytes
        /// </summary>
        /// <param name="bytes">The bytes to detect ECB/CBC on</param>
        /// <returns>The encryption type used as an enumeration</returns>
        private EncryptionTypeEnum DetectEncryptionType(byte[] bytes)
        {
            return challenge8.IsECBEncrypted(bytes) ? EncryptionTypeEnum.ECB : EncryptionTypeEnum.CBC; ;
        }

        /// <summary>
        /// Encrypt bytes using ECB and CBC randomly
        /// </summary>
        /// <param name="bytes">The bytes to encrypt</param>
        /// <param name="key">The key used to encrypt</param>
        /// <returns>A tuple containing the encrypted bytes and the encryption type used (ECB or CBC)</returns>
        private Tuple<byte[], EncryptionTypeEnum> EncryptBytesRandomly(byte[] bytes, byte[] key)
        {
            // Encrypt with ECB or CBC randomly
            byte[] encryptedBytes = new byte[bytes.Length];
            EncryptionTypeEnum encryptionType = (EncryptionTypeEnum)random.Next(1, 3);

            // Encrypt
            if (encryptionType.Equals(EncryptionTypeEnum.ECB))
            {
                // Encrypt using ECB
                encryptedBytes = Cryptography.AES_ECB(bytes, key);
            }
            else if (encryptionType.Equals(EncryptionTypeEnum.CBC))
            {
                // Create an initialization vector (use random IV)
                byte[] iv = GenerateRandomASCIIBytes(key.Length);

                // Encrypt using CBC
                encryptedBytes = challenge10.AES_CBC(true, bytes, key, iv);
            }

            return new Tuple<byte[], EncryptionTypeEnum>(encryptedBytes, encryptionType);
        }

        /// <summary>
        /// Encrypts data under a randomly generated key, randomly using ECB or CBC
        /// </summary>
        /// <param name="bytes">The bytes to encrypt</param>
        /// <param name="keyLength">The length of the key to populate with random bytes (0-256)</param>
        /// <returns>The bytes encrypted with an unknown key (ECB or CBC)</returns>
        private Tuple<byte[], EncryptionTypeEnum> EncryptWithUnknownKey(byte[] bytes, int keyLength)
        {
            // Insert 5-10 random bytes before and after the bytes
            byte[] bytesWithInserts = InsertRandomBytes(bytes);

            // Pad the constructed byte array if it is not divisible by the key length
            if (bytesWithInserts.Length % keyLength != 0)
                bytesWithInserts = challenge9.PadBytesToBlockSizeMultiple(bytesWithInserts, keyLength);

            // Create a random key
            byte[] key = GenerateRandomASCIIBytes(keyLength);

            // Encrypt the data with ECB or CBC
            return EncryptBytesRandomly(bytesWithInserts, key);
        }

        /// <summary>
        /// Format the output to succinctly display that all tests passed, or if any failed, how many
        /// </summary>
        /// <param name="totalTests">The total number of tests ran</param>
        /// <param name="testFailCount">The number of tests that failed</param>
        /// <returns>Formatted string with the total number of passed tests if all passed, or if any failed, then how many passed of all the tests run</returns>
        private string FormatOutput(int totalTests, int testFailCount)
        {
            string output = "";
            if (testFailCount == 0)
            {
                // If no tests failed, output that all N tests passed
                output = $"All {totalTests} tests passed.";
            }
            else
            {
                // If any test failed, output the number of passed tests vs the total
                output = $"{totalTests - testFailCount} tests passed of {totalTests}";
            }

            return output;
        }

        /// <inheritdoc cref="IChallenge11.GenerateRandomASCIIBytes(int)"/>
        public byte[] GenerateRandomASCIIBytes(int length)
        {
            // Create byte array to store encrypted key
            byte[] key = new byte[length];

            // Assign each part of the key to a random byte value (0-256 for full ASCII range)
            int min = 0;
            int max = 257;
            for(int i = 0; i < length; i++)
            {
                key[i] = (byte)random.Next(min, max);
            }
                
            return key;
        }

        /// <inheritdoc cref="IChallenge11.InsertBytes(byte[], byte[], bool)"/>
        public byte[] InsertBytes(byte[] bytesOriginal, byte[] bytesToInsert, bool insertBefore)
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

        /// <summary>
        /// Insert 5-10 random bytes before and after the bytes
        /// </summary>
        /// <param name="bytes">The original bytes</param>
        /// <returns>The original bytes with 5-10 random bytes inserted before and after them</returns>
        private byte[] InsertRandomBytes(byte[] bytes)
        {
            // Create byte arrays containing 5-10 random bytes for inserting & after before the bytes to encrypt
            int insertBeforeCount = random.Next(5, 11);
            int insertAfterCount = random.Next(5, 11);
            byte[] beforeBytes = GenerateRandomASCIIBytes(insertBeforeCount);
            byte[] afterBytes = GenerateRandomASCIIBytes(insertAfterCount);

            // Construct byte array with the original bytes after adding the additional bytes
            byte[] bytesWithInserts;
            bytesWithInserts = InsertBytes(bytes, beforeBytes, true); // Append before
            bytesWithInserts = InsertBytes(bytes, afterBytes, false); // Append after

            return bytesWithInserts;
        }
    }
}
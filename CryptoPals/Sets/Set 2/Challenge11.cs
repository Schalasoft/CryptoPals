using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            byte[] bytes = Encoding.ASCII.GetBytes(input);

            // Run Oracle against the input data multiple times (with different random byte keys)
            int testCount = 50;
            for (int i = 0; i < testCount; i++)
            {
                // Encrypt data using a random key
                int keySize = 16;
                Tuple<byte[], EncryptionTypeEnum> encryptionResult = EncryptWithUnknownKey(bytes, keySize);

                // Detect the Oracle to determine which encryption type was used
                EncryptionTypeEnum oracleEncryption = DetectEncryptionType(encryptionResult.Item1);
                
                // Get the actual encryption type that was used
                EncryptionTypeEnum actualEncryption = encryptionResult.Item2;

                // If the determination does not match, the test failed
                if (oracleEncryption.Equals(actualEncryption))
                    return "A test failed";
            }

            // If no tests failed, output that all N tests passed
            return "All {testCount} tests passed.";
        }

        /// <summary>
        /// Return the type of encryption used on bytes
        /// </summary>
        /// <param name="bytes">The bytes to detect ECB/CBC on</param>
        /// <returns>The encryption type used as an enumeration</returns>
        private EncryptionTypeEnum DetectEncryptionType(byte[] bytes)
        {
            // The amount of block sizes to try (will start at 2 so this trys 2 to 52)
            int blockSizeAttempts = 50;
            return challenge8.IsECBEncrypted(bytes, blockSizeAttempts) ? EncryptionTypeEnum.ECB : EncryptionTypeEnum.CBC; ;
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
                encryptedBytes = challenge7.AES_ECB(true, bytes, key);
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
                bytesWithInserts = challenge9.PadBytes(bytesWithInserts, keyLength);

            // Create a random key
            byte[] key = GenerateRandomASCIIBytes(keyLength);

            // Encrypt the data with ECB or CBC
            return EncryptBytesRandomly(bytesWithInserts, key);
        }

        /// <summary>
        /// Generates a random key of the specified size
        /// </summary>
        /// <param name="length">The size of the byte array to create</param>
        /// <returns>A byte array of the specified size containing random bytes in the range 0-256</returns>
        private byte[] GenerateRandomASCIIBytes(int length)
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

        /// <summary>
        /// Create a byte array with the bytes to insert put before or after the original set of bytes
        /// </summary>
        /// <param name="bytesOriginal">The original bytes</param>
        /// <param name="bytesToInsert">The bytes to insert</param>
        /// <param name="insertBefore">Whether to insert the bytes before, or after, the original bytes</param>
        /// <returns>A byte array with the bytes to insert added before/after the original bytes</returns>
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

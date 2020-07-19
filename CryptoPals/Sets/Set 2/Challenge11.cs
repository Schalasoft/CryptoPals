using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Collections;
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
        EncryptionTypeEnum actualEncryption;

        // Reuse previous challenge functionality
        IChallenge6 challenge6   = (IChallenge6)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge6);
        IChallenge7 challenge7   = (IChallenge7)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge7);
        IChallenge8 challenge8   = (IChallenge8)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge8);
        IChallenge9 challenge9   = (IChallenge9)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge9);
        IChallenge10 challenge10 = (IChallenge10)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge10);

        /// <inheritdoc />
        public string Solve(string input)
        {
            // Debug method to text many random byte arrays instead of just the one input string
            bool debug = true;
            int keySize = 16;
            if (debug)
            {
                return SolveDebug(keySize);
            }
            else
            {
                // Convert input string to bytes
                byte[] bytes = Encoding.ASCII.GetBytes(input);

                // Encrypt data
                byte[] encryptedBytes = EncryptWithUnknownKey(bytes, keySize);

                // Detect encryption
                EncryptionTypeEnum oracleType = DetectEncryptionType(encryptedBytes);

                // Detect the encryption type used and format the output
                return FormatOutput(DetectEncryptionType(encryptedBytes));
            }
        }

        /// <summary>
        /// Debug method used for testing various random byte arrays using the Oracle
        /// </summary>
        /// <param name="keySize">The size of the key to use for encryption</param>
        /// <returns>A formatted string containing the number of correct answers vs the total, and each determination vs the actual</returns>
        private string SolveDebug(int keySize)
        {
            // CDG DEBUG, check for accuracy on many encryptions
            // Due to the random bytes added, passing in the keySize does not return expected results
            // Should try ranges of key sizes
            byte[] encryptedBytes = new byte[0];
            List<string> outputs = new List<string>();
            int totalCount = 50; // Total number of tests
            bool[] correct = new bool[totalCount];
            byte[] bytes = new byte[0];
            for (int i = 0; i < totalCount; i++)
            {
                bytes = GenerateRandomASCIIBytes(100);
                encryptedBytes = EncryptWithUnknownKey(bytes, keySize);
                EncryptionTypeEnum oracleType = DetectEncryptionType(encryptedBytes);
                outputs.Add(FormatOutput(oracleType));
                correct[i] = oracleType.Equals(actualEncryption);
            }

            int correctCount = correct.Where(x => x.Equals(true)).Count();

            // CDG DEBUG
            return $"Correct: {correctCount}/{totalCount}{Environment.NewLine}{string.Join(Environment.NewLine, outputs)}";
        }

        /// <summary>
        /// Return the type of encryption used on bytes
        /// </summary>
        /// <param name="bytes">The bytes to detect ECB/CBC on</param>
        /// <returns>The encryption type used as an enumeration</returns>
        private EncryptionTypeEnum DetectEncryptionType(byte[] bytes)
        {
            // The amount of block sizes to try
            int blockSizeAttempts = 50;
            return challenge8.IsECBEncrypted(bytes, blockSizeAttempts) ? EncryptionTypeEnum.ECB : EncryptionTypeEnum.CBC; ;
        }

        /// <summary>
        /// Encrypt bytes using ECB and CBC randomly for each block
        /// </summary>
        /// <param name="bytes">The bytes to encrypt</param>
        /// <param name="blockLength">The length of each block to encrypt</param>
        /// <param name="key">The key used to encrypt</param>
        /// <returns>A byte array containing the encrypted bytes (where each block has a 50% chance of being ECB or CBC encrypted)</returns>
        private byte[] EncryptBytesRandomly(byte[] bytes, int blockLength, byte[] key)
        {
            // Encrypt all blocks with ECB or CBC randomly
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
                byte[] iv = GenerateRandomASCIIBytes(blockLength);

                // Encrypt using CBC
                encryptedBytes = challenge10.AES_CBC(true, bytes, key, iv);
            }

            // Record which encryption type we used for this block
            actualEncryption = encryptionType;

            return encryptedBytes;
        }

        /// <summary>
        /// Encrypts data under a randomly generated key, randomly using ECB or CBC for each block
        /// </summary>
        /// <param name="bytes">The bytes to encrypt</param>
        /// <param name="keyLength">The length of the key to populate with random bytes (0-256)</param>
        /// <returns>The bytes encrypted with an unknown key, where each block is ECB or CBC encrypted</returns>
        private byte[] EncryptWithUnknownKey(byte[] bytes, int keyLength)
        {
            // Insert 5-10 random bytes before and after the bytes
            byte[] bytesWithInserts = InsertRandomBytes(bytes);

            // Pad the constructed byte array if it is not divisible by the key length
            if (bytesWithInserts.Length % keyLength != 0)
                bytesWithInserts = challenge9.PadBytes(bytesWithInserts, keyLength);
            // TODO CDG we might not want to be using the key length to pad
            // Same for EncryptBytesRandomly

            // Create a random key
            byte[] key = GenerateRandomASCIIBytes(keyLength);

            // Encrypt the data where each block has been randomly encrypted with ECB or CBC
            return EncryptBytesRandomly(bytesWithInserts, keyLength, key);
        }

        /// <summary>
        /// Format the output to display the  the oracles determination, and the actual encryption used on the input bytes
        /// </summary>
        /// <param name="type">An enum containing the oracles determination of the encryption type used</param>
        /// <returns>The formatted output</returns>
        private string FormatOutput(EncryptionTypeEnum type)
        {
            return $"{type.ToString()} / {actualEncryption.ToString()} (Oracle / Actual)";
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

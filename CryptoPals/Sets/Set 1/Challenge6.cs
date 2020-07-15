using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CryptoPals.Sets
{
    class Challenge6 : IChallenge
    {
        /*
        Break repeating-key XOR
        It is officially on, now.
        This challenge isn't conceptually hard, but it involves actual error-prone coding. 
        The other challenges in this set are there to bring you up to speed. 
        This one is there to qualify you. If you can do this one, you're probably just fine up to Set 6.

        There's a file here. It's been base64'd after being encrypted with repeating-key XOR.

        Decrypt it.

        Here's how:

        !) Let KEYSIZE be the guessed length of the key; try values from 2 to (say) 40.
        2) Write a function to compute the edit distance/Hamming distance between two strings. 
           The Hamming distance is just the number of differing bits. The distance between:
           this is a test
           and
           wokka wokka!!!
           is 37. Make sure your code agrees before you proceed.
        3) For each KEYSIZE, take the first KEYSIZE worth of bytes, and the second KEYSIZE worth of bytes, and find the edit distance between them. 
           Normalize this result by dividing by KEYSIZE.
        4) The KEYSIZE with the smallest normalized edit distance is probably the key. You could proceed perhaps with the smallest 2-3 KEYSIZE values. 
           Or take 4 KEYSIZE blocks instead of 2 and average the distances.
        5) Now that you probably know the KEYSIZE: break the ciphertext into blocks of KEYSIZE length.
        6) Now transpose the blocks: make a block that is the first byte of every block, and a block that is the second byte of every block, and so on.
        7) Solve each block as if it was single-character XOR. You already have code to do this.
        8) For each block, the single-byte XOR key that produces the best looking histogram is the repeating-key XOR key byte for that block. 
           Put them together and you have the key.
        
        This code is going to turn out to be surprisingly useful later on. 
        Breaking repeating-key XOR ("Vigenere") statistically is obviously an academic exercise, a "Crypto 101" thing. 
        But more people "know how" to break it than can actually break it, and a similar technique breaks something much more important.

        No, that's not a mistake.
        We get more tech support questions for this challenge than any of the other ones. We promise, there aren't any blatant errors in this text. 
        In particular: the "wokka wokka!!!" edit distance really is 37.
        */

        // Reuse previous challenge functionality

        IChallenge2 challenge2 = (IChallenge2)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge2);
        IChallenge3 challenge3 = (IChallenge3)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge3);
        IChallenge5 challenge5 = (IChallenge5)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge5);

        // Solve the challenge
        public string Solve(string input)
        {
            // Decode Base64
            byte[] decodedBytes = Convert.FromBase64String(input);

            // Determine the repeating key using only the input bytes
            byte[] keyBytes = CalculateRepeatingKey(decodedBytes);

            // Decrypt using the determined repeating key
            byte[] outputBytes = challenge5.RepeatingKeyXOR(decodedBytes, keyBytes);

            // Get the key and decrypted as strings to output
            string key = Encoding.ASCII.GetString(keyBytes);
            string output = Encoding.ASCII.GetString(outputBytes);

            // Format the output
            string outputFormatted = $"{output}{Environment.NewLine}Key    : {key}";

            return outputFormatted;
        }

        // Calculate the key size by taking the lowest normalized hamming distance between equal sized sets of bytes in the text
        private int CalculateKeySize(byte[] bytes)
        {
            // Try key sizes 2 to 40
            int minKeySize = 2;
            int maxKeySize = 40;
            Dictionary<int, double> keySizes = new Dictionary<int, double>();
            for (int i = minKeySize; i <= maxKeySize; i++)
            {
                // Get the hamming distance of all adjacent blocks based of the current keysize
                double blocksHammingDistance = CalculateBlockHammingDistance(bytes, i);

                // Add the key size and hamming distance to the dictionary of possible key sizes
                keySizes.Add(i, blocksHammingDistance);
            }

            // Get the keysize associated with the smallest set of blocks hamming distance
            int keySize = keySizes.FirstOrDefault(x => x.Value == keySizes.Values.Min()).Key;

            return keySize;
        }

        // Get the hamming distance of consecutive adjacent blocks of bytes the length of keysize
        private double CalculateBlockHammingDistance(byte[] bytes, int keySize)
        {
            // Use MemoryStream to simplify taking chunks of bytes
            MemoryStream stream = new MemoryStream(bytes);

            // Get hamming distances for neighbouring chunks the full length of the bytes
            int i = 0;
            double[] distances = new double[bytes.Length / (keySize * 2)];
            while ((stream.Position + (keySize * 2)) < stream.Length) //  Don't take two full size chunks of keysize if there isn't enough bytes remaining
            {
                // Get the hamming distance between neighbouring keysize worth of bytes
                byte[] a = ByteConverter.GetBytes(stream, keySize); // This call effectively removes the bytes from the stream
                byte[] b = ByteConverter.GetBytes(stream, keySize);
                double distance = CalculateHammingDistance(a, b);

                // Normalize the distance by dividing by the keysize
                distance /= keySize;

                // Append the normalized distance to the list
                distances[i++] = distance;
            }

            // The block hamming distance is the sum of all the hamming distances of compared blocks normalized by dividing by the number of distances
            return (distances.Sum() / distances.Length);
        }

        // Break the ciphertext into blocks the size of the key
        private byte[][] CreateBlocks(byte[] bytes, int keySize)
        {
            byte[][] blocks = new byte[bytes.Length / keySize][];
            for (int i = 0; i < blocks.Length; i++)
            {
                byte[] block = bytes.Skip(i * keySize).Take(keySize).ToArray();
                blocks[i] = block;
            }

            return blocks;
        }

        // Transpose each block (using the blocks, make transposed blocks of the the 1st byte of each block, the 2nd, 3rd etc.)
        private byte[][] TransposeBlocks(byte[][] blocks, int keySize)
        {
            byte[][] transposedBlocks = new byte[keySize][];
            for (int transPos = 0; transPos < keySize; transPos++)
            {
                byte[] transposition = new byte[blocks.Length];
                for (int blockPos = 0; blockPos < blocks.Length; blockPos++)
                {
                    transposition[blockPos] = blocks[blockPos][transPos];
                }

                // Add the transposition to the array
                transposedBlocks[transPos] = transposition;
            }

            return transposedBlocks;
        }

        // Solve each transposed block as a single character XOR, combining these to get the actual key
        private byte[] SolveTransposedBlocks(byte[][] transposedBlocks, int keySize)
        {
            byte[] key = new byte[keySize];
            for (int i = 0; i < keySize; i++)
            {
                // Get the 'best' key XOR for this block
                KeyValuePair<int, Tuple<double, string>> kvp = challenge3.SingleKeyXORBruteForce(transposedBlocks[i]);
                key[i] = Convert.ToByte(kvp.Key);
            }

            // Get the actual key
            return key;
        }

        // Calculate the repeating key given only the input string (the text must be at least 81 characters long if we go up to a keysize of 40)
        private byte[] CalculateRepeatingKey(byte[] bytes)
        {
            // Calculate the key size
            int keySize = CalculateKeySize(bytes);

            // Break the ciphertext into blocks the size of the key
            byte[][] blocks = CreateBlocks(bytes, keySize);

            // Transpose each block (using the blocks, make transposed blocks of the the 1st byte of each block, the 2nd, 3rd etc.)
            byte[][] transposedBlocks = TransposeBlocks(blocks, keySize);

            // Solve each transposed block as a single character XOR, combining these to get the actual key
            byte[] key = SolveTransposedBlocks(transposedBlocks, keySize);

            return key;
        }

        // Get the Hamming Distance of two equal length byte arrays (the total number of set bits after XORing the bytes together)
        private int CalculateHammingDistance(byte[] a, byte[] b)
        {
            // Calculate hamming distance by XORing each byte, counting the number of 1s and summing them
            int hammingDistance = 0;
            for (int i = 0; i < a.Length; i++)
            {
                // Perform XOR
                byte c = challenge2.XOR(a[i], b[i]);

                // Count set bits
                int count = CountSetBits(c);

                // Add to the running total (hamming distance)
                hammingDistance += count;
            }

            return hammingDistance;
        }

        // Count the number of set bits in a byte by adding on any 1s using logical AND to the count, then right shifting to get the next bit
        // This is done until the byte is zero (has no 1s left)
        private int CountSetBits(byte b)
        {
            int count = 0;
            while (b > 0) 
            {
                count += b & 1;
                b >>= 1;
            }
            return count;
        }
    }
}
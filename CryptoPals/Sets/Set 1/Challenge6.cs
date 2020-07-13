using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
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

        // Solve the challenge
        public string Solve(string input)
        {
            // Calculate the repeating key given only the input string
            string key = CalculateRepeatingKey(input);

            string output = "";

            return output;
        }

        // Calculate the repeating key given only the input string (the text must be at least 81 characters long)
        private string CalculateRepeatingKey(string text)
        {
            // Try key sizes 2 to 40
            int maxKeySize = 40;
            int[] distances = new int[maxKeySize - 2];
            for (int i = 2; i <= maxKeySize; i++)
            {
                // Get the hamming distance between 1st and 2nd sets of bytes of the keysize, divide by keysize to normalize the result
                distances[i - 2] = GetHammingDistance(text.Substring(0, i), text.Substring(i + 1, i)) / i;
            }

            // Get the minimum distance from all the distances (this is likely the actual key size)
            int keySize = distances.Min();

            // Break the ciphertext into blocks the size of the key
            string[] blocks = new string[text.Length / keySize];
            for(int i = 0; i < text.Length; i += keySize)
            {
                blocks[i % keySize] = text.Substring(i, keySize);
            }

            // Transpose each block (using the blocks, make transposed blocks of the the 1st byte of each block, the 2nd, 3rd etc.)
            string[] transposedBlocks = new string[text.Length / keySize];
            for (int i = 0; i < transposedBlocks.Length; i++)
            {
                // Create a transposed block
                StringBuilder transposition = new StringBuilder();
                for (int j = 0; j < keySize; j++)
                {
                    transposition.Append(blocks[j][i]);
                }

                // Store the transposed block in an array
                transposedBlocks[i] = transposition.ToString();
            }

            // Solve each transposed block as a single character XOR
            int keyInt = 0;
            for(int i = 0; i < transposedBlocks.Length; i++)
            {
                // Get the 'best' repeating key XOR for this block
                string output = challenge3.Solve(transposedBlocks[i]);
                byte blockKey = (byte)'a';

                // Add the block key to the actual key (the actual key is the sum of the block keys)
                keyInt += (int)blockKey;
            }

            string key = (string)Encoding.ASCII.GetString(new byte[] { (byte)keyInt }); ;

            return key;
        }

        // Get the Hamming Distance of two equal length strings (the total number of set bits after XORing the bytes of the strings together)
        private int GetHammingDistance(string a, string b)
        {
            // Convert to bytes
            byte[] bytesX = Encoding.ASCII.GetBytes(a);
            byte[] bytesY = Encoding.ASCII.GetBytes(b);

            // Calculate hamming distance by XORing each byte, counting the number of 1s and summing them
            int hammingDistance = 0;
            for (int i = 0; i < bytesX.Length; i++)
            {
                // Perform XOR
                byte c = challenge2.XOR(bytesX[i], bytesY[i]);

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
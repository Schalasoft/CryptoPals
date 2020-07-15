using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using System;
using System.Text;

namespace CryptoPals.Sets
{
    class Challenge8 : IChallenge
    {
        /*
        Detect AES in ECB mode
        In this file are a bunch of hex-encoded ciphertexts.

        One of them has been encrypted with ECB.

        Detect it.

        Remember that the problem with ECB is that it is stateless and deterministic; the same 16 byte plaintext block will always produce the same 16 byte ciphertext.
        */

        // Reuse previous challenge functionality
        IChallenge6 challenge6 = (IChallenge6)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge6);
        IChallenge7 challenge7 = (IChallenge7)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge7);

        public string Solve(string input)
        {
            //Convert input to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(input);

            // Find the line with the hex-encoded ciphertext

            byte[] key = Encoding.ASCII.GetBytes("key");
            byte[] encryptedData = Encoding.ASCII.GetBytes("data");

            // Break text up into blocks
            //for (int i = 0; i < 40; i++)
            int i = 16;
            {
                byte[][] blocks = challenge6.CreateBlocks(bytes, i);

                // Check if there are any recurring patterns in the blocks
                
            }

            // Decrypt
            byte[] decrypted = challenge7.Decrypt(encryptedData, key);

            string output = Encoding.ASCII.GetString(decrypted);

            return output;
        }
    }
}

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
        IChallenge7 challenge7 = (IChallenge7)ChallengeFactory.InitializeChallenge(Enumerations.ChallengeEnum.Challenge7);
        IChallenge9 challenge9 = (IChallenge9)ChallengeFactory.InitializeChallenge(Enumerations.ChallengeEnum.Challenge9);

        public string Solve(string input)
        {
            // CDG DEBUG
            input = "Tell me and I forget. Teach me and I remember. Involve me and I learn.";

            byte[] bytes = Encoding.ASCII.GetBytes(input);
            byte[] key   = Encoding.ASCII.GetBytes("YELLOW SUBMARINE");

            // ECB Encrypt/Decrypt
            byte[] encrypted = challenge7.AES_ECB(true, bytes, key);
            string encryptedString = Encoding.ASCII.GetString(encrypted);
            byte[] decrypted = challenge7.AES_ECB(false, encrypted, key);
            string decryptedString = Encoding.ASCII.GetString(decrypted);
            // NOTE that the encrypted/decrypted bytes are missing bytes as we are not using padding!

            // XOR Combine


            string output = "";

            return output;
        }
    }
}

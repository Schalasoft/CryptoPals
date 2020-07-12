using CryptoPals.Interfaces;
using System.Text;

namespace CryptoPals.Sets
{
    class Challenge5 : IChallenge
    {
        /*
        Implement repeating-key XOR
        Here is the opening stanza of an important work of the English language:

        Burning 'em, if you ain't quick and nimble
        I go crazy when I hear a cymbal
        Encrypt it, under the key "ICE", using repeating-key XOR.

        In repeating-key XOR, you'll sequentially apply each byte of the key; the first byte of plaintext will be XOR'd against I, the next C, the next E, then I again for the 4th byte, and so on.

        It should come out to:

        0b3637272a2b2e63622c2e69692a23693a2a3c6324202d623d63343c2a26226324272765272
        a282b2f20430a652e2c652a3124333a653e2b2027630c692b20283165286326302e27282f
        Encrypt a bunch of stuff using your repeating-key XOR function. Encrypt your mail. Encrypt your password file. Your .sig file. Get a feel for it. I promise, we aren't wasting your time with this.
        */

        // Reuse previous challenge functionality
        IChallenge2 challenge2 = (IChallenge2)ChallengeManager.GetChallenge((int)Enumerations.ChallengeEnum.Challenge2);

        // Solve the challenge
        public string Solve(string input)
        {
            // Split the input to get the plaintext to encrypt and the cycling key
            string[] split = input.Split(Constants.Separator);

            // Encrypt
            string output = RepeatingKeyXOR(split[0], split[1]);

            return output;
        }

        // Sequentially apply each byte of the key against each byte of the input text and return the resulting encoded text
        private string RepeatingKeyXOR(string text, string key)
        {
            // Get text bytes
            byte[] textBytes = Encoding.ASCII.GetBytes(text);
            byte[] keyBytes  = Encoding.ASCII.GetBytes(key);

            // Go through each letter in the input text
            byte[] bytes = new byte[textBytes.Length];
            int keyIndex = 0;
            for (int i = 0; i < textBytes.Length; i++)
            {
                // Encrypt the bytes by XORing it against the appropriate part of the repeating key
                bytes[i] = challenge2.XOR(textBytes[i], keyBytes[keyIndex++]);

                // Reset repeating key index
                if (keyIndex > 2)
                    keyIndex = 0;
            }

            // todo Need to get \n to convert to a single byte of value 10 in decimal, it currently converts to two separate bytes ruining the encoding

            // Return encoded string from the encoded bytes
            string actual = challenge2.HexBytesToString(bytes);
            string expected = "0b3637272a2b2e63622c2e69692a23693a2a3c6324202d623d63343c2a26226324272765272a282b2f20430a652e2c652a3124333a653e2b2027630c692b20283165286326302e27282f";

            if(actual.Equals(expected))
            {
                int a = 0;
            }
            return actual;
        }
    }
}
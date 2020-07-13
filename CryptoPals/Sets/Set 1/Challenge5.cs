using CryptoPals.Interfaces;

namespace CryptoPals.Sets
{
    class Challenge5 : IChallenge5, IChallenge
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
            return RepeatingKeyXOR(split[0], split[1]);
        }

        // Sequentially apply each byte of the key against each byte of the input text and return the resulting encoded text
        public string RepeatingKeyXOR(string text, string key)
        {
            // Go through each letter in the input text
            byte[] bytes = new byte[text.Length];
            int keyIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                // Encrypt the bytes by XORing it against the appropriate part of the repeating key
                bytes[i] = challenge2.XOR((byte)text[i], (byte)key[keyIndex++]);

                // Reset repeating key index
                if (keyIndex > 2)
                    keyIndex = 0;
            }

            // Return encoded string from the encoded bytes
            return challenge2.HexBytesToString(bytes);
        }
    }
}
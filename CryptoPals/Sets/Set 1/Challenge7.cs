using CryptoPals.Interfaces;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Text;

namespace CryptoPals.Sets
{
    class Challenge7 : IChallenge7, IChallenge
    {
        /*
        The Base64-encoded content in this file has been encrypted via AES-128 in ECB mode under the key

        "YELLOW SUBMARINE".
        (case-sensitive, without the quotes; exactly 16 characters; I like "YELLOW SUBMARINE" because it's exactly 16 bytes long, and now you do too).

        Decrypt it. You know the key, after all.

        Easiest way: use OpenSSL::Cipher and give it AES-128-ECB as the cipher.

        Do this with code.
        You can obviously decrypt this using the OpenSSL command-line tool, but we're having you get ECB working in code for a reason. 
        You'll need it a lot later on, and not just for attacking ECB.
        */

        public string Solve(string input)
        {
            // Get bytes from Base64 string 
            byte[] bytes = Convert.FromBase64String(input);

            // Get key as bytes
            byte[] key = Encoding.ASCII.GetBytes("YELLOW SUBMARINE");

            // Decrypt the input using the key
            byte[] data = Decrypt(bytes, key);

            // Get decrypted bytes as text
            string output = Encoding.ASCII.GetString(decrypted);

            return output;
        }

        // Decrypt the encrypted payload
        public byte[] Decrypt(byte[] data, byte[] key)
        {
            // Setup the decryption cipher
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");
            cipher.Init(false, new KeyParameter(key));

            // Decrypt
            byte[] decrypted = cipher.ProcessBytes(data);

            return decrypted;
        }
    }
}

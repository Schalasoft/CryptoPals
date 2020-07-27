using CryptoPals.Interfaces;
using System;
using CryptoPals.Extension_Methods;

namespace CryptoPals.Sets
{
    ///<inheritdoc cref="IChallenge7"/>
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

        ///<inheritdoc />
        public string Solve(string input)
        {
            // Get bytes from Base64 string 
            byte[] bytes = Convert.FromBase64String(input);

            // Get key as bytes
            byte[] key = "YELLOW SUBMARINE".GetBytes();

            // Decrypt the input using the key
            byte[] data = AES_ECB(bytes, key, false);

            // Get decrypted bytes as text
            return data.GetASCIIString();
        }

        /// <summary>
        /// Encrypt/Decrypt bytes using AES in ECB mode
        /// </summary>
        /// <param name="bytes">The bytes to encrypt/decrypt</param>
        /// <param name="key">The key to use for encrypting/decrypting</param>
        /// <param name="encrypt">Whether we are encrypting or decrypting</param>
        /// <returns>The encrypted/decrypted data</returns>
        public byte[] AES_ECB(byte[] bytes, byte[] key, bool encrypt)
        {
            return Cryptography.AES_ECB(bytes, key, encrypt);
        }
    }
}

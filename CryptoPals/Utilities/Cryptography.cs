using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace CryptoPals
{
    public static class Cryptography
    {
        /// <summary>
        /// Encrypt/decrypt input bytes using AES ECB with no padding
        /// </summary>
        /// <param name="bytes">The bytes to encrypt/decrypt</param>
        /// <param name="key">The key to use for encryption/decryption</param>
        /// <param name="encrypt">Whether to encrypt or decrypt, defaults to true</param>
        /// <returns>The encrypted/decrypted bytes</returns>
		public static byte[] AES_ECB(byte[] bytes, byte[] key, bool encrypt = true)
		{
            // Setup the encryption/decryption cipher
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");
            cipher.Init(encrypt, new KeyParameter(key));

            // Encrypt/Decrypt
            return cipher.ProcessBytes(bytes);
        }
	}
}


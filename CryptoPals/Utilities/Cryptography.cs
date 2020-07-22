using System.Security.Cryptography;

namespace CryptoPals.Utilities
{
    static class Cryptography
    {
		public static byte[] AES_128_ECB_Encrypt(byte[] plainBytes, byte[] key)
		{
			var aes = new AesCryptoServiceProvider
			{
				KeySize = 128,
				Key = key,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.Zeros
			};

			var encryptor = aes.CreateEncryptor();
			byte[] cipherBytes = new byte[plainBytes.Length];
			for (int offset = 0; offset < plainBytes.Length; offset += aes.BlockSize / 8)
			{
				encryptor.TransformBlock(plainBytes, offset, aes.BlockSize / 8, cipherBytes, offset);
			}

			return cipherBytes;
		}
	}
}


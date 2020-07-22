using System.Security.Cryptography;

namespace CryptoPals.Interfaces
{
    /// <summary>
    /// Byte-at-a-time ECB decryption (Simple)
    /// </summary>
    interface IChallenge12 : IChallenge
    {
        public byte[] Oracle(bool encrypt, string text, byte[] key, int crypto = 0, byte[] iv = null, CipherMode mode = CipherMode.ECB);
    }
}

using System.Text;

namespace CryptoPals.Extension_Methods
{
    static class ByteArrayExtensionMethods
    {
        /// <summary>
        /// Returns the ASCII string representation of the bytes
        /// </summary>
        /// <param name="bytes">The bytes to convert to a string</param>
        /// <returns>The ASCII string represented by the bytes</returns>
        public static string GetASCIIString(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
    }
}

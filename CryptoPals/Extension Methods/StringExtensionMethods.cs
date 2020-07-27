using System.Text;

namespace CryptoPals.Extension_Methods
{
    static class StringExtensionMethods
    {
        /// <summary>
        /// Returns the ASCII bytes of the provided string
        /// </summary>
        /// <param name="text">The string to get the bytes of</param>
        /// <returns>The bytes of the ASCII string</returns>
        public static byte[] GetBytes(this string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }
    }
}
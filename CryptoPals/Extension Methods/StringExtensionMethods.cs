using System.Text;

namespace CryptoPals.Extension_Methods
{
    static class StringExtensionMethods
    {
        public static byte[] ToBytes(this string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }
    }
}
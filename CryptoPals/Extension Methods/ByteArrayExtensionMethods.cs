using System.Text;

namespace CryptoPals.Extension_Methods
{
    static class ByteArrayExtensionMethods
    {
        public static string ToASCIIString(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
    }
}

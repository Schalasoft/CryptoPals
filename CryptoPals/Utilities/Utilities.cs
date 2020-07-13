using System.IO;

namespace CryptoPals
{
    class ByteConverter
    {
        // Consume and return bytes from a memory stream
        public static byte[] GetBytes(MemoryStream stream, int length)
        {
            // Consume bytes from the memory stream
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);

            return bytes;
        }
    }
}


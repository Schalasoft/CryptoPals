using System.IO;

namespace CryptoPals
{
    static class FileHandling
    {
        // Get the text from a file in a directory
        public static string ReadFile(string dir, string fileName)
        {
            // Read entire file
            return File.ReadAllText($"{dir}{fileName}");
        }
    }
}

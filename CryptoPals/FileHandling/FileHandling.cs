using System.IO;
using System.Text.RegularExpressions;

namespace CryptoPals
{
    static class FileHandling
    {
        // Get the text from a file in a directory
        public static string ReadFile(string dir, string fileName)
        {
            // Read entire file
            string text = File.ReadAllText($"{dir}{fileName}");
            
            // Remove escape characters from string (we want '\n' instead of '\\n' or it will mess up the bytes) 
            text = Regex.Unescape(text);
            return text;
        }
    }
}

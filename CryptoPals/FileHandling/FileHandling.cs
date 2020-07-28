using System.IO;
using System.Text.RegularExpressions;

namespace CryptoPals
{
    /// <summary>
    /// The class handles files
    /// </summary>
    public static class FileHandling
    {
        /// <summary>
        /// Get the text from a file in a directory, if no directory provided then use the default data directory
        /// </summary>
        /// <param name="fileName">The name of the file (including .txt)</param>
        /// <param name="dir">The directory the file is found in</param>
        /// <returns>The plaintext from the file, with additional escape characters unescaped (as this causes issues with encryption/decryption)</returns>
        public static string ReadFile(string fileName, string dir = "..\\..\\..\\Data\\")
        {
            // Construct the file path
            string filePath = $"{dir}{fileName}";

            string output = "";
            if (File.Exists(filePath))
            {
                // Read entire file
                output = File.ReadAllText(filePath);

                // Remove escape characters from string (we want '\n' instead of '\\n' or it will mess up the bytes) 
                output = Regex.Unescape(output);
            }

            return output;
        }
    }
}

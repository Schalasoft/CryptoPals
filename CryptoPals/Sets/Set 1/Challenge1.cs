using System;
using System.Text;
using CryptoPals.Interfaces;

namespace CryptoPals.Sets.Challenges
{
    // Encode Hex string as Base64
    class Challenge1 : IChallenge
    {
        private int binaryBase = 2; // Binary is 0 or 1 == 2 bit
        private int hexBase = 16;   // Hex value is 2 bytes == 16 bit
        private int hexLength = 2;  // Length of a hex value in characters
        private string base64Table = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        // Convert hex to base 64 (2 char hex value is 1 byte, base64 is 4 characters per 3 bytes/hex values)
        // Input  : 49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d
        // Output : SSdtIGtpbGxpbmcgeW91ciBicmFpbiBsaWtlIGEgcG9pc29ub3VzIG11c2hyb29t
        public string Solution(string hex)
        {
            // Encode hex to Base64
            string encoded = Encode(hex);

            return encoded;
        }

        private string Encode(string hex)
        {
            // Get binary string representation of hex string
            string binary = HexStringToBinaryString(hex);

            // Get Base64 represenation of binary string
            string encoded = BinaryStringToBase64(binary);

            return encoded;
        }

        // Convert hex string to binary string
        private string HexStringToBinaryString(string text)
        {
            // Convert hex string to a string of binary
            StringBuilder binarySb = new StringBuilder();
            for (int i = 0; i < text.Length; i += hexLength)
            {
                // Grab hex value
                string hexValue = text.Substring(i, hexLength);

                // Convert hex value to hextet
                byte hextet = Convert.ToByte(hexValue, hexBase);

                // Convert byte to a string of binary
                string byteString = Convert.ToString(hextet, binaryBase).PadLeft(8, '0');

                // Append to the full binary sequence
                binarySb.Append(byteString);
            }
            return binarySb.ToString();
        }

        // Convert binary string to Base64 string
        private string BinaryStringToBase64(string text)
        {
            // Array to hold bytes (sextets) of binary string for conversion to Base64
            byte[] bytes = new byte[text.Length / 6];

            // Grab sextets from binary sequence
            StringBuilder stringBuilder = new StringBuilder();
            int index = 0;
            for (int i = 0; i < text.Length; i += 6, index++)
            {
                // Grab a sextet
                string sextet;
                if (i + 6 < text.Length)
                    sextet = text.Substring(i, 6);
                else
                    sextet = text.Substring(i, text.Length - i).PadLeft(6 - (text.Length - i), '0');

                // Convert to byte
                byte b = Convert.ToByte(sextet, binaryBase);

                // Build output string
                stringBuilder.Append(base64Table[(int)b]);
            }

            string byteString = stringBuilder.ToString();

            return byteString;
        }
    }
}

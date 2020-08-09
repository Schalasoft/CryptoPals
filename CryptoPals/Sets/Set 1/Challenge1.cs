using CryptoPals.Extension_Methods;
using CryptoPals.Interfaces;
using System;
using System.Linq;
using System.Text;

namespace CryptoPals.Sets
{
    ///<inheritdoc cref="IChallenge1"/>
    class Challenge1 : IChallenge1, IChallenge
    {
        /*
        Convert hex to base64

        The string:
        49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d
        
        Should produce:
        SSdtIGtpbGxpbmcgeW91ciBicmFpbiBsaWtlIGEgcG9pc29ub3VzIG11c2hyb29t

        So go ahead and make that happen. You'll need to use this code for the rest of the exercises.
        */

        // Note: 2 char hex value is 1 byte, base64 is 4 characters per 3 bytes/hex values
        private const int binaryBase = 2;    // Binary is 0 or 1 == 2 bit
        private const int hexBase    = 16;   // Hex value is 2 bytes == 16 bit
        private const string base64Table = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        ///<inheritdoc />
        public string Solve(string input)
        {
            // Convert Hex to Base64
            return HexStringToBase64(input);
        }

        ///<inheritdoc cref="IChallenge1.HexStringToBase64(string)"/>
        public string HexStringToBase64(string hex)
        {
            // Get binary string representation of hex string
            string binary = HexStringToBinaryString(hex);

            // Get Base64 represenation of binary string
            return BinaryStringToBase64(binary);
        }

        /// <summary>
        /// Convert hex string to its binary representation
        /// </summary>
        /// <param name="text">The hex text to convert</param>
        /// <returns>A string containing the binary representation of the hex string</returns>
        private string HexStringToBinaryString(string text)
        {
            // Convert to bytes
            byte[] bytes = HexStringToBytes(text);

            // Convert hex bytes to a string of binary
            StringBuilder binarySb = new StringBuilder();
            foreach (byte hextet in bytes)
            {
                // Convert byte to a string of binary
                string byteString = Convert.ToString(hextet, binaryBase).PadLeft(8, '0');

                // Append to the full binary sequence
                binarySb.Append(byteString);
            }

            return binarySb.ToString();
        }

        /// <summary>
        /// Check if provided bytes are valid hex (bytes have to be a multiple of 2 and contain only hex digits)
        /// </summary>
        /// <param name="bytes">The bytes to test</param>
        /// <returns>True if the bytes are valid hex, false otherwise</returns>
        private bool IsValidHexBytes(byte[] bytes)
        {
            return bytes.Length % 2 == 0 && !bytes.Any(x => x < 30 || x > 66);
        }

        /// <summary>
        /// Check if provided text is a valid hex string (bytes have to be a multiple of 2 and contain only hex digits)
        /// </summary>
        /// <param name="text">The text to test</param>
        /// <returns>True if the string is valid hex, false otherwise</returns>
        private bool IsValidHexString(string text)
        {
            return text.GetBytes().Length % 2 == 0 && System.Text.RegularExpressions.Regex.IsMatch(text, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        ///<inheritdoc cref="IChallenge1.HexStringToBytes(string)"/>
        public byte[] HexStringToBytes(string hex)
        {
            // Throw exception if the input text is not valid hex
            if (!IsValidHexString(hex))
                throw new FormatException();

            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = HexToByte(hex.Substring(i, 2));
            }

            return bytes;
        }

        ///<inheritdoc cref="IChallenge1.HexBytesToString(byte[])"/>
        public string HexBytesToString(byte[] bytes)
        {
            // Throw exception if the input is null
            if (bytes == null)
                throw new ArgumentNullException();

            // Throw exception if the input bytes are invalid
            if (!IsValidHexBytes(bytes))
                throw new FormatException();

            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        /// Convert a hex value to a byte
        /// </summary>
        /// <param name="hex">The hex to convert</param>
        /// <returns>The byte representation of the hex 2 character value</returns>
        private byte HexToByte(string hex)
        {
            return Convert.ToByte(hex, hexBase);
        }

        /// <summary>
        /// Convert hex binary string to Base64 string
        /// </summary>
        /// <param name="text">Hex byte represented as binary text</param>
        /// <returns>Base64 representation of the binary string</returns>
        private string BinaryStringToBase64(string text)
        {
            // Grab sextets from binary sequence
            StringBuilder stringBuilder = new StringBuilder();
            int index = 0;
            for (int i = 0; i < text.Length; i += 6, index++)
            {
                // Grab a sextet
                string sextet;
                if (i + 6 < text.Length)
                    sextet = text.Substring(i, 6);
                else // Missing data, pad sextet with zeros
                    sextet = text.Substring(i, text.Length - i).PadRight(6, '0');

                // Convert to byte
                byte b = Convert.ToByte(sextet, binaryBase);

                // Build output string
                stringBuilder.Append(base64Table[(int)b]);
            }

            // Get constructed string
            string output = stringBuilder.ToString();

            // Pad output to the expected data length (incase of 1 or 2 characters instead of 3 in final set)
            if(output.Length % 4 != 0)
                output = output.PadRight(output.Length + (4 - output.Length % 4), '=');

            return output;
        }
    }
}

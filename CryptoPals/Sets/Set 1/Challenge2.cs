using CryptoPals.Interfaces;
using System;

namespace CryptoPals.Sets
{
     /*
     Fixed XOR
     A function that takes two equal-length buffers and produces their XOR combination
     input:
     1c0111001f010100061a024b53535009181c (a)
     After hex decoding, and when XOR'd against:
     686974207468652062756c6c277320657965 (b)
     output:
     746865206b696420646f6e277420706c6179
     */

    class Challenge2 : IChallenge2, IChallenge
    {
        public string Solve(string input)
        {
            // Separate a and b from the combined input
            string[] split = input.Split('/');
            string hexA = split[0];
            string hexB = split[1];
            if(hexA.Length != hexB.Length)
                throw new Exception("A and B must be the same length to perform Fixed XOR");

            // Convert to bytes
            byte[] a = HexStringToBytes(hexA);
            byte[] b = HexStringToBytes(hexB);

            // Perform fixed XOR
            byte[] bytes = FixedXOR(a, b);

            // Convert to string
            string xord = HexBytesToString(bytes);

            return xord;
        }

        // Hex bytes to string with no dashes
        public string HexBytesToString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
        }

        // Converts a hex string to its byte representation
        public byte[] HexStringToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        // XOR two bytes
        public byte XOR(byte a, byte b)
        {
            return (byte)(a ^ b);
        }

        // XOR two equal length byte arrays
        private byte[] FixedXOR(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length];

            // XOR a and b to get c
            for (int i = 0; i < a.Length; i++)
                c[i] = XOR(a[i], b[i]);

            return c;
        }
    }
}

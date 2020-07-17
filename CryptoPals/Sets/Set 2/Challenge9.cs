﻿using CryptoPals.Interfaces;
using System.Text;

namespace CryptoPals.Sets
{
    class Challenge9 : IChallenge9, IChallenge
    {
        /*
        Implement PKCS#7 padding
        A block cipher transforms a fixed-sized block (usually 8 or 16 bytes) of plaintext into ciphertext. 
        But we almost never want to transform a single block; we encrypt irregularly-sized messages.

        One way we account for irregularly-sized messages is by padding, creating a plaintext that is an even multiple of the blocksize. 
        The most popular padding scheme is called PKCS#7.

        So: pad any block to a specific block length, by appending the number of bytes of padding to the end of the block. For instance,

        "YELLOW SUBMARINE"
        ... padded to 20 bytes would be:

        "YELLOW SUBMARINE\x04\x04\x04\x04"
        */

        public string Solve(string input)
        {
            // Get the text to pad
            byte[] bytes = Encoding.ASCII.GetBytes(input);

            // Pad the text bytes to the specified length
            int size = 20;
            byte[] paddedBytes = PadBytes(bytes, size);

            // Convert to string for output
            string output = Encoding.ASCII.GetString(paddedBytes);

            return output;
        }

        // Pad input bytes to the specified number of bytes
        public byte[] PadBytes(byte[] bytes, int size, byte paddingByte = (byte)0x04)
        {
            // If the blocksize we get is bigger than the the size specified, we are padding bytes to be evenly divisible by the size
            if (bytes.Length > size)
                size = bytes.Length + (size - (bytes.Length % size));

            // Create a byte array the size of the desired length
            byte[] paddedBytes = new byte[size];

            // Copy in the bytes to the padded bytes array
            bytes.CopyTo(paddedBytes, 0);
            
            // Pad the remaining unset bytes with EOT bytes (decimal 4)
            for(int i = bytes.Length; i < size; i++)
            {
                paddedBytes[i] = paddingByte;
            }

            return paddedBytes;
        }
    }
}

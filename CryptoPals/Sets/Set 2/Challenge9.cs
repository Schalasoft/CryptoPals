using CryptoPals.Extension_Methods;
using CryptoPals.Interfaces;

namespace CryptoPals.Sets
{
    ///<inheritdoc cref="IChallenge9"/>
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

        ///<inheritdoc />
        public string Solve(string input)
        {
            // Get the text to pad
            byte[] bytes = input.GetBytes();

            // Pad the text bytes to the specified length
            int size = 20;
            byte[] paddedBytes = PadBytes(size, bytes);

            // Convert to string for output
            return paddedBytes.GetASCIIString();
        }

        ///<inheritdoc cref="IChallenge9.PadBytes(byte[], int, byte)"/>
        public byte[] PadBytes(int size, byte[] bytes = null, byte paddingByte = (byte)0x04)
        {
            // Create a byte array the size of the desired length
            byte[] paddedBytes = new byte[size];
            int padFrom = 0; // Default to padding bytes from the start

            // Copy in bytes if any
            if (bytes != null)
            {
                // Copy in the bytes to the padded bytes array
                bytes.CopyTo(paddedBytes, 0);

                // Pad bytes after the inserted bytes
                padFrom = paddedBytes.Length;
            }
            else
            {
                bytes = new byte[0];
            }

            // If the blocksize we get is bigger than the the size specified, we are padding bytes to be evenly divisible by the size
            if (bytes.Length > size)
            {
                size = bytes.Length + (size - (bytes.Length % size));
            }

            // Pad the remaining unset bytes with EOT bytes (decimal 4)
            for (int i = padFrom; i < size; i++)
            {
                paddedBytes[i] = paddingByte;
            }

            return paddedBytes;
        }

        ///<inheritdoc cref="IChallenge9.PadBytesToBlockSizeMultiple(byte[], int, byte)"/>
        public byte[] PadBytesToBlockSizeMultiple(byte[] bytes, int blockSize, byte paddingByte = (byte)0x04)
        {
            // The byte array needs resized
            byte[] output;
            if (bytes.Length % blockSize != 0)
            {
                // Determine the new size (the size of the bytes plus how many bytes we are missing, which is the block size minus the modulus remainder of the length of the bytes and the block size)
                int newSize = bytes.Length + ((blockSize) - bytes.Length % blockSize);

                // Create resized byte array
                output = PadBytes(newSize, bytes, paddingByte);
            }
            else
            {
                // Doesn't need a resize, just output the bytes
                output = bytes;
            }

            return output;
        }
    }
}

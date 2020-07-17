using CryptoPals.Enumerations;
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

    ///<inheritdoc cref="IChallenge2"/>
    class Challenge2 : IChallenge2, IChallenge
    {
        // Reusue previous challenge functionality
        IChallenge1 challenge1 = (IChallenge1)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge1);

        ///<inheritdoc />
        public string Solve(string input)
        {
            // Separate a and b from the combined input
            string[] split = input.Split(Constants.Separator);
            string hexA = split[0];
            string hexB = split[1];
            if(hexA.Length != hexB.Length)
                throw new Exception("A and B must be the same length to perform Fixed XOR");

            // Convert to bytes
            byte[] a = challenge1.HexStringToBytes(hexA);
            byte[] b = challenge1.HexStringToBytes(hexB);

            // Perform fixed XOR
            byte[] bytes = XORByteArray(a, b);

            // Convert to string
            return challenge1.HexBytesToString(bytes);
        }

        ///<inheritdoc cref="IChallenge2.XORByte(byte, byte)"/>
        public byte XORByte(byte a, byte b)
        {
            return (byte)(a ^ b);
        }

        ///<inheritdoc cref="IChallenge2.XORByteArray(byte[], byte[])"/>
        public byte[] XORByteArray(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length];

            // XOR a and b to get c
            for (int i = 0; i < a.Length; i++)
                c[i] = XORByte(a[i], b[i]);

            return c;
        }
    }
}

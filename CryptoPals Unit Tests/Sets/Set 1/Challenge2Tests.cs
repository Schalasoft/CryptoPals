using CryptoPals.Enumerations;
using CryptoPals.Factories;
using CryptoPals.Interfaces;
using NUnit.Framework;
using System;

namespace CryptoPals_Unit_Tests_Challenge2
{
    public class Challenge2Tests
    {
        // Create challenge to test
        private static IChallenge2 challenge2 = (IChallenge2)ChallengeFactory.InitializeChallenge(ChallengeEnum.Challenge2);

        /// <summary>
        /// Setup method
        /// </summary>
        [SetUp]
        public void Setup() { }

        /// <summary>
        /// Test that challenge is solved correctly
        /// </summary>
        public class Solve
        {
            /// <summary>
            /// The challenge test
            /// </summary>
            [Test]
            public void Challenge()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Test that XORing two bytes works correctly
        /// </summary>
        public class XORByte
        {
            /// <summary>
            /// Test that XORing two bytes works correctly
            /// </summary>
            [Test]
            public void ValidInput_ValidResult()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Test that if either argument is null, we get an argument exception
            /// </summary>
            [Test]
            public void NullInput_ArgumentException()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Test that XORing two byte arrays works correctly
        /// </summary>
        public class XORByteArray
        {
            /// <summary>
            /// Test that byte arrays of the same size are correctly XOR'd together
            /// </summary>
            [Test]
            public void ValidInput_ValidResult()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Test that if either argument is null, we get an argument null exception
            /// </summary>
            [Test]
            public void NullInput_ArgumentNullException()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Test that if argument arrays are not the same size, get we an argument exception
            /// </summary>
            [Test]
            public void DifferentlySizedArrays_ArgumentNullException()
            {
                throw new NotImplementedException();
            }
        }
    }
}
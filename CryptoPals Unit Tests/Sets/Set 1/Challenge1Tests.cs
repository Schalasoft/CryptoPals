using CryptoPals.Enumerations;
using CryptoPals.Factories;
using CryptoPals.Interfaces;
using NUnit.Framework;
using System;
using System.Linq;

namespace CryptoPals_Unit_Tests_Challenge1
{
    // Only test public methods, why?
    // 1) These are the parts in the system that interact
    // 2) We don't care how the class gets the answer, just that it is right
    // 3) Small changes in private methods in our implementations mean changing the unit tests
    // ...meaning we lose our unit test safety net

    public class Challenge1Tests
    {
        // Create challenge to test
        private static IChallenge1 challenge1 = (IChallenge1)ChallengeFactory.InitializeChallenge(ChallengeEnum.Challenge1);

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
                string input = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d";
                string expectedOutput = "SSdtIGtpbGxpbmcgeW91ciBicmFpbiBsaWtlIGEgcG9pc29ub3VzIG11c2hyb29t";
                string actualOutput = challenge1.HexStringToBase64(input);

                // Use contains as the output contains formatting text surrounding the actual output
                if (actualOutput.Contains(expectedOutput))
                    Assert.Pass();
                else
                    Assert.Fail();
            }
        }

        /// <summary>
        /// Tests to confirm the method converts hex bytes to a hex string correctly
        /// </summary>
        public class HexBytesToString
        {
            /// <summary>
            /// Test that a valid hex byte array input produces a valid hex string as output
            /// </summary>
            [Test]
            public void ValidInput_ValidResult()
            {
                // Valid input bytes
                byte[] bytes = new byte[] { 30, 66 };
                
                // Convert bytes to hex string
                string result = challenge1.HexBytesToString(bytes);

                // Check if the conversion is correct
                if (result.Equals("1e42"))
                    Assert.Pass();
                else
                    Assert.Fail();
            }

            /// <summary>
            /// Test inputting invalid hex digits
            /// </summary>
            [Test]
            public void InvalidDigitInput_FormatExceptionThrown()
            {
                // Invalid input byte digits, but correct length
                byte[] bytes = new byte[] { 29, 67 };

                try
                {
                    // Convert bytes to hex string
                    challenge1.HexBytesToString(bytes);

                    Assert.Fail();
                }
                catch(FormatException)
                {
                    Assert.Pass();
                }
            }

            /// <summary>
            /// Test inputting invalid length hex string
            /// </summary>
            [Test]
            public void InvalidLengthInput_FormatExceptionThrown()
            {
                // Invalid input byte length (but valid hex digits)
                byte[] bytes = new byte[] { 73, 39, 109 };

                try
                {
                    // Convert bytes to hex string
                    challenge1.HexBytesToString(bytes);

                    Assert.Fail();
                }
                catch (FormatException)
                {
                    Assert.Pass();
                }
            }
        }

        /// <summary>
        /// Tests to confirm the method converts hex string to bytes correctly
        /// </summary>
        public class HexStringToBytes
        {
            /// <summary>
            /// Test that valid hex input text converts to expected valid output
            /// </summary>
            [Test]
            public void ValidInput_ValidResult()
            {
                byte[] expectedOutput = new byte[] { 30, 66 };
                string input = "1e42";
                byte[] actualOutput = challenge1.HexStringToBytes(input);

                if (actualOutput.SequenceEqual(expectedOutput))
                    Assert.Pass();
                else
                    Assert.Fail();
            }

            /// <summary>
            /// Test that invalid input throws a format exception
            /// </summary>
            [Test]
            public void InvalidInput_ExceptionThrown()
            {
                // Invalid input
                string input = "G";

                try
                {
                    // Convert bytes to hex string
                    challenge1.HexStringToBytes(input);

                    Assert.Fail();
                }
                catch (FormatException)
                {
                    Assert.Pass();
                }
            }
        }

        /// <summary>
        /// Test hex string to base 64 conversion
        /// </summary>
        public class HexStringToBase64
        {
            /// <summary>
            /// Test that valid hex input text produces valid base64 output text
            /// </summary>
            [Test]
            public void ValidInput_ValidResult()
            {
                string input = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d";
                string expectedOutput = "SSdtIGtpbGxpbmcgeW91ciBicmFpbiBsaWtlIGEgcG9pc29ub3VzIG11c2hyb29t";
                string actualOutput = challenge1.HexStringToBase64(input);

                if (actualOutput.Equals(expectedOutput))
                    Assert.Pass();
                else
                    Assert.Fail();
            }

            /// <summary>
            /// Test that invalid input produces a format exception
            /// </summary>
            [Test]
            public void InvalidInput_FormatExceptionThrown()
            {
                // Invalid input
                string input = "G";

                try
                {
                    // Convert bytes to hex string
                    challenge1.HexStringToBase64(input);

                    Assert.Fail();
                }
                catch (FormatException)
                {
                    Assert.Pass();
                }
            }
        }
    }
}
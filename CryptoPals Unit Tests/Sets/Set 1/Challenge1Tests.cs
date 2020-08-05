using CryptoPals.Enumerations;
using CryptoPals.Factories;
using CryptoPals.Interfaces;
using NUnit.Framework;

namespace CryptoPals_Unit_Tests_Sets
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

        [SetUp]
        public void Setup() { }

        /// <summary>
        /// 
        /// </summary>
        public class HexBytesToString
        {
            /// <summary>
            /// 
            /// </summary>
            [Test]
            public void ValidInput_ValidResult()
            {
                // string result = challenge1.HexBytesToString("49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d");
                string result = "";
                if (result.Equals(""))
                    Assert.Pass();
                else
                    Assert.Fail();
            }

            /// <summary>
            /// 
            /// </summary>
            [Test]
            public void InvalidInput_ExceptionThrown()
            {
                Assert.Pass();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class HexStringToBytes
        {
            [Test]
            public void ValidInput_ValidResult()
            {
                Assert.Pass();
            }

            [Test]
            public void InvalidInput_ExceptionThrown()
            {
                Assert.Pass();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class HexStringToBase64
        {
            /// <summary>
            /// 
            /// </summary>
            [Test]
            public void ValidInput_ValidResult()
            {
                string input = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d";
                string expectedOutput = "SSdtIGtpbGxpbmcgeW91ciBicmFpbiBsaWtlIGEgcG9pc29ub3VzIG11c2hyb29t";

                string output = challenge1.HexStringToBase64(input);

                if (output.Equals(expectedOutput))
                    Assert.Pass();
                else
                    Assert.Fail();
            }

            /// <summary>
            /// 
            /// </summary>
            [Test]
            public void InvalidInput_ArgumentExceptionThrown()
            {
                    Assert.Pass();
            }
        }
    }
}
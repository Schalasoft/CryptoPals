using CryptoPals.Enumerations;
using CryptoPals.Factories;
using CryptoPals.Interfaces;
using NUnit.Framework;
using System;

// This class is used to quickly create a new unit test

namespace CryptoPals_Unit_Tests_ChallengeN
{
    public class ChallengeNTests
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
                throw new NotImplementedException();
            }
        }
    }
}
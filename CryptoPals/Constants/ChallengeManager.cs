using CryptoPals.Enumerations;
using CryptoPals.Factories;
using CryptoPals.Interfaces;

namespace CryptoPals.Managers
{
    /// <summary>
    /// Challenge Manager initializes each challenge class with the ChallengeFactory.
    /// This is done so the challenges are only initialized once, where they can be requested by subsequent challenges
    /// </summary>
    public static class ChallengeManager
    {
        // Array of all the challenges (for reuse in later challenges)
        public static IChallenge[] Challenges { get; private set; }

        /// <summary>
        /// Initialize the number of specified challenges (starting from 1)
        /// </summary>
        /// <param name="challengeCount">The number of challenges to initialize</param>
        static public void Initialize(int challengeCount)
        {
            Challenges = new IChallenge[challengeCount];
            for(int i = 1; i <= challengeCount; i++)
            {
                Challenges[i-1] = ChallengeFactory.InitializeChallenge((ChallengeEnum)i);
            }
        }

        /// <summary>
        /// Get a challenge interface based on its ID
        /// </summary>
        /// <param name="challengeId">The challenge ID</param>
        /// <returns>The challenge interface associated with the ID (see ChallengeEnum)</returns>
        static public IChallenge GetChallenge(int challengeId)
        {
            return Challenges[challengeId-1];
        }
    }
}

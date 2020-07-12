using CryptoPals.Enumerations;
using CryptoPals.Factories;
using CryptoPals.Interfaces;

namespace CryptoPals
{
    static class ChallengeManager
    {
        // Array of all the challenges (for reuse in later challenges)
        static public IChallenge[] Challenges;

        // Initialize all challenges
        static public void Initialize(int challengeCount)
        {
            Challenges = new IChallenge[challengeCount];
            for(int i = 0; i < challengeCount; i++)
            {
                Challenges[i] = ChallengeFactory.InitializeChallenge((ChallengeEnum)i);
            }
        }

        // Get a challenge based on its ID
        static public IChallenge GetChallenge(int challengeId)
        {
            return Challenges[challengeId];
        }
    }
}

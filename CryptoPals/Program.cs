using CryptoPals.Interfaces;
using CryptoPals.Factories;
using CryptoPals.Enumerations;

namespace CryptoPals
{
    class Program
    {
        static void Main(string[] args)
        {
            SolveChallenges();
        }

        private static void SolveChallenges()
        {
            int challengeCount = 2;
            string input;
            for(int i = 1; i <= challengeCount; i++)
            {
                // Setup input
                switch(i)
                {
                    case 1:
                        input = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d";
                    break;

                    case 2:
                        input = "1c0111001f010100061a024b53535009181c";
                        break;

                    default:
                        input = "";
                    break;
                }

                // Solve the challenge
                SolveChallenge(i, input);
            }
        }

        private static string SolveChallenge(int challengeId, string input)
        {
            ChallengeEnum challengeEnum = (ChallengeEnum)challengeId;

            IChallenge challenge = ChallengeFactory.InitializeChallenge(challengeEnum);

            return challenge.Solution(input);
        }
    }
}

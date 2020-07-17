using System;
using System.Collections.Generic;

namespace CryptoPals
{
    /*
                                                     ,  ,
                                                   / \/ \
                                                  (/ //_ \_
         .-._                                      \||  .  \
          \  '-._                            _,:__.-"/---\_ \
     ______/___  '.    .--------------------'~-'--.)__( , )\ \
    `'--.___  _\  /    |             Here        ,'    \)|\ `\|
         /_.-' _\ \ _:,_          Be Dragons           " ||   (
       .'__ _.' \'-/,`-~`                                |/
           '. ___.> /=,|  Abandon hope all ye who enter  |
            / .-'/_ )  '---------------------------------'
            )'  ( /(/
                 \\ "
                  '=='
    */

    class Program
    {
        private static List<int> challengesWithoutInputs = new List<int>() { };

        // Main entry point
        public static void Main(string[] args)
        {
            SolveChallenges();
        }

        // Challenge Manager holds instances of challenges for reuse
        private static void InitializeChallengeManager(int challengeCount)
        {
            ChallengeManager.Initialize(challengeCount);
        }

        // Solve all challenges
        private static void SolveChallenges()
        {
            // Initialize the Challenge Manager used to hold the instance of each challenge
            // DEBUG Should start at 1 and end at the last challenge but for speed we only run the current challenge
            int currentChallenge = 11;
            int startChallenge = 1;
            int challengeCount = currentChallenge;
            InitializeChallengeManager(challengeCount);

            string input = "";
            for (int i = startChallenge; i <= challengeCount; i++)
            {
                // Alias for input when it is too long for outputting to the console
                string inputAlias = "";

                // Construct challenge input filename
                string fileName = $"Challenge{i}.txt";

                // Read the challenge input file, unless it is a challenge without an input
                if(!challengesWithoutInputs.Contains(i))
                    input = FileHandling.ReadFile(Constants.Directory, fileName);

                // Perform any additional preparation operations for specific challenges
                switch (i)
                {
                    case 4:
                    case 6:
                    case 7:
                    case 8:
                    case 10:
                        inputAlias = fileName;
                    break;

                    default:
                        
                        break;
                }

                // Solve the challenge
                string output = SolveChallenge(i, input);

                // Replace input text with alias for logging if an alias has been set
                if (!inputAlias.Equals(""))
                    input = inputAlias;

                // Truncate the output to a max length
                int maxLength = 200;
                if (output.Length > maxLength)
                    output = $"{output.Substring(0, maxLength)} ...{Environment.NewLine}TRUNCATED";

                // Output challenge information
                Console.WriteLine($"Challenge {i}");
                Console.WriteLine($"Input  :{Environment.NewLine}{input}");
                Console.WriteLine($"Output :{Environment.NewLine}{output}");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------\n");
            }
        }

        // Solve an individual challenge
        private static string SolveChallenge(int challengeId, string input)
        {
            return ChallengeManager.GetChallenge(challengeId).Solve(input);
        }
    }
}

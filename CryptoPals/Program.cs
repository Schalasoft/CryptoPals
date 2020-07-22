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
        // Used to identify challenges that do not require input files
        private static List<int> challengesWithoutInputs = new List<int>() { 13 };
        private static List<int> challengesToUsePreviousInputs = new List<int>() { 12 };

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">Additional command line arguments (unused)</param>
        public static void Main(string[] args)
        {
            SolveChallenges();
        }

        /// <summary>
        /// Initialize the Challenge Manager that holds instances of challenges for reuse
        /// </summary>
        /// <param name="challengeCount"></param>
        private static void InitializeChallengeManager(int challengeCount)
        {
            ChallengeManager.Initialize(challengeCount);
        }

        /// <summary>
        /// Solve all the challenges
        /// </summary>
        private static void SolveChallenges()
        {
            // Initialize the Challenge Manager used to hold the instance of each challenge
            // DEBUG Should start at 1 and end at the last challenge but for speed we only run the current challenge
            int currentChallenge = 13;
            int startChallenge = currentChallenge;
            int challengeCount = 57;
            InitializeChallengeManager(challengeCount);

            string input = "";
            for (int i = startChallenge; i <= challengeCount; i++)
            {
                // Alias for input when it is too long for outputting to the console
                string inputAlias = "";

                // Use previous challenge input file if this challenge is specified to
                int fileIndex = i;
                if (challengesToUsePreviousInputs.Contains(i))
                    fileIndex = fileIndex - 1;

                // Construct challenge input filename
                string fileName = $"Challenge{fileIndex}.txt";

                // Read the challenge input file, unless it is a challenge without an input
                if(!challengesWithoutInputs.Contains(i))
                    input = FileHandling.ReadFile(fileName);

                // If the input file does not exist, or is empty, bomb out
                if (input.Length == 0)
                    return;

                // Perform any additional preparation operations for specific challenges
                switch (i)
                {
                    case 4:
                    case 6:
                    case 7:
                    case 8:
                    case 10:
                    case 11:
                    case 12:
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
                int maxLength = 2000;
                if (output.Length > maxLength)
                    output = $"{output.Substring(0, maxLength)} ...{Environment.NewLine}TRUNCATED";

                // Output challenge information
                Console.WriteLine($"Challenge {i}");
                Console.WriteLine($"Input  :{Environment.NewLine}{input}");
                Console.WriteLine($"Output :{Environment.NewLine}{output}");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------\n");
            }
        }

        /// <summary>
        /// Solve an individual challenge
        /// </summary>
        /// <param name="challengeId">The challenge to solve (its ID relates to its ChallengeEnum value)</param>
        /// <param name="input">The input string for the challenge (files from the Data directory)</param>
        /// <returns>The output of the challenge class (specific to each challenge)</returns>
        private static string SolveChallenge(int challengeId, string input)
        {
            return ChallengeManager.GetChallenge(challengeId).Solve(input);
        }
    }
}

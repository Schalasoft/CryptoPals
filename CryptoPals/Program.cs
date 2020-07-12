using System;

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
            int challengeCount = 5 + 1; // (ChallengeCount + 1) as challenge array is 0 based
            InitializeChallengeManager(challengeCount);

            string input, fileName;
            for (int i = 1; i < challengeCount; i++)
            {
                // Alias for input when it is too long for outputting to the console
                string inputAlias = "";

                // Read input, and perform any additional preparation operations on the input data for specific challenges
                fileName = $"Challenge{i}.txt";
                input = FileHandling.ReadFile(Constants.Directory, fileName);
                switch (i)
                {
                    case 4:
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

                // Output challenge information
                Console.WriteLine("Challenge {0}", i);
                Console.WriteLine("Input  : {0}", input);
                Console.WriteLine("Output : {0}", output);
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

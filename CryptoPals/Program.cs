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
            int challengeCount = 7;
            InitializeChallengeManager(challengeCount);

            string input, fileName;
            for (int i = 7; i <= challengeCount; i++)
            {
                // Alias for input when it is too long for outputting to the console
                string inputAlias = "";

                // Read input
                fileName = $"Challenge{i}.txt";
                input = FileHandling.ReadFile(Constants.Directory, fileName);

                // Perform any additional preparation operations for specific challenges
                switch (i)
                {
                    case 4:
                    case 6:
                    case 7:
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

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

            string input;
            for(int i = 1; i < challengeCount; i++)
            {
                // Alias for input when it is too long for outputting to the console
                string inputAlias = "";

                // Setup input
                switch (i)
                {
                    case 1:
                        input = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d";
                    break;

                    case 2:
                        string a = "1c0111001f010100061a024b53535009181c";
                        string b = "686974207468652062756c6c277320657965";
                        input = $"{a}{Constants.Separator}{b}";
                    break;

                    case 3:
                        input = "1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736";
                    break;

                    case 4:
                        string fileName = "Challenge4.txt";
                        input = FileHandling.ReadFile(Constants.Directory, fileName);
                        inputAlias = fileName;
                    break;

                    case 5:
                        string text = "Burning 'em, if you ain't quick and nimble I go crazy when I hear a cymbal";
                        string key = "ICE"; //ice baby
                        input = $"{text}{Constants.Separator}{key}";
                    break;

                    default:
                        input = "";
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

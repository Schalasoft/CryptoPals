using CryptoPals.Interfaces;
using CryptoPals.Factories;
using CryptoPals.Enumerations;
using System;
using System.IO;

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
        // File variables
        static string Directory = "..\\..\\..\\Data\\";

        static void Main(string[] args)
        {
            SolveChallenges();
        }

        private static void SolveChallenges()
        {
            int challengeCount = 4;
            string input;
            for(int i = 1; i <= challengeCount; i++)
            {
                // Alias for input when it is too long
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
                        char separator = '/';
                        input = $"{a}{separator}{b}";
                    break;

                    case 3:
                        input = "1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736";
                    break;

                    case 4:
                        string fileName = "Challenge4.txt";
                        input = ReadFile(Directory, fileName);
                        inputAlias = fileName;
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
            ChallengeEnum challengeEnum = (ChallengeEnum)challengeId;

            IChallenge challenge = ChallengeFactory.InitializeChallenge(challengeEnum);

            return challenge.Solve(input);
        }

        // Get the payload from the layer file
        private static string ReadFile(string dir, string fileName)
        {
            // Read entire file
            return File.ReadAllText($"{dir}{fileName}");
        }
    }
}

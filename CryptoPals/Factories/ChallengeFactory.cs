using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using CryptoPals.Sets;

namespace CryptoPals.Factories
{
    /// <summary>
    /// Factory pattern for creating challenges
    /// </summary>
    public static class ChallengeFactory
    {
        /// <summary>
        /// Inititalize a challenge based on its enumeration
        /// </summary>
        /// <param name="challenge">The challenge to initialize</param>
        /// <returns>The class associated with the challenge enumeration specified</returns>
        public static IChallenge InitializeChallenge(ChallengeEnum challenge)
        {
            // Initialize the appropriate class based on the provided enumeration
            switch (challenge)
            {
                case ChallengeEnum.None:
                    return null;

                case ChallengeEnum.Challenge1:
                    return new Challenge1();

                case ChallengeEnum.Challenge2:
                    return new Challenge2();

                case ChallengeEnum.Challenge3:
                    return new Challenge3();

                case ChallengeEnum.Challenge4:
                    return new Challenge4();

                case ChallengeEnum.Challenge5:
                    return new Challenge5();
                
                case ChallengeEnum.Challenge6:
                    return new Challenge6();
                
                case ChallengeEnum.Challenge7:
                    return new Challenge7();
                
                case ChallengeEnum.Challenge8:
                    return new Challenge8();
                
                case ChallengeEnum.Challenge9:
                    return new Challenge9();
                
                case ChallengeEnum.Challenge10:
                    return new Challenge10();
                
                case ChallengeEnum.Challenge11:
                    return new Challenge11();
                
                case ChallengeEnum.Challenge12:
                    return new Challenge12();
                
                case ChallengeEnum.Challenge13:
                    return new Challenge13();
                
                case ChallengeEnum.Challenge14:
                    return new Challenge14();
                /*
                case ChallengeEnum.Challenge15:
                    return new Challenge15();
                /*
                case ChallengeEnum.Challenge16:
                    return new Challenge16();
                /*
                case ChallengeEnum.Challenge17:
                    return new Challenge17();
                /*
                case ChallengeEnum.Challenge18:
                    return new Challenge18();
                /*
                case ChallengeEnum.Challenge19:
                    return new Challenge19();
                /*
                case ChallengeEnum.Challenge20:
                    return new Challenge20();
                /*
                case ChallengeEnum.Challenge21:
                    return new Challenge21();
                /*
                case ChallengeEnum.Challenge22:
                    return new Challenge22();
                /*
                case ChallengeEnum.Challenge23:
                    return new Challenge23();
                /*
                case ChallengeEnum.Challenge24:
                    return new Challenge24();
                /*
                case ChallengeEnum.Challenge25:
                    return new Challenge25();
                /*
                case ChallengeEnum.Challenge26:
                    return new Challenge26();
                /*
                case ChallengeEnum.Challenge27:
                    return new Challenge27();
                /*
                case ChallengeEnum.Challenge28:
                    return new Challenge28();
                /*
                case ChallengeEnum.Challenge29:
                    return new Challenge29();
                /*
                case ChallengeEnum.Challenge30:
                    return new Challenge30();
                /*
                case ChallengeEnum.Challenge31:
                    return new Challenge31();
                /*
                case ChallengeEnum.Challenge32:
                    return new Challenge32();
                /*
                case ChallengeEnum.Challenge33:
                    return new Challenge33();
                /*
                case ChallengeEnum.Challenge34:
                    return new Challenge34();
                /*
                case ChallengeEnum.Challenge35:
                    return new Challenge35();
                /*
                case ChallengeEnum.Challenge36:
                    return new Challenge36();
                /*
                case ChallengeEnum.Challenge37:
                    return new Challenge37();
                /*
                case ChallengeEnum.Challenge38:
                    return new Challenge38();
                /*
                case ChallengeEnum.Challenge39:
                    return new Challenge39();
                /*
                case ChallengeEnum.Challenge40:
                    return new Challenge40();
                /*
                case ChallengeEnum.Challenge41:
                    return new Challenge41();
                /*
                case ChallengeEnum.Challenge42:
                    return new Challenge42();
                /*
                case ChallengeEnum.Challenge43:
                    return new Challenge43();
                /*
                case ChallengeEnum.Challenge44:
                    return new Challenge44();
                /*
                case ChallengeEnum.Challenge45:
                    return new Challenge45();
                /*
                case ChallengeEnum.Challenge46:
                    return new Challenge46();
                /*
                case ChallengeEnum.Challenge47:
                    return new Challenge47();
                /*
                case ChallengeEnum.Challenge48:
                    return new Challenge48();
                /*
                case ChallengeEnum.Challenge49:
                    return new Challenge49();
                /*
                case ChallengeEnum.Challenge50:
                    return new Challenge50();
                /*
                case ChallengeEnum.Challenge51:
                    return new Challenge51();
                /*
                case ChallengeEnum.Challenge52:
                    return new Challenge52();
                /*
                case ChallengeEnum.Challenge53:
                    return new Challenge53();
                /*
                case ChallengeEnum.Challenge54:
                    return new Challenge54();
                /*
                case ChallengeEnum.Challenge55:
                    return new Challenge55();
                /*
                case ChallengeEnum.Challenge56:
                    return new Challenge56();
                /*
                case ChallengeEnum.Challenge57:
                    return new Challenge57();
                */
                default:
                    return null;
            }
    }
}
}

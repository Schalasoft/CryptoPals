using CryptoPals.Enumerations;
using CryptoPals.Interfaces;
using CryptoPals.Managers;

namespace CryptoPals.Sets
{
    /// <inheritdoc cref="IChallenge14"/>
    class Challenge14 : IChallenge14, IChallenge
    {
        /*
        Byte-at-a-time ECB decryption (Harder)
        Take your oracle function from #12. Now generate a random count of random bytes and prepend this string to every plaintext. You are now doing:

        AES-128-ECB(random-prefix || attacker-controlled || target-bytes, random-key)
        Same goal: decrypt the target-bytes.

        Stop and think for a second.
        What's harder than challenge #12 about doing this? How would you overcome that obstacle? The hint is: you're using all the tools you already have; no crazy math is required.

        Think "STIMULUS" and "RESPONSE".
        */

        // Use previous challenge functionality
        IChallenge13 challenge13 = (IChallenge13)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge13);

        /// <inheritdoc/>
        public string Solve(string input)
        {
            return "";
        }
    }
}

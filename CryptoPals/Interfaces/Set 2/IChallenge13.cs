namespace CryptoPals.Interfaces
{
    /// <summary>
    /// ECB cut-and-paste
    /// </summary>
    public interface IChallenge13 : IChallenge
    {
        /// <summary>
        /// Decrypt the encoded user profile and parse it.
        /// Using only the user input to profile_for() (as an oracle to generate "valid" ciphertexts) and the ciphertexts themselves, make a role = admin profile.
        /// </summary>
        /// <param name="encryptedProfileBytes"></param>
        /// <returns></returns>
        public string Attacker(byte[] encryptedProfileBytes);
    }
}

using CryptoPals.Enumerations;
using CryptoPals.Extension_Methods;
using CryptoPals.Interfaces;
using CryptoPals.Managers;
using System;
using System.Text.Json;

namespace CryptoPals.Sets
{
    /// <inheritdoc cref="IChallenge13"/>
    class Challenge13 : IChallenge13, IChallenge
    {
        /*
        Write a k=v parsing routine, as if for a structured cookie. The routine should take:

        foo=bar&baz=qux&zap=zazzle
        ... and produce:

        {
          foo: 'bar',
          baz: 'qux',
          zap: 'zazzle'
        }
        (you know, the object; I don't care if you convert it to JSON).

        Now write a function that encodes a user profile in that format, given an email address. You should have something like:

        profile_for("foo@bar.com")
        ... and it should produce:

        {
          email: 'foo@bar.com',
          uid: 10,
          role: 'user'
        }
        ... encoded as:

        email=foo@bar.com&uid=10&role=user
        Your "profile_for" function should not allow encoding metacharacters (& and =). Eat them, quote them, whatever you want to do, but don't let people set their email address to "foo@bar.com&role=admin".

        Now, two more easy functions. Generate a random AES key, then:

        Encrypt the encoded user profile under the key; "provide" that to the "attacker".
        Decrypt the encoded user profile and parse it.
        Using only the user input to profile_for() (as an oracle to generate "valid" ciphertexts) and the ciphertexts themselves, make a role=admin profile.
        */

        // Use previous challenge functionality
        IChallenge9 challenge9 = (IChallenge9)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge9);
        IChallenge11 challenge11 = (IChallenge11)ChallengeManager.GetChallenge((int)ChallengeEnum.Challenge11);

        // Key used for encryption/decryption: set once
        byte[] key;

        /// <inheritdoc />
        public string Solve(string input)
        {
            // Generate a random ASCII key
            key = challenge11.GenerateRandomASCIIBytes(16);

            // Make an encrypted, encoded profile by email (make it a specific length so the value of admin is at the start of a cipher block)
            byte[] encryptedProfile = profile_for("foooooooooooooooooooo@bar.com");

            // Decrypt the profile and return the plaintext of a cipher that will make a valid admin account
            return Attacker(encryptedProfile);
        }

        /// <inheritdoc cref="IChallenge13.Attacker(byte[])"/>
        public string Attacker(byte[] encryptedProfileBytes)
        {
            // Contrsuct a block containing admin value with padding
            byte[] adminValueBytes = "admin".GetBytes();
            byte[] adminPadded = challenge9.PadBytes(16, adminValueBytes);
            byte[] adminValueCipher = Cryptography.AES_ECB(adminPadded, key);

            // Construct the ciphertext with the last block containing user ciphertext with admin + padding ciphertext
            byte[] constructed = new byte[encryptedProfileBytes.Length + 16];
            Array.Copy(encryptedProfileBytes, 0, constructed, 0, encryptedProfileBytes.Length);
            Array.Copy(adminValueCipher, 0, constructed, encryptedProfileBytes.Length - 16, 16);

            // Decrypt it to validate
            byte[] adminDecrypted = Cryptography.AES_ECB(constructed, key, false);
            string adminCipherText = adminDecrypted.GetASCIIString();

            // Decrypt encrypted profile bytes for output
            string decryptedProfilePlainText = Cryptography.AES_ECB(encryptedProfileBytes, key, false).GetASCIIString();

            // Return formatted output
            return FormatOutput(encryptedProfileBytes.GetASCIIString(), decryptedProfilePlainText, constructed.GetASCIIString(), adminCipherText);
        }

        /// <summary>
        /// Format the output to show the original cipher and the manipulated output
        /// </summary>
        /// <param name="originalCipher"></param>
        /// <param name="originalPlainText"></param>
        /// <param name="modifiedCipher"></param>
        /// <param name="modifiedPlainText"></param>
        /// <returns>The formatted output</returns>
        private string FormatOutput(string originalCipher, string originalPlainText, string modifiedCipher, string modifiedPlainText)
        {
            return $"Original Cipher: {originalCipher}{Environment.NewLine}Original Plaintext: {originalPlainText}{Environment.NewLine}Modified Cipher: {modifiedCipher}{Environment.NewLine}Modified Plaintext: {modifiedPlainText}";
        }

        /// <summary>
        /// Return an encrypted, JSON profile for the email provided
        /// </summary>
        /// <param name="email">The email of the user to get/create</param>
        /// <returns>The encrypted, JSON profile</returns>
        private byte[] profile_for(string email)
        {
            // Remove meta characters
            email = email.Replace("&", "").Replace("=", "");

            // Create profile
            Profile profile = new Profile(email, 10, "user");

            // Encode profile as JSON
            string profileJson = JsonSerializer.Serialize(profile);

            // Return the JSON profile, as an encrypted ASCII string
            return Cryptography.AES_ECB(DecodeJSON_Profile(profileJson).GetBytes(), key);
        }

        /// <summary>
        /// Decode a JSON profile as a string
        /// </summary>
        /// <param name="jsonProfile">The JSON profile to decode as a string</param>
        /// <returns>String containing the decoded JSON profile</returns>
        private string DecodeJSON_Profile(string jsonProfile)
        {
            // Deserialize the JSON profile object
            Profile profile = JsonSerializer.Deserialize<Profile>(jsonProfile);

            // Encode the profile
            string encoded = $"email={profile.email}&uid={profile.uid}&role={profile.role}";

            // Pad the encoded profile so we don't lose parts in the encryption
            string padded = challenge9.PadBytesToBlockSizeMultiple(encoded.GetBytes(), 16).GetASCIIString();

            // Return the formatted, and padded output
            return padded;
        }

        /// <summary>
        /// Profile class: used to serialize the data into JSON format
        /// </summary>
        class Profile
        {
            // Member variables
            public string email { get; set; }
            public int uid { get; set; }
            public string role { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="email"></param>
            /// <param name="uid"></param>
            /// <param name="role"></param>
            public Profile(string email, int uid, string role)
            {
                this.email = email;
                this.uid = uid;
                this.role = role;
            }

            /// <summary>
            /// Parameterless constructor for deserialization
            /// </summary>
            public Profile()
            {

            }
        }
    }
}

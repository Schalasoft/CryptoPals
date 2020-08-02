using CryptoPals.Enumerations;
using CryptoPals.Extension_Methods;
using CryptoPals.Interfaces;
using CryptoPals.Managers;
using System;
using System.Text.Json;

namespace CryptoPals.Sets
{
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

        public string Solve(string input)
        {
            // Generate a random ASCII key
            key = challenge11.GenerateRandomASCIIBytes(16);

            // Make an encrypted, encoded profile by email (make it a specific length so the value of admin is at the start of a cipher block)
            byte[] encryptedProfile = profile_for("foooooooooooooooooooo@bar.com");

            // Decrypt the profile and return the plaintext of a cipher that will make a valid admin account
            return Attacker(encryptedProfile);
        }

        /*
        Decrypt the encoded user profile and parse it.
        Using only the user input to profile_for() (as an oracle to generate "valid" ciphertexts) and the ciphertexts themselves, make a role=admin profile.
        */
        private string Attacker(byte[] encryptedProfileBytes)
        {
            // Contrsuct a block containing admin value with padding
            byte[] adminValueBytes = "admin".GetBytes();
            byte[] adminPadded = challenge9.PadBytes(16, adminValueBytes);
            byte[] adminValueCipher = Cryptography.AES_ECB(adminPadded, key);

            // Construct the ciphertext with the last block containing user ciphertext with admin + padding ciphertext
            byte[] constructed = new byte[encryptedProfileBytes.Length + 16];
            Array.Copy(encryptedProfileBytes, 0, constructed, 0, encryptedProfileBytes.Length);
            Array.Copy(adminValueCipher, 0, constructed, encryptedProfileBytes.Length, 16);

            // Decrypt it to validate
            byte[] adminDecrypted = Cryptography.AES_ECB(constructed, key, false);
            string adminCipherText = adminDecrypted.GetASCIIString();

            return adminCipherText;
        }

        private byte[] profile_for(string email)
        {
            // Remove meta characters
            email = email.Replace("&", "").Replace("=", "");

            // Create profile
            Profile profile = new Profile(email, 10, "user");

            // Encode profile as JSON
            string profileJson = JsonSerializer.Serialize(profile);

            // Return the encoded JSON profile, as an encrypted ASCII string
            return Cryptography.AES_ECB(EncodeJSON_Profile(profileJson).GetBytes(), key);
        }

        private string EncodeJSON_Profile(string jsonProfile)
        {
            // Deserialize the JSON profile object
            Profile profile = JsonSerializer.Deserialize<Profile>(jsonProfile);

            // Return the formatted output
            return $"email={profile.email}&uid={profile.uid}&role={profile.role}";
        }

        // Profile class
        class Profile
        {
            // Member variables
            public string email { get; set; }
            public int uid { get; set; }
            public string role { get; set; }

            // Constructor
            public Profile(string email, int uid, string role)
            {
                this.email = email;
                this.uid = uid;
                this.role = role;
            }

            // Parameterless constructor for deserialization
            public Profile()
            {

            }
        }
    }
}

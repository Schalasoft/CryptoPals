using CryptoPals.Interfaces;
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

        public string Solve(string input)
        {
            // Convert input text to JSON
            string json = ConvertCookieToJSON("foo=bar&baz=qux&zap=zazzle");

            return "";
        }

        // Cookie class
        class Cookie
        {
            // Member variables
            string foo;
            string baz;
            string zap;

            // Constructor
            public Cookie(string foo, string baz, string zap)
            {
                this.foo = foo;
                this.baz = baz;
                this.zap = zap;
            }
        }

        // Profile class
        class Profile
        {
            // Member variables
            string email;
            int uid;
            string role;

            // Constructor
            public Profile(string email, int uid, string role)
            {
                this.email = email;
                this.uid = uid;
                this.role = role;
            }
        }

        private string ConvertCookieToJSON(string text)
        {
            // Get each key value pair
            string[] kvps = text.Split('&');

            // Reduce each key value pair to just the value and assign it (we're assuming they come in the correct order)
            string[] vals = new string[kvps.Length];
            for(int i = 0; i < kvps.Length; i++)
            {
                // Split around the equals sign and replace the kvp with just the value
                vals[i] = kvps[i].Split('=')[1];
            }

            // Create the object
            Cookie cookie = new Cookie(vals[0], vals[1], vals[2]);

            // Convert the object to JSON
            return JsonSerializer.Serialize(cookie);
        }
    }
}

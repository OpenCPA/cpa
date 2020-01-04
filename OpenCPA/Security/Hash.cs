using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace OpenCPA.Security
{
    public class Hash
    {
        /// <summary>
        /// Create a hash from a password. Minimum strength 1000.
        /// </summary>
        public static string Create(string data, int strength, byte[] constSalt = null)
        {
            if (strength < 1000)
            {
                throw new Exception("Cryptographic strength for hashing must be >=1000.");
            }
            if (constSalt != null && constSalt.Length != 16)
            {
                throw new Exception("Constant salt must be 16 bytes long.");
            }

            //Create a salt from the PRNG provider.
            byte[] salt = new byte[16];
            if (constSalt == null)
            {
                var prng = new RNGCryptoServiceProvider();
                prng.GetBytes(salt);
            }
            else
            {
                salt = constSalt;
            }

            //Get the hash bytes (along with the salt) for return.
            var pbkdf2 = new Rfc2898DeriveBytes(data, salt, strength);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            //Return as a base 64 string.
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Verifies a raw password against a hash.
        /// </summary>
        public static bool Verify(string data, string hashStr, int strength)
        {
            //Get the salt from the base64 string.
            byte[] hashBytes = Convert.FromBase64String(hashStr);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            //Hash the data that's being compared with the same salt.
            var verifyHash = new Rfc2898DeriveBytes(data, salt, strength);
            byte[] hash = verifyHash.GetBytes(20);

            //See if they're the same.
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a cryptographically secure string of a given length, and charset (if provided).
        /// </summary>
        public static string GetCryptoSecureString(int length, string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_")
        {
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[length];

                //If chars.Length isn't a power of 2 then there is a bias if we simply use the modulus operator. The first characters of chars will be more probable than the last ones.
                //buffer used if we encounter an unusable random byte. We will regenerate it in this buffer
                byte[] buffer = null;

                //Maximum random number that can be used without introducing a bias
                int maxRandom = byte.MaxValue - ((byte.MaxValue + 1) % chars.Length);

                crypto.GetBytes(data);

                char[] result = new char[length];

                for (int i = 0; i < length; i++)
                {
                    byte value = data[i];

                    while (value > maxRandom)
                    {
                        if (buffer == null)
                        {
                            buffer = new byte[1];
                        }

                        crypto.GetBytes(buffer);
                        value = buffer[0];
                    }

                    result[i] = chars[value % chars.Length];
                }

                return new string(result);
            }
        }
    }
}
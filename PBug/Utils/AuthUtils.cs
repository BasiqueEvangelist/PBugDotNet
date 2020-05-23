using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace PBug.Utils
{
    public static class AuthUtils
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public static byte[] GetHashFor(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, 100000, 64);
        }

        public static byte[] GetRandomData(int length)
        {
            byte[] data = new byte[length];
            rng.GetBytes(data);
            return data;
        }

        public static Claim GetClaim(this ClaimsPrincipal principal, string id)
        {
            return principal.Claims.First(x => x.Type == id);
        }

        public static int GetUserId(this ClaimsPrincipal principal)
        {
            return int.Parse(principal.GetClaim(ClaimTypes.PrimarySid).Value);
        }

        public static bool IsAnonymous(this ClaimsPrincipal principal)
        {
            return principal.HasClaim(x => x.Type == ClaimTypes.Anonymous);
        }
    }
}
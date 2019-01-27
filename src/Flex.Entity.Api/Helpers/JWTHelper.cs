using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using JWT;

namespace Flex.Entity.Api.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class JwtHelper
    {
        /// <summary>
        /// Generate a JWT Token based on supplied keys
        /// </summary>
        /// <param name="symmetricKey"></param>
        /// <param name="audience"></param>
        /// <param name="issuer"></param>
        /// <returns></returns>
        public static string GenerateToken(string symmetricKey, string audience, string issuer)
        {
            if (string.IsNullOrWhiteSpace(symmetricKey) || string.IsNullOrWhiteSpace(audience))
                throw new ArgumentNullException(nameof(symmetricKey));
            byte[] secretKey = Base64UrlDecode(symmetricKey);
            DateTime issued = DateTime.Now;
            DateTime expire = DateTime.Now.AddHours(10);

            var userMetadata = new Dictionary<string, object>()
            {
                {"client", "oe"},
                {"defaultapi", "entities"},
                {"defaultload", "hotwater"},
                {"color", "red"}
            };

            var asset = new Dictionary<string, object>()
            {
                {"root", "jsb"},
                {"permission", 9}
            };
            var serrvice = new Dictionary<string, object>()
            {
                {"root", "oe"},
                {"permission", 9}
            };
            var appMetadata = new Dictionary<string, object>()
            {
                {"asset", asset},
                {"service", serrvice},

            };
            var payload = new Dictionary<string, object>()
            {
                {"iss", issuer ?? string.Empty},
                {"aud", audience},
                {"sub", "anonymous"},
                {"iat", ToUnixTime(issued).ToString()},
                {"exp", ToUnixTime(expire).ToString()},
                {"email", "nooruddin.kapasi@gmail.com" },
                { "email_verified", true},
                {"user_id", "auth0|57e00a551cc9858c1eb8b86a" },
                {"name", "nooruddin.kapasi@gmail.com" },
                {"picture", "https://s.gravatar.com/avatar/74c0ad4e45e392ab06f7960a44d92796?s=480&r=pg&d=https%3A%2F%2Fcdn.auth0.com%2Favatars%2Fno.png" },
                { "nickname", "nooruddin.kapasi"},
                { "updated_at", "2016-10-04T15:14:42.708Z"},
                { "created_at", "2016-09-19T15:55:01.541Z"},
                { "user_metadata",userMetadata },
                { "app_metadata",appMetadata},
                {"global_client_id", "JP1qnou00u0EvhfaEruOi1eCaCsDXWDJ" },

            };

            string token = JsonWebToken.Encode(payload, secretKey, JwtHashAlgorithm.HS256);
            return token;
        }

        /// <remarks>
        /// Take from http://stackoverflow.com/a/33113820
        /// </remarks>
        private static byte[] Base64UrlDecode(string arg)
        {
            string s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default:
                    throw new Exception(
                        "Illegal base64url string!");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        }

        private static long ToUnixTime(DateTime dateTime)
        {
            return (int)(dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
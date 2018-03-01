using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApprovaOSAuthService.Core
{
    public class TokenHandler
    {
        public bool ValidateTokenAsync(string authorizationHeader)
        {
            try
            {
                string token = GetBearerToken(authorizationHeader);
                if (token == null)
                {
                    return false;
                }
                else
                    return true;

               // var jwks = FetchJwks();

                //return ValidateToken(jwks, token);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetBearerToken(string authorizationHeader)
        {
            string token = null;

            token = authorizationHeader.ToString().Substring("Bearer ".Length).Trim();

            return token;
        }

        private bool ValidateToken(string jwksJson, string token)
        {
            if (!token.Contains("."))
            {
                //Logger.error(MfaConstants.MfaEndpointError.InvalidToken);
                return false;
            }

            var signedKey = GetSecurityKey(jwksJson, token);
            if (signedKey == null)
            {
                //Logger.error(MfaConstants.MfaEndpointError.SigninKeyNull);
                return false;
            }

            return ValidateTokenAgainstIssuer(token, signedKey);
        }

        private static bool ValidateTokenAgainstIssuer(string token, SecurityKey signedKey)
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = true,
                ValidAudience = "MingleTechStackService",
                IssuerSigningKey = signedKey,
                RequireExpirationTime = true
            };

            SecurityToken validatedToken;
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                var audClaim = claimsPrincipal.FindAll("aud");
                return true;
            }
            catch (Exception e)
            {
                //Logger.error("message ::" + e.Message);
                return false;
            }
        }

        private static SecurityKey GetSecurityKey(string jwksJson, string token)
        {
            string[] tokenParts = token.Split('.');
            JwtHeader header = JwtHeader.Base64UrlDeserialize(tokenParts[0]);

            JsonWebKeySet jwks = new JsonWebKeySet(jwksJson);
            IList<SecurityKey> keyList = jwks.GetSigningKeys();
            SecurityKey signedKey = null;
            for (int index = 0; index < keyList.Count; index++)
            {
                SecurityKey key = keyList[index];

                if (StringComparer.Ordinal.Equals(key.KeyId, header.Kid))
                {
                    signedKey = key;
                    break;
                }
            }

            return signedKey;
        }

        private string FetchJwks()
        {
            string jwksEndPoint = GetJwksUrlFromDiscovery();

            return GetJwksResponse(jwksEndPoint);

        }

        private string GetJwksResponse(string jwksEndPoint)
        {
            var req = WebRequest.Create(jwksEndPoint) as HttpWebRequest;
            string result = null;
            using (var resp = req.GetResponse() as HttpWebResponse)
            {
                var reader = new StreamReader(resp.GetResponseStream());
                result = reader.ReadToEnd();
            }
            return result;
        }

        private string GetJwksUrlFromDiscovery()
        {
            var wellknownEndpoint = "";// ConfigurationManager.AppSettings["WellKnownEndPoint"];
            var request = WebRequest.Create(new Uri(wellknownEndpoint)) as HttpWebRequest;

            string response = null;
            using (var resp = request.GetResponse() as HttpWebResponse)
            {
                using (var reader = new StreamReader(resp.GetResponseStream()))
                {
                    response = reader.ReadToEnd();
                    reader.Close();
                    resp.Close();
                }

            }
            var jObject = JObject.Parse(response);
            var jwksEndPoint = jObject.GetValue("jwks_uri").ToString();
            return jwksEndPoint;
        }
    }
}

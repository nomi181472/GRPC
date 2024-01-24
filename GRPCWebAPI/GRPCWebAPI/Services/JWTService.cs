//JWTService.cs
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
namespace GRPCWebAPI.Services
{
    public interface IJWTService
    {
        string GenerateJWTToken(Dictionary<string, string> parameters);
    }
    
    public class JWTService : IJWTService
    {
        readonly public static string secretKey = "blahblahblashblahblahblashblahblahblashblahblahblashblahblahblashblahblahblashblahblahblash"; //for only demo purpose
        readonly public static string issuer = "https://www.botonetics.com";
        private static List<Claim> GetClaims(Dictionary<string, string> parameters)
        {
            List<Claim> claims = new List<Claim>();
            foreach (var dict in parameters)
            {
                claims.Add(new Claim(dict.Key, dict.Value));
            }

            return claims;
        }
        public string GenerateJWTToken(Dictionary<string, string> parameters)
        {
            List<Claim> claims = GetClaims(parameters);
            
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(36), // Token expiration time
                signingCredentials: creds
            );


            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;

        }
    }
}

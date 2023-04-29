using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiemNgoaiKhoa.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;

namespace DiemNgoaiKhoa.Helpers
{
    public static class Helper
    {
        public static string GenerateToken(Account user, Role role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("this is my custom Secret key for authnetication");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name,user.Username.ToString()),
                        new Claim(ClaimTypes.Role,role.Name.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async static Task SignInAsync(this HttpContext context, Account user, Role role)
        {
            ClaimsIdentity identity;
            var userClaims = new List<Claim>()
                 {
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim(ClaimTypes.Role,role.Name.ToString())
                };
            identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(new[] { identity });
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                     userPrincipal,
                     new AuthenticationProperties { IsPersistent = true });
        }
        public static string Hash128(string input)
        {
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
            }
        }
    }
}
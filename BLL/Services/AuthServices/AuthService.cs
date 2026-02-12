using DAL.Repository.AuthRepositories;
using DAL.Repository.UserRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.Auth_Models;
using Models.Post_Models;
using Models.User_Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;
        public AuthService(IAuthRepository repository,  IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }


        public async Task<string?> RegisterAsync(RegisterForm form)
        {
            try
            {
                var salt = Guid.NewGuid();
                var passwordHash = HashPassword(form.Password, salt);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = form.Username,
                    Email = form.Email,
                    Address = form.Adress,
                    Salt = salt,
                    PasswordHash = passwordHash,
                    Role = "user",
                    Posts = new List<Post>()
                };

                var createdUser = await _repository.RegisterUserAsync(user);

                return GenerateToken(user);
            }
            catch(Exception ex)
            {
                return null;
            }
        }


        public async Task<string?> LoginAsync(Loginform form)
        {
            try
            { 
                var user = await _repository.GetUserByCredentialsAsync(form.Email);
                if (user == null)
                    return null;

                var hash = HashPassword(form.Password, user.Salt);
                if (user.PasswordHash != hash)
                    return null;

                return GenerateToken(user);
            }
            catch(Exception ex)
            {
                return null;
            }
        }



        private string HashPassword(string password, Guid salt)
        {
            using var sha256 = SHA256.Create();
            var combined = password + salt.ToString();
            var bytes = Encoding.UTF8.GetBytes(combined);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }


        private UserRead ToUserRead(User user)
        {
            return new UserRead
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Address = user.Address,
                Role = user.Role,
                Posts = user.Posts
            };
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("username", user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(120),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

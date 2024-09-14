using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AssignmentApp.Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _userJsonPath = @"C:\Users\Rock\source\repos\AssignmentApp\AssignmentApp\bin\Debug\net8.0\Data\users.json";

    private readonly IConfiguration _configuration;

    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Step 1: Get user by username and password
    public async Task<User> GetUserAsync(string username, string password)
    {
        var users = await LoadUsersFromJsonAsync();
        return users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }

    // Step 2: Generate JWT Token after successful authentication
    public string GenerateJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Key"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role) // Set role-based claims
            }),
            Expires = DateTime.UtcNow.AddHours(2), // Token expiration time
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    // Step 3: Load users from JSON file
    private async Task<List<User>> LoadUsersFromJsonAsync()
    {
        var json = await File.ReadAllTextAsync(_userJsonPath);
        return JsonSerializer.Deserialize<List<User>>(json);
    }
}


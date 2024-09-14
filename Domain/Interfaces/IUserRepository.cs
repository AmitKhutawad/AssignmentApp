using AssignmentApp.Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserAsync(string username, string password);
    string GenerateJwtToken(User user);
}

namespace ExpenseTrackerAPI.Repositories;

public interface IUserRepository
{
    Task<User> AddUser(string email, string username, string password);
    Task<User?> GetUserByEmail(string email);
}

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = [];
    
    public async Task<User> AddUser(string email, string username, string password)
    {
        if (string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(username))
        {
            throw new Exception("Invalid data");
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Username = username,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(password)
        };
        
        _users.Add(user);
        return await Task.FromResult(user);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var user = _users.FirstOrDefault(u => u.Email == email);
        return await Task.FromResult(user);
    }
}
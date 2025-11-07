using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ExpenseTrackerAPI.Repositories;

public interface IUserRepository
{
    Task<User?> AddUser(string email, string username, string password);
    Task<User?> GetUserByEmail(string email);
}

public class UserRepository(IConfiguration configuration) : IUserRepository
{
    private readonly List<User> _users = [];

    public async Task<User?> AddUser(string email, string username, string password)
    {
        if (string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("One or more arguments are blank");
        }
        
        var param = new { email, username, hashedPassword = BCrypt.Net.BCrypt.HashPassword(password) };

        await using var conn = new SqlConnection(configuration.GetConnectionString("ExpenseTracker")!);
        await conn.OpenAsync();

        var user = await conn.QueryFirstOrDefaultAsync<User>("sp_CreateUserIfEmailDoesNotExists", param,
            commandType: CommandType.StoredProcedure);
        return user;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var sql = "SELECT u.UserId, u.Username, u.Email, u.HashedPassword FROM Users u WHERE Email = @email";

        await using var conn = new SqlConnection(configuration.GetConnectionString("ExpenseTracker")!);
        await conn.OpenAsync();

        var user = await conn.QueryFirstOrDefaultAsync<User>(sql, new { email });
        return user;
    }
}
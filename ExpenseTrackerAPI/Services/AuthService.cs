using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Repositories;

namespace ExpenseTrackerAPI.Services;

public interface IAuthService
{
    Task<string> Login(LoginDTO login);
    Task<string> Register(RegisterDTO register);
}

public class AuthService(IUserRepository userRepository, TokenProvider tokenProvider) : IAuthService
{
    public async Task<string> Login(LoginDTO login)
    {
        var user = await userRepository.GetUserByEmail(login.Email) 
                   ?? throw new LoginException("User not found", login.Email);

        var passwordVerified = BCrypt.Net.BCrypt.Verify(login.Password, user.HashedPassword);
        if (!passwordVerified)
            throw new LoginException("Incorrect password", login.Email);

        var token = tokenProvider.Create(user);
        return await Task.FromResult(token);
    }

    public async Task<string> Register(RegisterDTO register)
    {
        var user = await userRepository.AddUser(register.Email, register.Username, register.Password);

        var token = tokenProvider.Create(user);
        return await Task.FromResult(token);
    }
}

public class LoginException(string message, string email) : Exception(message)
{
    public string Email { get; } = email;
}
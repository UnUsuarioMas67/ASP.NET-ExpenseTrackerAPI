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
                   ?? throw new AuthenticationException("User not found", login.Email, AuthenticationActions.Login);

        var passwordVerified = BCrypt.Net.BCrypt.Verify(login.Password, user.HashedPassword);
        if (!passwordVerified)
            throw new AuthenticationException("Incorrect password", login.Email, AuthenticationActions.Login);

        var token = tokenProvider.Create(user);
        return await Task.FromResult(token);
    }

    public async Task<string> Register(RegisterDTO register)
    {
        try
        {
            var user = await userRepository.AddUser(register.Email, register.Username, register.Password);
            
            var token = tokenProvider.Create(user);
            return token;
        }
        catch (DuplicateEmailException)
        {
            throw new AuthenticationException("Email already registered", register.Email, AuthenticationActions.Register);
        }
    }
}

public class AuthenticationException(string message, string email, AuthenticationActions action)
    : Exception($"{action.ToString()} Error: {message}")
{
    public string Email { get; } = email;
    public string InnerMessage { get; } = message;
    public AuthenticationActions AuthenticationAction { get; } = action;
}

public enum AuthenticationActions
{
    Login,
    Register,
}
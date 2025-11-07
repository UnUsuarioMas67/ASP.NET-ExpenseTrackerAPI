using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Repositories;

namespace ExpenseTrackerAPI.Services;

public interface IAuthService
{
    Task<Result<string>> Login(LoginRequest login);
    Task<Result<string>> Register(RegisterRequest register);
}

public class AuthService(IUserRepository userRepository, TokenProvider tokenProvider) : IAuthService
{
    public async Task<Result<string>> Login(LoginRequest login)
    {
        var user = await userRepository.GetUserByEmail(login.Email);
        if (user == null)
            return Result<string>.Failure("User not found");

        var passwordVerified = BCrypt.Net.BCrypt.Verify(login.Password, user.HashedPassword);
        if (!passwordVerified)
            return Result<string>.Failure("Incorrect password");

        var token = tokenProvider.Create(user);
        return Result<string>.Success(tokenProvider.Create(user));
    }

    public async Task<Result<string>> Register(RegisterRequest register)
    {
        var user = await userRepository.AddUser(register.Email, register.Username, register.Password);

        if (user == null)
            return Result<string>.Failure("Email already registered");

        return Result<string>.Success(tokenProvider.Create(user));
    }
}
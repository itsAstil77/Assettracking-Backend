using AssetTrackingAuthAPI.Config;
using AssetTrackingAuthAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AssetTrackingAuthAPI.Services;

public class AuthService
{
    private readonly IMongoCollection<User> _users;
    private readonly EmailService _emailService;
    private readonly TokenService _tokenService;


    public AuthService(IOptions<MongoDbSettings> dbSettings, EmailService emailService , TokenService tokenService)
    {
        var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
        var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        _users = database.GetCollection<User>(dbSettings.Value.UserCollection);
        _emailService = emailService;
        _tokenService = tokenService;
    }

    public async Task<string> CreateUserAsync(CreateUserRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            return "Passwords do not match";

        if (!IsValidPassword(request.Password))
            return "Password must be at least 6 characters, include uppercase, lowercase, number, and special character.";

        var existingUser = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
        if (existingUser != null)
            return "Email already exists";

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            Password = hashedPassword,
            AssignedRoles = request.AssignedRoles,
            OTP = "",
            OTPExpiry = DateTime.UtcNow
        };

        await _users.InsertOneAsync(newUser);
        return "User created successfully";
    }

    private bool IsValidPassword(string password)
    {
        return password.Length >= 6 &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit) &&
               password.Any(ch => !char.IsLetterOrDigit(ch));
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            return "Invalid credentials";

        var otp = new Random().Next(1000, 9999).ToString();
        user.OTP = otp;
        user.OTPExpiry = DateTime.UtcNow.AddMinutes(1);
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user);

        await _emailService.SendEmailAsync(
            email,
            "Your OTP Code",
            $"Your one-time password is: <b>{otp}</b>. It is valid for 1 minute.");

        return "OTP sent to email";
    }

    public async Task<string> VerifyOtpAsync(string email, string otp)
    {
        var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (user == null || user.OTP != otp || user.OTPExpiry < DateTime.UtcNow)
            return "Invalid or expired OTP";

        // OTP verified, generate JWT token
        var token = _tokenService.GenerateToken(user);
        return token;  // return the JWT token on successful OTP verification
    }

    public async Task<string> ResendOtpAsync(string email)
    {
        var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (user == null) return "User not found";

        var otp = new Random().Next(1000, 9999).ToString();
        user.OTP = otp;
        user.OTPExpiry = DateTime.UtcNow.AddMinutes(1);
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user);

        await _emailService.SendEmailAsync(
            email,
            "Your OTP Code",
            $"Your one-time password is: <b>{otp}</b>. It is valid for 1 minute.");

        return "OTP resent";
    }
}

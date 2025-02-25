namespace BLL.Services;

using BCrypt.Net;

public class PasswordService
{
    // Hash a password
    public static string HashPassword(string plainPassword)
    {
        string hashedPassword = BCrypt.HashPassword(plainPassword);
        Console.WriteLine(hashedPassword);
        return hashedPassword;
    }

    // Verify if the password matches the hashed password
    public static bool VerifyPassword(string plainPassword, string hashedPassword)
    {
        return BCrypt.Verify(plainPassword, hashedPassword);
    }
}
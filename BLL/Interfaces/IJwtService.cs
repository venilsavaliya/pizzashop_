namespace BLL.Interfaces;

public interface IJwtService
{
     public string GenerateJwtToken(string Email, string Role);
     public  bool IsTokenExpired(string token);
}

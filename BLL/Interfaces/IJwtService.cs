namespace BLL.Interfaces;

public interface IJwtService
{
     public string GenerateJwtToken(string Email, string Role,int day=7);
     public  bool IsTokenExpired(string token);
     public string GetEmailDetailsFromToken(string Token);
}

using DAL.Models;
namespace DAL.Interfaces;

public interface IUserRepository
{
    User GetUserByEmail(string email);
}

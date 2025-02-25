using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
namespace BLL.Interfaces;

public interface IUserService
{
     User GetUserByEmail(string email);

     Userdetail GetUserDetailByemail(string email);

    public Task<UserListViewModel> GetUserList(string sortColumn, string sortOrder, int pageNumber = 1, int pageSize = 2, string searchKeyword = "");

    public Task<AuthResponse> AddUser(AddUserViewModel model);
}

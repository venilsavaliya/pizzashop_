using BLL.Models;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
namespace BLL.Interfaces;

public interface IUserService
{
    User GetUserByEmail(string email);

    Userdetail GetUserDetailByemail(string email);

    Userdetail GetUserDetailById(string id);

    // public Task<UserListViewModel> GetUserList(string sortColumn, string sortOrder, int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    public Task<UserListPaginationViewModel> GetUserList(string sortColumn, string sortOrder, int pageNumber = 1, int pageSize = 2, string searchKeyword = "");

    public Task<AuthResponse> AddUser(EditUserViewModel model);

    public EditUserViewModel GetEditUserById(string id);

    public Task<AuthResponse> EditUser(EditUserViewModel user);

    public Task<AuthResponse> ChangeProfilePassword(ChangePasswordViewModel model);

    public Task<AuthResponse> UpdateUserProfile(UpdateUserViewModel model);

    public AuthResponse DeleteUserById(string id);

    public Guid GetUserIdfromToken(string token);

    public string UploadFile(IFormFile file);
}

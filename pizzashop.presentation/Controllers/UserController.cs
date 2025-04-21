using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using AspNetCoreHero.ToastNotification.Abstractions;
using BLL.Attributes;
using BLL.Interfaces;
using BLL.Models;
using DAL.Constants;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class UserController : BaseController
{

    private readonly IUserService _userService;

    private readonly INotyfService _notyf;
    public UserController(IJwtService jwtService, IAdminService adminservice, IUserService userService, INotyfService notyfy, BLL.Interfaces.IAuthorizationService authservice) : base(jwtService, userService, adminservice, authservice)
    {
        _userService = userService;
        _notyf = notyfy;
    }

    #region Profile
    // [Authorize(Roles="Admin")]
    // GET : User/Profile
    public IActionResult Profile()
    {
        var token = Request.Cookies["jwt"];
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        var user = _userService.GetUserDetailByemail(email);

        if (user == null)
        {

            return RedirectToAction("Login", "Auth");
        }
        else
        {
            return View(user);
        }
    }

    // POST : User/UpdateProfile
    // [Authorize(Roles="Admin")]
    [HttpPost]
    public async Task<IActionResult> UpdateProfile(UpdateUserViewModel model)
    {

        if (!ModelState.IsValid)
        {
            var user = _userService.GetUserDetailById(model.Id.ToString());
            return View("Profile", user);
        }


        var AuthResponse = _userService.UpdateUserProfile(model).Result;
        if (!AuthResponse.Success)
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
            var user = _userService.GetUserDetailById(model.Id.ToString());
            return View("Profile", user);
        }

        TempData["ToastrType"] = "success";
        TempData["ToastrMessage"] = AuthResponse.Message;
        return RedirectToAction("Profile", "User");
    }

    // GET : User/ChangePassword
    // [Authorize(Roles="Admin")]
    public IActionResult ChangePassword()
    {
        return View();
    }

    // POST : User/ChangePassword
    [HttpPost]
    // [Authorize(Roles="Admin")]
    public IActionResult ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var AuthResponse = _userService.ChangeProfilePassword(model).Result;

        if (!AuthResponse.Success)
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
            return View(model);
        }
        else
        {
            TempData["ToastrType"] = "success";
            TempData["ToastrMessage"] = AuthResponse.Message;
            return RedirectToAction("Profile", "User");
        }
    }

    #endregion

    #region  UserList

    // GET : User/userlist
    [AuthorizePermission(PermissionName.Users, ActionPermission.CanView)]
    public async Task<IActionResult> GetUserList(string sortColumn = "", string sortOrder = "", int pageNumber = 1, int pageSize = 5, string searchKeyword = "")
    {
        var userListViewModel = await _userService.GetUserList(sortColumn, sortOrder, pageNumber, pageSize, searchKeyword);
        return View(userListViewModel);
    }
    public async Task<IActionResult> UserListPV(string sortColumn = "", string sortOrder = "", int pageNumber = 1, int pageSize = 5, string searchKeyword = "")
    {
        var userListViewModel = await _userService.GetUserList(sortColumn, sortOrder, pageNumber, pageSize, searchKeyword);

        return PartialView("~/Views/User/_UserList.cshtml", userListViewModel);
    }
    #endregion

     #region  User CRUD
    // Get : Add Edit User Form
    [AuthorizePermission(PermissionName.Users, ActionPermission.CanAddEdit)]
    public IActionResult UserAddEditForm(string id)
    {
        if (id != null)
        {
            var edituser = _userService.GetEditUserById(id);
            return View(edituser);
        }
        else
        {
            var model = new EditUserViewModel();
            return View(model);

        }


    }


    //Post : Add Edit User Form Submission
    [HttpPost]
    [AuthorizePermission(PermissionName.Users, ActionPermission.CanAddEdit)]
    public async Task<IActionResult> AddEditUser(EditUserViewModel model)
    {
        AuthResponse response;
        if (model.Id != null)
        {
            response = await _userService.EditUser(model);

        }
        else
        {
            response = await _userService.AddUser(model);

        }

        return Json(new { message = response.Message, success = response.Success });


    }

    // Get : User/DeleteUser
    [AuthorizePermission(PermissionName.Users, ActionPermission.CanDelete)]
    public IActionResult DeleteUser(string id)
    {
        var AuthResponse = _userService.DeleteUserById(id);

        if (!AuthResponse.Success)
        {
            TempData["ToastrType"] = "error";
            TempData["ToastrMessage"] = AuthResponse.Message;
            return RedirectToAction("GetUserList", "User");
        }
        else
        {
            TempData["ToastrType"] = "success";
            TempData["ToastrMessage"] = AuthResponse.Message;
            return RedirectToAction("GetUserList", "User");
        }

    }

    #endregion

}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using AspNetCoreHero.ToastNotification.Abstractions;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class UserController : BaseController
{

    private readonly IUserService _userService;

    private readonly INotyfService _notyf;
    public UserController(IJwtService jwtService, IAdminService adminservice, IUserService userService, INotyfService notyfy) : base(jwtService, userService, adminservice)
    {
        _userService = userService;
        _notyf = notyfy;
    }

    #region Profile
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

    public IActionResult ChangePassword()
    {
        return View();
    }

    // POST : User/ChangePassword
    [HttpPost]
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
    public async Task<IActionResult> GetUserList(string sortColumn, string sortOrder, int pageNumber = 1, int pageSize = 5, string searchKeyword = "")
    {
        var userListViewModel = await _userService.GetUserList(sortColumn, sortOrder, pageNumber, pageSize, searchKeyword);
        return View(userListViewModel);
    }

    // GET : User/AddUser
    public IActionResult AddUser()
    {
        return View();
    }


    // POST : User/AddUser

    [HttpPost]

    public IActionResult AddUser(AddUserViewModel model)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var AuthResponse = _userService.AddUser(model).Result;

        if (!AuthResponse.Success)
        {
            _notyf.Error(AuthResponse.Message);
            return View(model);
        }
        else
        {
            return RedirectToAction("GetUserList", "user");
        }

    }


    // GET : User/EditUser

    public IActionResult EditUser(string id)
    {
        var edituser = _userService.GetEditUserById(id);
        return View(edituser);
    }

    // POST : User/EditUser
    [HttpPost]
    public IActionResult EditUser(EditUserViewModel model)
    {
        // if (!ModelState.IsValid)
        // {
        //     return View(model);
        // }

        var AuthResponse = _userService.EditUser(model).Result;

        if (!AuthResponse.Success)
        {
            _notyf.Error(AuthResponse.Message);
            return View(model);
        }
        else
        {
            return RedirectToAction("GetUserList", "user");
        }
    }





    // Get : User/DeleteUser

    public IActionResult DeleteUser(string id)
    {
        var AuthResponse = _userService.DeleteUserById(id);

        if (!AuthResponse.Success)
        {
            _notyf.Error("Could Not Delete User");
            return RedirectToAction("GetUserList", "User");
        }
        else
        {
            _notyf.Success("User Deleted Successfully");
            return RedirectToAction("GetUserList", "User");
        }

    }

    #endregion

}

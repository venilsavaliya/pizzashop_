using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace pizzashop.presentation.Controllers;

public class UserController : Controller
{

    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }


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
    public IActionResult UpdateProfile(UpdateUserViewModel model)
    {
        var AuthResponse = _userService.UpdateUserProfile(model).Result;
        if (!AuthResponse.Success)
        {
            return View(model);
        }
        return RedirectToAction("Profile", "User");
    }

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
            return View(model);
        }
        else
        {
            return RedirectToAction("GetUserList", "user");
        }
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
            return View(model);
        }
        else
        {
            return RedirectToAction("Profile", "User");
        }
    }

    // Get : User/DeleteUser

    public IActionResult DeleteUser(string id)
    {
        var AuthResponse = _userService.DeleteUserById(id);

        if (!AuthResponse.Success)
        {
            return RedirectToAction("GetUserList", "User");
        }
        else
        {
            return RedirectToAction("GetUserList", "User");
        }

    }

}

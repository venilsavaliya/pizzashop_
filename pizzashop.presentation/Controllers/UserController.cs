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
        var email = Request.Cookies["email"];

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

        // if(!ModelState.IsValid)
        // {
        //     return View(model);
        // }

        var AuthResponse = _userService.AddUser(model).Result;

        if (!AuthResponse.Success)
        {
            return View(model);
        }
        else{
            return RedirectToAction("GetUserList","user");
        }

    }
}

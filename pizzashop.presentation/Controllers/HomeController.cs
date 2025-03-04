using System.Diagnostics;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pizzashop.presentation.Models;

namespace pizzashop.presentation.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;

    private readonly IJwtService _jwtservice;

    public HomeController(ILogger<HomeController> logger,IJwtService jwtService,IUserService userService,IAdminService adminservice) : base(jwtService,userService,adminservice)
    {
        _logger = logger;
        _jwtservice = jwtService;
    }
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

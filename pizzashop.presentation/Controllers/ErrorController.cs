using System.Diagnostics;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pizzashop.presentation.Models;

namespace pizzashop.presentation.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IJwtService _jwtservice;

    public ErrorController(ILogger<HomeController> logger,IJwtService jwtService)
    {
        _logger = logger;
        _jwtservice = jwtService;
    }
    
   
    public IActionResult LinkExpired()
    {
        return View();
    }

   
}

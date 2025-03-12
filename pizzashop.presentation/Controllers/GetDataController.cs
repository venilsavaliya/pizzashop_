using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;

[Route("GetData")]
public class GetDataController : Controller
{
    private readonly IDataService _getDataService;

    private readonly IMenuServices _menuService;

    public GetDataController(IDataService getDataService,IMenuServices menuServices)
    {
        _getDataService = getDataService;

        _menuService = menuServices;
    }

    [HttpGet("GetCountries")]
    public IActionResult GetCountries()
    {
        
        var countries = _getDataService.GetCountries();
        return Json(countries);
    }

    [HttpGet("GetStates")]
    public IActionResult GetStates(int countryId)
    {
        var states = _getDataService.GetStates(countryId);
        return Json(states);
    }

    [HttpGet("GetCities")]
    public IActionResult GetCities(int stateId)
    {
        var cities = _getDataService.GetCities(stateId);
        return Json(cities);
    }

    public IActionResult GetItems(int category)
    {
        
        var items = _menuService.GetItemsListByCategoryId(category);
        return Json(items);
    }
    [HttpGet("GetModifierGroupList")]
    public IActionResult GetModifierGroupList()
    {
       
        var groups = _getDataService.GetModifiersGroupList();
        return Json(groups);
    }
}

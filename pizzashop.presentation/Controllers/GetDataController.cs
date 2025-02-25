using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;

[Route("GetData")]
public class GetDataController : Controller
{
    private readonly IDataService _getDataService;

    public GetDataController(IDataService getDataService)
    {
        _getDataService = getDataService;
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
}

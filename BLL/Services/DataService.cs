using System.Text.Json.Nodes;
using BLL.Interfaces;
using DAL.Models;

namespace BLL.Services;

public class DataService : IDataService
{   
    private readonly ApplicationDbContext _context;
    public DataService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Country> GetCountries()
    {
         var countries = _context.Countries.ToList();
         return countries;
    }

    public IEnumerable<State> GetStates(int countryId)
    {
        var states = _context.States.Where(s => s.CountryId == countryId).ToList();
        return states;
    }

    public IEnumerable<City> GetCities(int stateId)
    {
        var cities = _context.Cities.Where(c => c.StateId == stateId).ToList();
        return cities;
    }

    

}

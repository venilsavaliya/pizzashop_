using System.Text.Json.Nodes;
using DAL.Models;

namespace BLL.Interfaces;

public interface IDataService
{
    IEnumerable<Country> GetCountries();
    IEnumerable<State> GetStates(int countryId);
    IEnumerable<City> GetCities(int stateId);
    IEnumerable<Modifiersgroup> GetModifiersGroupList();
}

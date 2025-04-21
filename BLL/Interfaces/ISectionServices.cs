namespace BLL.Interfaces;

using BLL.Models;
using DAL.Models;
using DAL.ViewModels;

public interface ISectionServices
{
    public Task<AuthResponse> AddSection(SectionNameViewModel model);

    public IEnumerable<SectionNameViewModel> GetSectionList();

    public Task<AddTableViewmodel> GetTableDetailById(int id);
    public  Task<SectionNameViewModel> GetSectionDetailById(int id);

    public IEnumerable<Tablestatus> GetTableStatusList();

    public TableListPaginationViewModel GetDiningTablesListBySectionId(int sectionid, int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    public Task<AuthResponse> AddTable(AddTableViewmodel model);

    public Task<AuthResponse> EditTable(AddTableViewmodel model);

    public Task<AuthResponse> EditSection(SectionNameViewModel model);

    public  Task<AuthResponse> DeleteSection(int id);

    public Task<AuthResponse> DeleteTable(int id);

    public Task<AuthResponse> DeleteTables(List<int> ids);

    public int GetTableStatusIdByName(string name);
    public Task<int> GetBusyTableCountOfSection(int sectionid = 0);
}

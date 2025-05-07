using BLL.Models;
using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppWaitingListService
{
   public Task<WaitingListMainViewModel> GetSectionList();

   public Task<List<WaitingTokenViewModel>> GetWaitingTokenList(int sectionid = 0);

   public Task<AuthResponse> AddWaitingToken(AddEditWaitingTokenViewModel model);

   public Task<AuthResponse> DeleteWaitingToken(int TokenId);
   
   public Task<List<TableViewModel>> GetAvailableTableList(int SectionId);

   public Task<AddEditWaitingTokenViewModel> GetAddEditWaitingTokenDetail(int id = 0);
}

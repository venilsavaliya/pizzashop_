using BLL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppMenuService
{

    public Task<List<OrderAppMenuItemViewModel>> GetMenuItem(int catid = 0, string searchkeyword = "", bool isfav = false);

    public Task<OrderAppModifierItemList> GetModifierGroupsByItemId(int itemId);

    public Task<AuthResponse> ChangeStatusOfFavouriteItem(int itemid);

}

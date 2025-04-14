using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppMenuService
{

    public Task<List<OrderAppMenuItemViewModel>> GetMenuItem(int catid = 0, string searchkeyword = "", bool isfav = false);

}

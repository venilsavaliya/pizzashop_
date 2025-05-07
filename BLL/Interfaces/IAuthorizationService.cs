using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IAuthorizationService
{
     bool HasPermission(string module, ActionPermission action);
}

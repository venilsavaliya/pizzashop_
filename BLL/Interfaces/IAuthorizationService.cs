using DAL.ViewModels;
using DAL.Constants;

namespace BLL.Interfaces;

public interface IAuthorizationService
{
     bool HasPermission(string module, ActionPermission action);
}

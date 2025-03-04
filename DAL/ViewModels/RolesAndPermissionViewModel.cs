namespace DAL.ViewModels;

public class RolesAndPermissionViewModel
{ 

    // public string? RoleName { get; set; }
    public string Id { get; set; }

    public string PermissionName { get; set; }

    public bool Canedit { get; set; }

    public bool Canview { get; set; }

    public bool Candelete { get; set; }

    public bool Isenable { get; set; } = false;
}

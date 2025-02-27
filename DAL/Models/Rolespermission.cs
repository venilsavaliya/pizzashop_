using System; 
using System.Collections.Generic;

namespace DAL.Models;

public partial class Rolespermission
{
    public int Id { get; set; }

    public Guid Roleid { get; set; }

    public int PermissionId { get; set; }

    public bool Canedit { get; set; }

    public bool Canview { get; set; }

    public bool Candelete { get; set; }

    public bool? Isenable { get; set; }

    public Guid? Modifiedby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}

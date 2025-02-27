using System;
using System.Collections.Generic;

namespace DAL.Models;
 
public partial class Permission
{
    public int PermissionId { get; set; }

    public string PermissionName { get; set; } = null!;

    public virtual ICollection<Rolespermission> Rolespermissions { get; } = new List<Rolespermission>();
}

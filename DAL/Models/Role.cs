using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Role
{
    public Guid Roleid { get; set; }

    public string? Name { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Rolespermission> Rolespermissions { get; } = new List<Rolespermission>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<Userdetail> Userdetails { get; } = new List<Userdetail>();
}

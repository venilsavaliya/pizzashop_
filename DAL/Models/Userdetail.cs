using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Userdetail
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string UserName { get; set; } = null!;

    public string? Phone { get; set; }

    public bool Status { get; set; }

    public string? Country { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    public string? Zipcode { get; set; }

    public Guid? RoleId { get; set; }

    public string? Profile { get; set; }

    public DateTime Createddate { get; set; }

    public Guid Createdby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public Guid? Modifiedby { get; set; }

    public bool? Isdeleted { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual User? ModifiedbyNavigation { get; set; }

    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }
}

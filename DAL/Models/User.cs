using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool? Rememberme { get; set; }

    public virtual ICollection<Category> CategoryCreatedbyNavigations { get; } = new List<Category>();

    public virtual ICollection<Category> CategoryModifyiedbyNavigations { get; } = new List<Category>();

    public virtual ICollection<Item> ItemCreatedbyNavigations { get; } = new List<Item>();

    public virtual ICollection<Item> ItemModifyiedbyNavigations { get; } = new List<Item>();

    public virtual ICollection<Role> RoleCreatedByNavigations { get; } = new List<Role>();

    public virtual ICollection<Role> RoleUpdatedByNavigations { get; } = new List<Role>();

    public virtual ICollection<Userdetail> UserdetailCreatedbyNavigations { get; } = new List<Userdetail>();

    public virtual ICollection<Userdetail> UserdetailModifiedbyNavigations { get; } = new List<Userdetail>();

    public virtual ICollection<Userdetail> UserdetailUsers { get; } = new List<Userdetail>();
}

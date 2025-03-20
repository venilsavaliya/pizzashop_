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

    public virtual ICollection<Diningtable> DiningtableCreatedbyNavigations { get; } = new List<Diningtable>();

    public virtual ICollection<Diningtable> DiningtableModifyiedbyNavigations { get; } = new List<Diningtable>();

    public virtual ICollection<Item> ItemCreatedbyNavigations { get; } = new List<Item>();

    public virtual ICollection<Item> ItemModifyiedbyNavigations { get; } = new List<Item>();

    public virtual ICollection<Modifieritem> ModifieritemCreatedbyNavigations { get; } = new List<Modifieritem>();

    public virtual ICollection<Modifieritem> ModifieritemModifyiedbyNavigations { get; } = new List<Modifieritem>();

    public virtual ICollection<Modifiersgroup> ModifiersgroupCreatedbyNavigations { get; } = new List<Modifiersgroup>();

    public virtual ICollection<Modifiersgroup> ModifiersgroupModifyiedbyNavigations { get; } = new List<Modifiersgroup>();

    public virtual ICollection<Role> RoleCreatedByNavigations { get; } = new List<Role>();

    public virtual ICollection<Role> RoleUpdatedByNavigations { get; } = new List<Role>();

    public virtual ICollection<Section> SectionCreatedbyNavigations { get; } = new List<Section>();

    public virtual ICollection<Section> SectionModifyiedbyNavigations { get; } = new List<Section>();

    public virtual ICollection<Taxis> TaxisCreatedbyNavigations { get; } = new List<Taxis>();

    public virtual ICollection<Taxis> TaxisModifyiedbyNavigations { get; } = new List<Taxis>();

    public virtual ICollection<Userdetail> UserdetailCreatedbyNavigations { get; } = new List<Userdetail>();

    public virtual ICollection<Userdetail> UserdetailModifiedbyNavigations { get; } = new List<Userdetail>();

    public virtual ICollection<Userdetail> UserdetailUsers { get; } = new List<Userdetail>();
}

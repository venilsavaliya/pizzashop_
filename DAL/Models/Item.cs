using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public int? CategoryId { get; set; }

    public string ItemName { get; set; } = null!;

    public string Type { get; set; } = null!;

    public short Rate { get; set; }

    public short Quantity { get; set; }

    public string Unit { get; set; } = null!;

    public bool DefaultTax { get; set; }

    public double TaxPercentage { get; set; }

    public string? ShortCode { get; set; }

    public bool Isavailable { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public DateTime Createddate { get; set; }

    public Guid Createdby { get; set; }

    public DateTime? Modifieddate { get; set; }

    public Guid? Modifyiedby { get; set; }

    public bool? Isdeleted { get; set; }

    public bool? Isfavourite { get; set; }

    public virtual Category? Category { get; set; }

    public virtual User CreatedbyNavigation { get; set; } = null!;

    public virtual ICollection<Dishritem> Dishritems { get; } = new List<Dishritem>();

    public virtual ICollection<ItemModifiergroupMapping> ItemModifiergroupMappings { get; } = new List<ItemModifiergroupMapping>();

    public virtual ICollection<Itemsminmaxmapping> Itemsminmaxmappings { get; } = new List<Itemsminmaxmapping>();

    public virtual ICollection<Itemsmodifiergroupminmaxmapping> Itemsmodifiergroupminmaxmappings { get; } = new List<Itemsmodifiergroupminmaxmapping>();

    public virtual ICollection<Itemsrmodifiersgroup> Itemsrmodifiersgroups { get; } = new List<Itemsrmodifiersgroup>();

    public virtual User? ModifyiedbyNavigation { get; set; }
}

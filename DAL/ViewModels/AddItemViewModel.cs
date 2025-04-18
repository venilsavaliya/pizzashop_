using System.ComponentModel.DataAnnotations;
using DAL.Models;
using Microsoft.AspNetCore.Http;

namespace DAL.ViewModels;


public class AddItemViewModel
{
    public int Id {get;set;} 
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Item Name is required")]
    public string ItemName { get; set; } 

    [Required(ErrorMessage = "Please select The Type")]
    public string Type { get; set; } 

    [Range(0, short.MaxValue, ErrorMessage = "Rate cannot be negative")]
    public short Rate { get; set; }

    [Range(0, short.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public short Quantity { get; set; }

    [Required(ErrorMessage = "Please Select The Unit")]
    public int Unit { get; set; } 

    public bool DefaultTax { get; set; }

    [Range(0, 100, ErrorMessage = "Tax Percentage must between 0-100")]
    public double TaxPercentage { get; set; }

    public string? ShortCode { get; set; }

    public bool Isavailable { get; set; }

    public string? Description { get; set; }

    public IFormFile? Image { get; set; }

    public List<int>? Modifiers {get;set;}

    public List<ModifierGroup>? ModifierGroups { get; set; }

    public List<CategoryNameViewModel> Categories {get;set;}
    public List<ModifierGroupNameViewModel> ModifierGroupNames {get;set;}

    public List<Unit> UnitsList {get;set;}

    // public DateTime Createddate { get; set; } = DateTime.Now;
}


public class ModifierGroup
{
    public int ModifierGroupId { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
}
using Microsoft.AspNetCore.Http;

namespace DAL.ViewModels;

public class AddItemViewModel
{
    public int? Id {get;set;}
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

    public IFormFile? Image { get; set; }

    public List<int>? Modifiers {get;set;}

    // public DateTime Createddate { get; set; } = DateTime.Now;

}

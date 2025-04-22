using System.ComponentModel.DataAnnotations;
using DAL.Models;

namespace DAL.ViewModels;

public class AddModifierItemViewModel
{
    public int ModifierId { get; set; }

    [Required(ErrorMessage = "Please Select Any Modifier Group")]
    public List<int> ModifierGroupid { get; set; }

    public string ModifierName { get; set; } = null!;

    [Required(ErrorMessage = "Rate is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Rate must be a non-negative number.")]
    public int Rate { get; set; }

    public string Unit { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative integer.")]

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public List<Unit> UnitsList {get;set;}


}

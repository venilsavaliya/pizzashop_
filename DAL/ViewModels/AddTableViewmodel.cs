using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class AddTableViewmodel
{
    public int? TableId { get; set; }

    [Required(ErrorMessage = "Please Select Section")]
    public int SectionId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } 

    [Required(ErrorMessage = "Capacity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be Greater Than zero")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public int Status { get; set; }
}

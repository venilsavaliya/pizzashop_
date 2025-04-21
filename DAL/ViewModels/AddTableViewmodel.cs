using System.ComponentModel.DataAnnotations;
using DAL.Models;

namespace DAL.ViewModels;

public class AddTableViewmodel
{
    public int TableId { get; set; }=0;

    [Required(ErrorMessage = "Please Select Section")]
    public int SectionId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } 

    [Required(ErrorMessage = "Capacity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be Greater Than zero")]
    public int Capacity { get; set; } =0;

    [Required(ErrorMessage = "Status is required.")]
    public int Status { get; set; }

    public List<Tablestatus> Tablestatuses = new List<Tablestatus>();
    public List<SectionNameViewModel> Sections = new List<SectionNameViewModel>();

}

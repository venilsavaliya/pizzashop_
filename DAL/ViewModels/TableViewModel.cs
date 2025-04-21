using DAL.Models;

namespace DAL.ViewModels;

public class TableViewModel
{
    public int TableId { get; set; }  

    public int? SectionId { get; set; }

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public string Status { get; set; } 

}

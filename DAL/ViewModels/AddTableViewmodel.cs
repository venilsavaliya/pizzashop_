namespace DAL.ViewModels;

public class AddTableViewmodel
{
    public int? TableId { get; set; }

    public int SectionId { get; set; }

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public string Status { get; set; } = null!;

}

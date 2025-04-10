namespace DAL.ViewModels;

public class WaitingTokenViewModel
{
    public int Tokenid { get; set; }

    public int sectionid {get;set;}

    public DateTime Createdat { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string Mobile { get; set; } = null!;

    public int? Totalperson { get; set; }
}
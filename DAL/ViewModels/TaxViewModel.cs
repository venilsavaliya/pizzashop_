namespace DAL.ViewModels;

public class TaxViewModel
{
    public int TaxId { get; set; } = 0;

    public string TaxName { get; set; } = null!;

    public string Type { get; set; } = null!;

    public decimal TaxAmount { get; set; }

    public bool Isenable { get; set; } 

    public bool Isdefault { get; set; }

}

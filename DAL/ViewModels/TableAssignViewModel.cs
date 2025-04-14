namespace DAL.ViewModels;

public class TableAssignViewModel 
{   
    public int? Tokenid { get; set; }
    public List<int> TableId { get; set; } 

    public int SectionId { get; set; }

    public AddCustomerViewModel Customer { get; set; } = new AddCustomerViewModel();

}


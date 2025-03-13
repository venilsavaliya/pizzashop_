namespace DAL.ViewModels;

public class ModifierGroupanditemsViewModel
{
    public string Name { get; set; } = null!;

    public int ModifierGroupId {get;set;} 

    public int? Minvalue {get;set;}

    public int? Maxvalue {get;set;}

    public List<ModifierItemsAndRate> items {get;set;} = new List<ModifierItemsAndRate>();
}

public class ModifierItemsAndRate
{
    public string Name {get;set;}

    public int Rate {get;set;}
}

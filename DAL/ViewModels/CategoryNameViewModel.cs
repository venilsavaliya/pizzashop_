namespace DAL.ViewModels;


public class CategoryListModel
{
    public List<CategoryNameViewModel> Catagories {get;set;}
}

public class CategoryNameViewModel
{
    public int Id {get;set;} = 0;

    public string Name {get;set;} 

    public string? Description {get;set;} 
 
}





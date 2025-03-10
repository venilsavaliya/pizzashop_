namespace DAL.ViewModels;

public class CategoryListViewModel
{
    public IEnumerable<CategoryNameViewModel>? Categories {get;set;}

    public string? SelectedCategory {get;set;}
}

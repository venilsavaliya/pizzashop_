namespace DAL.ViewModels;

public class CategoryListViewModel
{
    public IEnumerable<CategoryNameViewModel>? Categories {get;set;}

    public int? SelectedCategory {get;set;}
}

namespace DAL.ViewModels;

public class ModifierListViewModel
{
    public int? SelectedModifierGroup {get;set;}

    public IEnumerable<ModifierGroupNameViewModel> ModifierGroups {get;set;}
}

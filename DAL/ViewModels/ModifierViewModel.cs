namespace DAL.ViewModels;

public class ModifierViewModel
{
    public int? SelectedModifierGroup {get;set;}

    public IEnumerable<ModifierGroupNameViewModel> ModifierGroups {get;set;}

}
 
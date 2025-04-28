namespace DAL.ViewModels;

public class InstructionViewModel
{
    public string Instruction { get; set; } = string.Empty;
    public int OrderId { get; set; } = 0;

    public int DishId { get; set; } = 0;
    public int Index {get;set;} = 0;
}

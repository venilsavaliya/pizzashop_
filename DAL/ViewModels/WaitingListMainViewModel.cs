namespace DAL.ViewModels;

public class WaitingListMainViewModel
{
    public List<SectionListWaiting> section { get; set; }

    public int TotalWaitingToken { get; set; } = 0;
}

public class SectionListWaiting
{
    public int SectionId { get; set; }

    public string SectionName { get; set; } = null!;

    public int TotalWaitingToken { get; set; } = 0;
}
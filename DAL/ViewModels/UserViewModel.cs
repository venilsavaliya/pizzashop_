namespace DAL.ViewModels;

public class UserViewModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Status { get; set; }
    public string? Phone { get; set; }
    public bool? Isdeleted { get; set; }
}


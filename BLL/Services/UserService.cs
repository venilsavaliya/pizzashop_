using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using DAL;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using BLL.Models;
using BLL.Services;

public class UserService : IUserService
{


    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {

        _context = context;
    }

    public User GetUserByEmail(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        return user;
    }

    public Userdetail GetUserDetailByemail(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        var userdetail = _context.Userdetails.FirstOrDefault(u => u.UserId == user.Id);

        return userdetail;
    }

    public async Task<UserListViewModel> GetUserList(string sortColumn, string sortOrder, int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {
        searchKeyword = searchKeyword.ToLower();

        var query = from u in _context.Userdetails
                    join user in _context.Users on u.UserId equals user.Id
                    join role in _context.Roles on u.RoleId equals role.Roleid
                    select new UserViewModel
                    {
                        Id = u.Id,
                        Name = u.FirstName + " " + u.LastName,
                        Email = user.Email,
                        Role = role.Name,
                        Status = u.Status ? "Active" : "Inactive",
                        Phone = u.Phone
                    };

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(u => u.Name.ToLower().Contains(searchKeyword) ||
                                 u.Email.ToLower().Contains(searchKeyword) ||
                                 u.Role.ToLower().Contains(searchKeyword) ||
                                 u.Phone.Contains(searchKeyword));
        }

        // Sorting logic
        if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
        {
            switch (sortColumn)
            {
                case "Name":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name);
                    break;
                case "Role":
                    query = sortOrder == "asc" ? query.OrderBy(u => u.Role) : query.OrderByDescending(u => u.Role);
                    break;
            }
        }

        // Pagination
        int totalCount = query.Count();
        var usersList = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        return new UserListViewModel
        {
            Users = usersList,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            StartIndex = (pageNumber - 1) * pageSize + 1,
            EndIndex = Math.Min(pageNumber * pageSize, totalCount),
            SortColumn = sortColumn,
            SortOrder = sortOrder,
            SearchKeyword = searchKeyword
        };
    }


    public async Task<AuthResponse> AddUser(AddUserViewModel user)
    {
        
        var loggedinadmin = _context.Users.FirstOrDefault(u => u.Email == user.Email);
        user.Createdby = loggedinadmin.Id;

        var countryname = _context.Countries.FirstOrDefault(c => c.CountryId.ToString() == user.Country);
        var statename = _context.States.FirstOrDefault(c => c.StateId.ToString() == user.State);
        var cityname = _context.Cities.FirstOrDefault(c => c.CityId.ToString() == user.City);

        user.Country = countryname?.CountryName;
        user.State = statename?.StateName;
        user.City = cityname?.CityName;

        Console.WriteLine(user);

        var usercredential = new User
        {
            Email = user.Email,
            Password = PasswordService.HashPassword(user.Password)
        };

        _context.Users.Add(usercredential);
        await _context.SaveChangesAsync();

        var userid = _context.Users.FirstOrDefault(u => u.Email == user.Email).Id;

        var roleid = _context.Roles.FirstOrDefault(r => r.Name == user.RoleName).Roleid;

        var userdetail = new Userdetail
        {
            FirstName = user.FirstName,
            UserId = userid,
            LastName = user.LastName,
            UserName = user.UserName,
            Phone = user.Phone,
            Status = user.Status,
            Country = user.Country,
            State = user.State,
            City = user.City,
            Address = user.Address,
            Zipcode = user.Zipcode,
            RoleId = roleid,
            Profile = user.Profile,
            Createdby = user.Createdby
        };

        _context.Userdetails.Add(userdetail);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "User Added Successfully"
        };
    }
}

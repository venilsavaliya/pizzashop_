using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using DAL;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using BLL.Models;
using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class UserService : IUserService
{


    private readonly ApplicationDbContext _context;

    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IEmailService _emailService;

    private readonly IWebHostEnvironment _env;
    public UserService(ApplicationDbContext context, IHttpContextAccessor contextAccessor, IEmailService emailService, IWebHostEnvironment env)
    {

        _context = context;

        _httpContextAccessor = contextAccessor;

        _emailService = emailService;

        _env = env;
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
                    where u.Isdeleted == false
                    select new UserViewModel
                    {
                        Id = u.Id,
                        Name = u.FirstName + " " + u.LastName,
                        Email = user.Email,
                        Role = role.Name,
                        Status = u.Status ? "Active" : "Inactive",
                        Phone = u.Phone,
                        Profile = u.Profile,
                        Isdeleted = u.Isdeleted
                    };

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(u =>
                                 u.Name.ToLower().Contains(searchKeyword) ||
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
                default:
                    query = query.OrderBy(u => u.Id); // **Ensure a default sorting**
                    break;
            }
        }
        else
        {
            query = query.OrderBy(u => u.Id); // **Apply a default ordering if no sort is provided**
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

    public Guid GetUserIdfromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        return user.Id;
    }

    public async Task<AuthResponse> AddUser(AddUserViewModel user)
    {

        // logic for checking unique username
        if (_context.Userdetails.Any(u => u.UserName == user.UserName))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Username already exists"
            };
        }
        // logic for checking unique phone 
        if (_context.Userdetails.Any(u => u.Phone == user.Phone))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Phone already exists"
            };
        }
        // logic for checking unique email
        if (_context.Users.Any(u => u.Email == user.Email))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Email already exists"
            };
        }

        // getting jwt token from cookies
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwt"];

        // decode jwt token to get email from it
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        // getting id of logged in admin
        var loggedinadmin = _context.Users.FirstOrDefault(u => u.Email == email);


        // fetching country, state and city names
        var countryname = _context.Countries.FirstOrDefault(c => c.CountryId.ToString() == user.Country);
        var statename = _context.States.FirstOrDefault(c => c.StateId.ToString() == user.State);
        var cityname = _context.Cities.FirstOrDefault(c => c.CityId.ToString() == user.City);

        user.Country = countryname?.CountryName;
        user.State = statename?.StateName;
        user.City = cityname?.CityName;

        string url ="";

        if(user.Profile!=null)
        {
            url = UploadFile(user.Profile);
        }

        var usercredential = new User
        {
            Email = user.Email,
            Password = PasswordService.HashPassword(user.Password)
        };

        // save new user credentials in users table
        _context.Users.Add(usercredential);
        await _context.SaveChangesAsync();


        // fetch html template from root folder
        string htmltemplate = System.IO.File.ReadAllText(_env.WebRootPath + "/HtmlTemplate/AccountCreated.html");

        htmltemplate=htmltemplate.Replace("_username_", user.UserName);
        htmltemplate=htmltemplate.Replace("_password_", user.Password);


        // send email to the newly created user
        await _emailService.SendEmailAsync(user.Email, "Account Credentials", htmltemplate);

        // get userid and role id to give reffrence to userdetail table 
        var userid = _context.Users.FirstOrDefault(u => u.Email == user.Email)?.Id;
        var roleid = _context.Roles.FirstOrDefault(r => r.Name == user.RoleName)?.Roleid;


        // creating new userdetail object which will store in db
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
            Profile = url,
            Createdby = loggedinadmin?.Id ?? Guid.Empty
        };

        // save new user detail in userdetails table
        _context.Userdetails.Add(userdetail);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "User Added Successfully"
        };
    }


    public Userdetail GetUserDetailById(string id)
    {
        var user = _context.Userdetails.FirstOrDefault(u => u.Id.ToString() == id.ToString());
        return user;
    }

    public EditUserViewModel GetEditUserById(string id)
    {
        var user = _context.Userdetails.FirstOrDefault(u => u.Id.ToString() == id.ToString());
        var email = _context.Users.FirstOrDefault(u => u.Id == user.UserId)?.Email;

        var edituser = new EditUserViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Phone = user.Phone,
            Email = email,
            Status = user.Status,
            Country = user.Country,
            State = user.State,
            City = user.City,
            Address = user.Address,
            Zipcode = user.Zipcode,
            RoleName = _context.Roles.FirstOrDefault(r => r.Roleid == user.RoleId).Name,
            // Profile = user.Profile
        };

        return edituser;
    }


    public async Task<AuthResponse> EditUser(EditUserViewModel user)
    {

        // get jwt token from cookies
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwt"];

        // decode jwt token to get email from it
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        // get id of logged in admin
        var loggedinadmin = _context.Users.FirstOrDefault(u => u.Email == email);

        // get userdetail object from db which needs to be updated
        var userdetail = _context.Userdetails.FirstOrDefault(u => u.Id == user.Id);

        // logic for checking unique username
        if (_context.Userdetails.Any(u => u.UserName == user.UserName && u.Id != user.Id))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Username already exists"
            };
        }

        if (user.Profile != null)
        {
            userdetail.Profile = UploadFile(user.Profile);
        }

        // get country, state and city names
        var countryname = _context.Countries.FirstOrDefault(c => c.CountryId.ToString() == user.Country);
        var statename = _context.States.FirstOrDefault(c => c.StateId.ToString() == user.State);
        var cityname = _context.Cities.FirstOrDefault(c => c.CityId.ToString() == user.City);

        if (countryname != null)
        {
            user.Country = countryname?.CountryName;
        }
        if (statename != null)
        {
            user.State = statename?.StateName;
        }
        if (cityname != null)
        {
            user.City = cityname?.CityName;
        }

        var roleid = _context.Roles.FirstOrDefault(r => r.Name == user.RoleName)?.Roleid;

        userdetail.FirstName = user.FirstName;
        userdetail.LastName = user.LastName;
        userdetail.UserName = user.UserName;
        userdetail.Phone = user.Phone;
        userdetail.Status = user.Status;
        userdetail.Country = user.Country;
        userdetail.State = user.State;
        userdetail.City = user.City;
        userdetail.Address = user.Address;
        userdetail.Zipcode = user.Zipcode;
        userdetail.RoleId = roleid;
        // userdetail.Profile = user.Profile;
        userdetail.Modifiedby = loggedinadmin?.Id;

        _context.Userdetails.Update(userdetail);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "User Updated Successfully"
        };
    }


    public async Task<AuthResponse> ChangeProfilePassword(ChangePasswordViewModel model)
    {
        var token = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (PasswordService.VerifyPassword(model.OldPassword, user.Password))
        {
            user.Password = PasswordService.HashPassword(model.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Password Changed Successfully"
            };
        }
        else
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid Old Password"
            };
        }
    }

    public async Task<AuthResponse> UpdateUserProfile(UpdateUserViewModel model)
    {

        // logic for checking unique username
        if (_context.Userdetails.Any(u => u.UserName == model.UserName && u.Id != model.Id))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Username already exists"
            };
        }

        var token = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        var userdetail = _context.Userdetails.FirstOrDefault(u => u.UserId == _context.Users.FirstOrDefault(u => u.Email == email).Id);

        var statename = _context.States.FirstOrDefault(s => s.StateId.ToString() == model.State);
        var countryname = _context.Countries.FirstOrDefault(c => c.CountryId.ToString() == model.Country);
        var cityname = _context.Cities.FirstOrDefault(c => c.CityId.ToString() == model.City);

        // userdetail.State = statename?.StateName;
        // userdetail.Country = countryname?.CountryName;
        // userdetail.City = cityname?.CityName;
        if (countryname != null)
        {
            userdetail.Country = countryname?.CountryName;
        }
        if (statename != null)
        {
            userdetail.State = statename?.StateName;
        }
        if (cityname != null)
        {
            userdetail.City = cityname?.CityName;
        }

        userdetail.FirstName = model.FirstName;
        userdetail.LastName = model.LastName;
        userdetail.UserName = model.UserName;
        userdetail.Phone = model.Phone;
        userdetail.Address = model.Address;
        userdetail.Zipcode = model.Zipcode;
        // userdetail.Profile = model.Profile;

        if (model.Profile != null)
        {
            userdetail.Profile = UploadFile(model.Profile);
        }

        try
        {
            _context.Userdetails.Update(userdetail);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"DB Update Error: {ex.InnerException?.Message}");
            return new AuthResponse
            {
                Success = false,
                Message = "Database error occurred: " + ex.InnerException?.Message
            };
        }

        return new AuthResponse
        {
            Success = true,
            Message = "Profile Updated Successfully"
        };
    }


    public AuthResponse DeleteUserById(string id)
    {
        var user = _context.Userdetails.FirstOrDefault(u => u.Id.ToString() == id.ToString());
        user.Isdeleted = true;

        _context.Userdetails.Update(user);
        _context.SaveChanges();
        return new AuthResponse
        {
            Success = true,
            Message = "User Deleted Successfully"
        };
    }

    public string UploadFile(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "profile_images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            return $"/profile_images/{uniqueFileName}";
        }

        return null;
    }
}

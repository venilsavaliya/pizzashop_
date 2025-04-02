using System.Text;
using AspNetCoreHero.ToastNotification;
using BLL.Interfaces;
using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var jwtConfig = builder.Configuration.GetSection("jwt");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
}
);



// Add DbContext using Dependency Injection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? ""));


// Add JWT Authentication

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["issuer"],  // The issuer of the token (e.g., your app's URL)
            ValidAudience = jwtConfig["audience"], // The audience for the token (e.g., your API)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["key"] ?? "")), // The key to validate the JWT's signature
            // RoleClaimType = ClaimTypes.Role,
            // NameClaimType = ClaimTypes.Name 
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Check for the token in cookies
                var token = context.Request.Cookies["jwt"]; // Change "AuthToken" to your cookie name if it's different
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers["Authorization"] = "Bearer " + token;
                }
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                context.Response.Redirect("/Auth/Login"); // Redirect to login if forbidden
                return Task.CompletedTask;
            },
             OnChallenge = context =>
            {
                // Prevent the default 401 response
                context.HandleResponse();

                // Redirect the user to login page
                context.Response.Redirect("/Auth/Login"); // Change to your actual login route
                
                return Task.CompletedTask;
            }
            
        };
    }
);

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ISectionServices, SectionServices>();
builder.Services.AddScoped<IMenuServices,MenuServices>();
builder.Services.AddScoped<ITaxService,TaxService>();
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddScoped<ICustomerService,CustomerService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
builder.Services.AddSingleton<ExcelExportService>();

// Configure Rotativa
// var rootPath = Directory.GetCurrentDirectory(); // Get the current directory
// var wkhtmlPath = Path.Combine(rootPath, "wwwroot", "Rotativa"); // Combine paths
// RotativaConfiguration.Setup(rootPath, wkhtmlPath); // Setup with root and executable path


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseRotativa();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();

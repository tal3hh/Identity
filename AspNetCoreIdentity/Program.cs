using AspNetCoreIdentity.Context;
using AspNetCoreIdentity.Entities;
using AspNetCoreIdentity.FluentValidation.Account;
using AspNetCoreIdentity.Helper;
using AspNetCoreIdentity.Models.Account;
using AspNetCoreIdentity.Services;
using AspNetCoreIdentity.Services.Interface;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IMessageSend, MessageSend>();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<IValidator<UserCreateModel>, UserCreateValidation>();
builder.Services.AddScoped<IValidator<ResetPasswordModel>, ResetPasswordValidation>();
builder.Services.AddScoped<IValidator<UserLoginModel>, UserLoginValidation>();

builder.Services.AddScoped<RabbitMQHelper>();
builder.Services.AddScoped<RabbitMQHandler>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

#region Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;                  //Simvollardan biri olmalidir(@,/,$) 
    opt.Password.RequireLowercase = true;                         //Mutleq Kicik herf
    opt.Password.RequireUppercase = true;                         //Mutleq Boyuk herf 
    opt.Password.RequiredLength = 6;                              //Min. simvol sayi
    opt.Password.RequireDigit = false;

    opt.User.RequireUniqueEmail = true;                           //Email oxsarsiz olmalidir.

    opt.SignIn.RequireConfirmedEmail = true;                      //Email'e gelen register mesaji tesdiq edilmelidir.
    opt.SignIn.RequireConfirmedAccount = false;

    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3); //Sifreni 5 defe sehv girdikde hesab 3dk baglanir.
    opt.Lockout.MaxFailedAccessAttempts = 5;                      //Sifreni max. 5 defe sehv girmek olar.

}).AddErrorDescriber<CustomErrorDescriber>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
#endregion


#region Configure
builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.Cookie.HttpOnly = true;
    opt.Cookie.SameSite = SameSiteMode.Strict;
    opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    opt.Cookie.Name = "RestorantApp";
    opt.LoginPath = new PathString("/Account/Login");
    opt.AccessDeniedPath = new PathString("/Account/AccessDenied");
    opt.ExpireTimeSpan = TimeSpan.FromDays(1);
});
#endregion


#region Context
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration["ConnectionStrings:Postgresql"]);
});
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using DiemNgoaiKhoa.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var service = builder.Services;

// Add services to the container.
builder.Services.AddControllersWithViews();
service.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));
service.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MBI Core", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.Cookie.Name = "Login";
    options.LoginPath = "/Authen/Index";
    options.LogoutPath = "/Authen/Logout";
    options.AccessDeniedPath = "/Authen/Index";
});
builder.Services.ConfigureApplicationCookie(x =>
{
    x.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    x.SlidingExpiration = true;
});
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MBI Core v1"));
app.MapRazorPages();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Login
app.UseCookiePolicy();

app.UseAuthentication();

app.UseAuthorization();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Points}/{action=Index}/{id?}");

app.Run();

using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    options =>
    {
        options.LoginPath = "/Giris/GirisYap";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Add this line for HTTP Strict Transport Security (HSTS)
}

app.UseHttpsRedirection(); // Add this line to redirect HTTP requests to HTTPS

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Authentication

app.UseAuthorization(); // Authorization

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id=1}");

app.Run();

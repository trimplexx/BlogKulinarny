using BlogKulinarny.Data;
using BlogKulinarny.Data.Services;
using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Data.Services.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// admin services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AdminUsersService>();
builder.Services.AddScoped<AdminRecipesService>();

//user services
builder.Services.AddScoped<UserRecipesService>();

// Dodaj obsługę sesji
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Dodaj HttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(); // Dodaj schemat uwierzytelniania opartego na ciasteczkach (cookies)

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Dodaj obsługę uwierzytelniania
app.UseAuthorization();

// Dodaj obsługę sesji
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// initial seeding data
AppDbInitializer.Seed(app);

app.Run();
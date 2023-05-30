using BlogKulinarny.Data;
using BlogKulinarny.Data.Helpers;
using BlogKulinarny.Data.Services;
using BlogKulinarny.Data.Services.Admin;
using BlogKulinarny.Data.Services.Mail;
using BlogKulinarny.Data.Services.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

//mailer
var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.Configure<FormOptions>(o => {
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

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

// Dodaj obsługę uwierzytelniania
app.UseAuthentication(); 
app.UseAuthorization();

// Dodaj obsługę sesji
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// initial seeding data
AppDbInitializer.Seed(app);

app.Run();
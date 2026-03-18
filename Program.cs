using Microsoft.EntityFrameworkCore;
using WalletSICAI.Models;
using WalletSICAI.Services;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<WalletContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQL")));

builder.Services.AddAuthentication("MiCookieAuth").AddCookie("MiCookieAuth", options => { 
    //options.LoginPath = "/Cuenta/Login"; 
    options.LogoutPath = "/Home/Logout";
});
builder.Services.AddIdentity<Administrador, Rol>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedEmail = true; // recomendable
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthorization();

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<ReportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();

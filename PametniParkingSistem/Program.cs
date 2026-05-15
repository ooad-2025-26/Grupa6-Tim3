using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services;
using PametniParkingSistem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var baseConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var password = builder.Configuration["DbPassword"];

var connectionString = baseConnectionString + $"Password={password};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<Korisnik, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// repositories
builder.Services.AddScoped<IRezervacijaRepository, RezervacijaRepository>();
builder.Services.AddScoped<IParkingMjestoRepository, ParkingMjestoRepository>();
builder.Services.AddScoped<IPlacanjeRepository, PlacanjeRepository>();
builder.Services.AddScoped<IRecenzijaRepository, RecenzijaRepository>();
builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<ICjenovnikRepository, CjenovnikRepository>();
builder.Services.AddScoped<IEmailObavijestRepository, EmailObavijestRepository>();
builder.Services.AddScoped<IParkingZonaRepository, ParkingZonaRepository>();

// services
builder.Services.AddScoped<IRezervacijaService, RezervacijaService>();
builder.Services.AddScoped<IParkingMjestoService, ParkingMjestoService>();
builder.Services.AddScoped<IRecenzijaService, RecenzijaService>();
builder.Services.AddScoped<IPlacanjeService, PlacanjeService>();
builder.Services.AddScoped<IParkingZonaService, ParkingZonaService>();
builder.Services.AddScoped<IKorisnikService, KorisnikService>();
builder.Services.AddScoped<IEmailObavijestService, EmailObavijestService>();
builder.Services.AddScoped<ICjenovnikService, CjenovnikService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

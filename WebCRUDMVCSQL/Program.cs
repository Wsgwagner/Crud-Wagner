using Microsoft.EntityFrameworkCore;
using WebCRUDMVCSQL.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<Contexto>(options =>
    options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CRUD_MVC_SQL_CANAL_DEV;Integrated Security=False;User ID=tds;Password=tds123;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configuração de Localização para pt-BR
var supportedCultures = new[] { "pt-BR" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuarios}/{action=Login}/{id?}");

app.Run();
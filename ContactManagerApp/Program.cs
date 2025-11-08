using ServiceContracts;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using Repository;
using Serilog;
using ContactManagerApp.Filters.ActionFilters;


var builder = WebApplication.CreateBuilder(args);

//logging
builder.Host.UseSerilog(
    (HostBuilderContext context,
     IServiceProvider services,
     LoggerConfiguration loggerConfiguration) =>
    {
        loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
    });


builder.Services.AddControllersWithViews(options =>
{
    var logger = builder.Services.BuildServiceProvider().
    GetRequiredService<ILogger<PersonsListActionFilter>>();

    options.Filters.Add(new PersonsListActionFilter(logger,"global",2));
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService,PersonService>();


var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}




if (builder.Environment.IsEnvironment("Test") == false)
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}




app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();


public partial class Program
{

}
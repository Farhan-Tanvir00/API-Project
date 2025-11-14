using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.DTO;

var builder = WebApplication.CreateBuilder(args);

//Database connection string

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();

string licenseKey = builder.Configuration["AutoMapper:LicenseKey"] ?? string.Empty;

builder.Services.AddAutoMapper(cfg =>
{
    // set the license key
    cfg.LicenseKey = licenseKey;

    // optionally add other config, e.g. profile-specific config
},
// then pass types from assemblies where your profiles live
typeof(MappingProfile).Assembly /*, typeof(OtherProfile), etc. */
// /Assembly for all the profile classes
);


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

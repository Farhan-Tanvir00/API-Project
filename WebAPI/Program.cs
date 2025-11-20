using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Conventions;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using WebAPI.CustomMiddlewares;
using WebAPI.Data;
using WebAPI.DTO;
using WebAPI.Filters.OperationFilters;


var builder = WebApplication.CreateBuilder(args);

//Database connection string

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //Add the operation filter to include Authorization header
    options.OperationFilter<AuthorizationHeaderOperationFilter>();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

});


builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        //new UrlSegmentApiVersionReader(), Don’T want version in URL
        new HeaderApiVersionReader("api-version"),
        new MediaTypeApiVersionReader("api-version"),
        new QueryStringApiVersionReader("api-version")
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; 
    options.SubstituteApiVersionInUrl = false; // DON’T want version in URL else have to set true define routes with {version} parameter
});


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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["SecurityKey"] ?? string.Empty)),
        ClockSkew = TimeSpan.Zero
    };


    //Configuring JWT error messages

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            // Skip the default logic
            context.HandleResponse();

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Your token is missing, invalid, or expired."
            };

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(problemDetails);
        },

        OnAuthenticationFailed = context =>
        {
            context.NoResult(); // Prevent default response

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Token Validation Failed",
                Detail = context.Exception.Message
            };

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(problemDetails);
        }
    };

    //Configuring JWT error messages

});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Read", policy => policy.RequireClaim("read"));
    options.AddPolicy("Write", policy => policy.RequireClaim("write"));
    options.AddPolicy("Delete", policy => policy.RequireClaim("delete"));
});
//for returning custom authorization responses
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationResultHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiDemo V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApis.Database;
using MinimalApis.Domain;
using MinimalApis.Extensions;
using MinimalApis.Filters;
using MinimalApis.Model;
using MinimalApis.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddDbContext<EFContext>(options => 
{
    options.UseInMemoryDatabase("db_inmemory");
}).AddScoped<IDbContext, EFContext>();

builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
});
builder.Services.AddCustomValidations();

#region #api-versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

#region #authentications
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = "localhost",
            ValidAudience = "localhost",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("8x95L5fXbqSgJhwK2nobqF7lUa5MQjEmnswG"))
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(ResultModel.Error(401, "Your session has expired or is invalid."));
                context.HandleResponse();
            }
        };
    });
#endregion

//register minial endpoints
builder.Services.RegisterEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IdempotencyFilter>();
builder.Services.AddScoped<IEmailService, EmailProcessor>();
builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();

#region #swagger-ui
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Bearer token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "Bearer {token}",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme,
        }
    };
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() },
    });
});
#endregion

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<IdempotencyMiddleware>();

//map minimal endpoints
app.MapEndpoints();

app.Run();
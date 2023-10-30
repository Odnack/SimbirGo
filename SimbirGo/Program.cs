using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Simbir.Go.DataAccess.Context;
using Simbir.Go.DataAccess.Extensions;
using Simbir.Go.DataAccess.Helpers;
using Simbir.Go.DataAccess.Repositories;
using Simbir.GO.Entities.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration[$"{JwtOptions.SectionOption}:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration[$"{JwtOptions.SectionOption}:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[$"{JwtOptions.SectionOption}:Key"]!)),
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddAppSettings(builder.Configuration);

builder.Services.AddContext(builder.Configuration);

builder.Services.AddScoped<PostgresContext>();

builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<HashHelper>();
builder.Services.AddScoped<TokenHelper>();
builder.Services.AddScoped<IdentityHelper>();
builder.Services.AddScoped<TransportRepository>();

var app = builder.Build();

app.Services.MigrateDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DebtCollector.Infrastructure;
using DebtCollector.Infrastructure.Services;
using DebtCollector.Api.Hubs;
using DebtCollector.Application.Common.Interfaces;
using DebtCollector.Application;
using DebtCollector.Api.Helpers;

const string DevelopmentCorsPolicy = "_developmentCorsPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services
    .AddGraphQLServer()
    .AddQueryType<DebtCollector.Api.GraphQL.Query>();

builder.Services.AddHttpContextAccessor();
// AuthorizationService moved to Infrastructure DI
// builder.Services.AddScoped<IEmailService, AzureEmailService>(); // Moved to Infra
// builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<DebtCollectorContext>()); // Moved to Infra
// builder.Services.AddSingleton(new BlobStorageService(builder.Configuration.GetConnectionString("AzureBlobStorage"))); // Moved to Infra
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// SQL DB registration moved to AddInfrastructure

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: DevelopmentCorsPolicy,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();

                      });
});

// JWT
var jwtKey = builder.Configuration["JWT_KEY"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(DevelopmentCorsPolicy);
}

app.UseHttpsRedirection();

// Enable Swagger in all environments (including Production for Azure)
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GroupHub>("/hubs/group");

app.MapControllers();
app.MapGraphQL();
app.Run();

public partial class Program { }

using Accounting.Infrastructure;
using Accounting.Application;
using Accounting.GeneralLedger.Application;
using Accounting.GeneralLedger.Infrastructure;
using Accounting.AccountsPayable.Application;
using Accounting.AccountsPayable.Infrastructure;
using Accounting.API.Middleware;
using Accounting.Identity;
using Accounting.API.Authentication;
using Accounting.Identity.Seeding;
using Accounting.API.Seeding;
using Accounting.Identity.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<Accounting.API.Swagger.CompanyHeaderOperationFilter>();
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter your JWT access token."
    });

    options.AddSecurityRequirement(document =>
        new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [new Microsoft.OpenApi.OpenApiSecuritySchemeReference(
                "Bearer", document)] = []
        });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddGeneralLedgerApplication();
builder.Services.AddGeneralLedgerInfrastructure();

builder.Services.AddAccountsPayableApplication();
builder.Services.AddAccountsPayableInfrastructure();

builder.Services.AddIdentityModule(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddScoped<
    IDevelopmentSeedService,
    DevelopmentSeedService>();

builder.Services.AddScoped<
    ITenantStatusService,
    TenantStatusService>();
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();


    using var scope = app.Services.CreateScope();

    var seedService = scope.ServiceProvider
        .GetRequiredService<IDevelopmentSeedService>();

    // await seedService.SeedAsync();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
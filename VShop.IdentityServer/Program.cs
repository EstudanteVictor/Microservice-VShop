using Duende.IdentityServer.Licensing;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;
using System.Text;
using VShop.IdentityServer;
using VShop.IdentityServer.Configuration;
using VShop.IdentityServer.Data;
using VShop.IdentityServer.Extensions;
using VShop.IdentityServer.SeedDatabase;
using VShop.IdentityServer.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySQL(connectionString!));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

    var builderIdentityServer = builder.Services.AddIdentityServer(options =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
        options.EmitStaticAudienceClaim = true;
    }).AddInMemoryIdentityResources(
                           IdentityConfiguration.IdentityResources)
                            .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
                            .AddInMemoryApiResources(IdentityConfiguration.ApiResources)
                            .AddInMemoryClients(IdentityConfiguration.Clients)
                           .AddAspNetIdentity<ApplicationUser>();

    builder.Services.AddScoped<IDatabaseSeedInitializer, DatabaseIdentityServerInitializer>();

    builderIdentityServer.AddDeveloperSigningCredential();
    _ = builderIdentityServer.AddLicenseSummary();

    builder.Services.AddScoped<IProfileService, ProfileAppService>();

    var app = builder
        .ConfigureLogging()
        .ConfigureServices()
        .ConfigurePipeline();

    SeedDatabaseIdentityServer(app);

    app.MapControllerRoute(
        name: "default",
        pattern: "{comtroller=Home}/{action=Index}/{id?}");

    if (app.Environment.IsDevelopment())
    {
        _ = app.Lifetime.ApplicationStopping.Register(() =>
        {
            var usage = app.Services.GetRequiredService<LicenseUsageSummary>();
            Console.Write(Summary(usage));
        });
    }

    app.Run();

    void SeedDatabaseIdentityServer(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var initRolesUsers = serviceScope.ServiceProvider.GetService<IDatabaseSeedInitializer>();

            initRolesUsers.InitializeSeedRoles();
            initRolesUsers.InitializeSeedUsers();
        }
    }
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

static string Summary(LicenseUsageSummary usage)
{
    var sb = new StringBuilder();
    _ = sb.AppendLine("IdentityServer Usage Summary:");
    _ = sb.AppendLine(CultureInfo.InvariantCulture, $"  License: {string.Join(", ", usage.EntitledSkus)}");
    var features = usage.FeaturesUsed.Count > 0 ? string.Join(", ", usage.FeaturesUsed) : "None";
    _ = sb.AppendLine(CultureInfo.InvariantCulture, $"  Business and Enterprise Edition Features Used: {features}");
    _ = sb.AppendLine(CultureInfo.InvariantCulture, $"  {usage.ClientsUsed.Count} Client Id(s) Used");
    _ = sb.AppendLine(CultureInfo.InvariantCulture, $"  {usage.IssuersUsed.Count} Issuer(s) Used");

    return sb.ToString();
}

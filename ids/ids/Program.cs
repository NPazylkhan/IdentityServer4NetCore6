using IdentityServer4;
using ids;
using ids.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // uncomment to write to Azure diagnostics stream
                //.WriteTo.File(
                //    @"D:\home\LogFiles\Application\identityserver.txt",
                //    fileSizeLimitBytes: 1_000_000,
                //    rollOnFileSizeLimit: true,
                //    shared: true,
                //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");
var assembly = typeof(Program).Assembly.GetName().Name;

if (seed)
{
    Log.Information("Seeding database...");
    SeedData.EnsureSeedData(defaultConnString);
    Log.Information("Done seeding database.");
}
Log.Information("Starting host...");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(defaultConnString, b => b.MigrationsAssembly(assembly)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
        .AddAspNetIdentity<IdentityUser>()
        .AddConfigurationStore(options => { options.ConfigureDbContext = b => b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly)); })
        .AddOperationalStore(options => { options.ConfigureDbContext = b => b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly)); })
        .AddDeveloperSigningCredential();

builder.Services.AddAuthentication()
    .AddGoogle("Google", options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

        options.ClientId     = "client_id";
        options.ClientSecret = "client_secret";
    })
    .AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
     {
         options.SignInScheme  = IdentityServerConstants.ExternalCookieAuthenticationScheme;
         options.SignOutScheme = IdentityServerConstants.SignoutScheme;
         options.SaveTokens    = true;

         options.Authority     = "https://demo.identityserver.io/";
         options.ClientId      = "interactive.confidential";
         options.ClientSecret  = "secret";
         options.ResponseType  = "code";

         options.TokenValidationParameters = new TokenValidationParameters
         {
             NameClaimType = "name",
             RoleClaimType = "role"
         };
     });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());

app.MapRazorPages();

app.Run();

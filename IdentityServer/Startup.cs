using IdentityServer.Identity;
using IdentityServer.Interfaces;
using IdentityServer.Options;
using IdentityServer.Services;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }


    public void ConfigureServices(IServiceCollection services)
    {
        if (_configuration.GetValue<bool>("UseInMemoryDatabase"))
            services.AddDbContext<IdentityAppDbContext>(options =>
                options.UseInMemoryDatabase("CleanArchitectureDb"));
        else
            services.AddDbContext<IdentityAppDbContext>(options =>
                options.UseSqlServer(
                    _configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(IdentityAppDbContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(config =>
        {
            config.Cookie.Name = "IdentityServer.Cookie";
            config.LoginPath = "/Auth/Login";
            config.LogoutPath = "/Auth/Logout";
        });

        services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, IdentityAppDbContext>(opts =>
            {
                if (_environment.IsProduction())
                {
                    opts.ApiResources = new ApiResourceCollection(IdentityConfigurationProduction.GetApis());
                    opts.Clients = new ClientCollection(IdentityConfigurationProduction.GetClients());
                    opts.IdentityResources =
                        new IdentityResourceCollection(IdentityConfigurationProduction.GetIdentityResources());
                }
                else
                {
                    opts.ApiResources = new ApiResourceCollection(IdentityConfiguration.GetApis());
                    opts.Clients = new ClientCollection(IdentityConfiguration.GetClients());
                    opts.IdentityResources =
                        new IdentityResourceCollection(IdentityConfiguration.GetIdentityResources());
                }

            })
            .AddDeveloperSigningCredential();

        services.AddAuthentication()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = _configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = _configuration["Authentication:Google:ClientSecret"];
            });

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddHttpContextAccessor();

        services.AddControllersWithViews();

        services.AddCors(options =>
        {
            options.AddPolicy("AngularClient",
                builder =>
                {
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins(_configuration["ClientUrl"]);
                });
        });

        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IEmailService, EmailService>();

        services.Configure<EmailOptions>(_configuration.GetSection(EmailOptions.SectionName));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors("AngularClient");

        app.UseAuthentication();
        app.UseIdentityServer();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                "{controller}/{action=Index}/{id?}");
        });
    }
}
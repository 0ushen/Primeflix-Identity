using IdentityServer.Identity;
using IdentityServer.Services;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        if (Configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<IdentityAppDbContext>(options =>
                options.UseInMemoryDatabase("CleanArchitectureDb"));
        }
        else
        {
            services.AddDbContext<IdentityAppDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(IdentityAppDbContext).Assembly.FullName)));
        }

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
                opts.ApiResources = new ApiResourceCollection(IdentityServer.Configuration.GetApis());
                opts.Clients = new ClientCollection(IdentityServer.Configuration.GetClients());
                opts.IdentityResources =
                    new IdentityResourceCollection(IdentityServer.Configuration.GetIdentityResources());
            })
            .AddDeveloperSigningCredential();

        services.AddAuthentication()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
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
                           .WithOrigins("http://localhost:4200");
                });
        });

        services.AddTransient<EmailService>();
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
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
        });
    }
}

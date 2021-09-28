using EbookArchiver.Data.MySql;
using EbookArchiver.OneDrive;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace EbookArchiver.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                // Add ability to call web API (Graph) and get access tokens.
                // Unsure what the scope binding here is for, it doesn't seem to actually work.
                // The specific pages still need to specify their required scopes.
                // https://github.com/AzureAD/microsoft-identity-web/wiki/Managing-incremental-consent-and-conditional-access
                .EnableTokenAcquisitionToCallDownstreamApi(options =>
                    {
                        Configuration.Bind("AzureAd", options);
                    },
                    GraphConstants.DefaultScopes)
                .AddMicrosoftGraph(options =>
                    {
                        options.Scopes = string.Join(' ', GraphConstants.DefaultScopes);
                    }
                )
                // Use in-memory token cache
                // See https://github.com/AzureAD/microsoft-identity-web/wiki/token-cache-serialization
                .AddInMemoryTokenCaches();

            services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.DefaultPolicy;
            });
            services.AddRazorPages()
                .AddMvcOptions(options => { })
                .AddMicrosoftIdentityUI();

            string? connection = Configuration.GetConnectionString("localdb");
            string? port = Configuration.GetValue<string>("WEBSITE_MYSQL_PORT");
            if (port != null)
            {
                // Azure App Services do not create a valid connection string. We need to tweak it
                // and move the port # into a separate key-value pair.
                connection = connection.Replace(":" + port, string.Empty) + ";Port=" + port;
            }
            services.AddDbContext<EbookArchiverDbContext>(options =>
                options.UseMySql(connection,
                    ServerVersion.AutoDetect(connection),
                    b => b.MigrationsAssembly("EbookArchiver.Data.MySql")
                )
            );

            // Eventually we'll want to use Lamar, but it doesn't support .NET 6 yet.
            services.AddScoped<BookService>();
            services.AddScoped<OneDriveService>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}


using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

using MyWebApp.Services;

namespace MyWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //JwtSecurityTokenHandler used by JwtBearer & OIDC maps the standard OIDC claim types to long namespace names to match older protocols like WsFed. 
            //We can't disable this by default without affecting existing users.          
            //Turning this off manually requires either calling JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //configure client for service
            services.AddHttpClient();
            services.AddTransient<ApiService>();
            services.AddOptions();
             
            //Add Authentication form WebApp
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "AzureAd")
                    //Add support for token acquisition for downstream api -> delegated access
                    .EnableTokenAcquisitionToCallDownstreamApi(Configuration.GetSection("Api:ScopesForAccessToken").Get<string[]>())
                    .AddInMemoryTokenCaches();



            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();
            //Adds MicrosoftIdentityUI for SignOut en SignIn functionality
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
                app.UseExceptionHandler("/Home/Error");
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

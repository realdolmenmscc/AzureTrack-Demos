using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

using MyApi.Swagger;

namespace MyApi
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

            //Flag which indicates whether or not PII is shown in logs. False by default.
            // IdentityModelEventSource.ShowPII = true;

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                            .AllowCredentials()
                            .WithOrigins(
                                "https://localhost:4200")
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });


            // This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
            // By default, the claims mapping will map claim names in the old format to accommodate older SAML applications.
            // 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' instead of 'roles'
            // This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token
            // JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            // Adds Microsoft Identity platform (AAD v2.0) support to protect this Api
            services.AddMicrosoftIdentityWebApiAuthentication(Configuration, "AzureAd")
                    //only needed to call downstream API
                    .EnableTokenAcquisitionToCallDownstreamApi()
                    .AddInMemoryTokenCaches();

            //Add swagger
            SwaggerOAuthOptions swaggerAzureAdB2COptions = null;
            services.Configure<SwaggerOAuthOptions>(options =>
            {
                Configuration.GetSection("SwaggerOAuth").Bind(options);
                swaggerAzureAdB2COptions = options;
            });

            services.AddSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApi API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(x => x.FullName);
                options.AddSecurityDefinition("oauth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(swaggerAzureAdB2COptions.AuthorizationUrl),
                            TokenUrl = new Uri(swaggerAzureAdB2COptions.TokenUrl),
                            Scopes = swaggerAzureAdB2COptions.Scopes.ToDictionary(i => i.Value, i => i.Key)
                        }

                        //Implicit flow is also supported
                        //Implicit = new OpenApiOAuthFlow
                        //{
                        //    AuthorizationUrl = new Uri(swaggerAzureAdB2COptions.AuthorizationUrl),
                        //    TokenUrl = new Uri(swaggerAzureAdB2COptions.TokenUrl),
                        //    Scopes = swaggerAzureAdB2COptions.Scopes.ToDictionary(i => i.Value, i => i.Key)
                        //} 
                    }
                });
                options.OperationFilter<AuthorizeCheckOperationFilter>(); 
            });


            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    // .RequireClaim("email") // disabled this to test with users that have no email (no license added)
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAllOrigins");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var swaggerConfig = app.ApplicationServices.GetRequiredService<IOptions<SwaggerOAuthOptions>>();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApi");
                options.OAuthClientId(swaggerConfig.Value.ClientId);

                //not needed for implicit flow
                options.OAuthUsePkce();
            });
        }
    }
}

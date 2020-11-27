using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyApi.Swagger
{
    internal class AuthorizeCheckOperationFilter : IOperationFilter
    {
        private readonly SwaggerOAuthOptions _swaggerOAuthOptions;

        public AuthorizeCheckOperationFilter(IOptions<SwaggerOAuthOptions> swaggerOAuthOptions)
        {
            _swaggerOAuthOptions = swaggerOAuthOptions.Value;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check for authorize attribute
            var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                                                                 .Union(context.MethodInfo.GetCustomAttributes(true))
                                                                  .OfType<AuthorizeAttribute>();

            if (authAttributes.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                   new OpenApiSecurityRequirement()
                   {
                       {
                           new OpenApiSecurityScheme
                           {
                               Reference = new OpenApiReference
                               {
                                   Type = ReferenceType.SecurityScheme,
                                   Id = "oauth"
                               }
                           },
                           _swaggerOAuthOptions.Scopes.Select(x=>x.Value).ToList()
                       }
                   }
                };
            }
        }
    }
}

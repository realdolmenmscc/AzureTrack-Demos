using System.Collections.Generic;

namespace MyApi.Swagger
{
    public class SwaggerOAuthOptions
    {
        public string ClientId { get; set; }
        public string AuthorizationUrl { get; set; }
        public string TokenUrl { get; set; }
        public Dictionary<string, string> Scopes { get; set; }
    }
}

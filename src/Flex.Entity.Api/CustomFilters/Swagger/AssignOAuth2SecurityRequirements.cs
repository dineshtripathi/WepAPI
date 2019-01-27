using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Flex.Entity.Api.CustomFilters.Swagger
{
    /// <summary>
    /// 
    /// </summary>
    public class AssignOAuth2SecurityRequirements : IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            // Correspond each "Authorize" role to an oauth2 scope

            // Determine if the operation has the Authorize attribute
            var authorizeAttributes = apiDescription.ActionDescriptor.GetCustomAttributes<AuthorizeAttribute>();

            if (!authorizeAttributes.Any())
                return;

            // Initialize the operation.security property
            if (operation.security == null)
                operation.security = new List<IDictionary<string, IEnumerable<string>>>();

            // Add the appropriate security definition to the operation
            var oAuthRequirements = new Dictionary<string, IEnumerable<string>>
            {
                { "apiKey", Enumerable.Empty<string>() }
            };

            operation.security.Add(oAuthRequirements);
        }
    }
}
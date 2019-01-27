using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Flex.Entity.Api.CustomFilters.Swagger
{
    //Swagger Mutlitpe operations with same verb
    /// <summary>
    /// 
    /// </summary>
    public class SwaggerVerbFilter : IOperationFilter
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="operation"></param>
            /// <param name="schemaRegistry"></param>
            /// <param name="apiDescription"></param>
            public void Apply(
                Operation operation,
                SchemaRegistry schemaRegistry,
                ApiDescription apiDescription)
            {
                if (operation.parameters != null)
                {
                    operation.operationId += "By";
                    foreach (var parm in operation.parameters)
                    {
                        operation.operationId += $"{parm.name}";
                    
                    }
                }
            }
        }
   
}


using Flex.Entity.Api.CustomFilters.Swagger;

namespace Flex.Entity.Api.Controllers.SwaggerExamples
{
    /// <summary>
    /// 
    /// </summary>
    public class StringRequestExample : IProvideExample
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return "L";
        }
    }
}
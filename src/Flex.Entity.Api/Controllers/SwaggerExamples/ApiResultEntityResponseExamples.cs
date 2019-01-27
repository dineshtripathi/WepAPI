using Flex.Entity.Api.CustomFilters.Swagger;
using Flex.Entity.Api.Model;

namespace Flex.Entity.Api.Controllers.SwaggerExamples
{
    /// <summary>
    /// </summary>
    public class ApiResultEntityResponseExamples200 : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return new ApiResult {success = true, error = ""};
        }
    }


    /// <summary>
    /// </summary>
    public class ApiResultEntityResponseExamples400 : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return new ApiResult
            {
                success = false,
                error = "Entity could not be deleted/created"
            };
        }
    }



    /// <summary>
    /// </summary>
    public class ApiResultEntityResponseExamples404 : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return new ApiResult {success = false, error = "entity with code = L1 not found."};
        }
    }

}
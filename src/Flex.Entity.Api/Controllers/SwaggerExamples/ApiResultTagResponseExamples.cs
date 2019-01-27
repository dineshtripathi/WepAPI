using Flex.Entity.Api.CustomFilters.Swagger;
using Flex.Entity.Api.Model;

namespace Flex.Tag.Api.Controllers.SwaggerExamples
{
    /// <summary>
    /// </summary>
    public class ApiResultTagResponseExamples200 : IProvideExample
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
    public class ApiResultTagResponseExamples400 : IProvideExample
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
                error = "Tag could not be deleted/created"
            };
        }
    }



    /// <summary>
    /// </summary>
    public class ApiResultTagResponseExamples404 : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return new ApiResult {success = false, error = "tag with entityCode = {entityCode} and key = {key} not found." };
        }
    }

}
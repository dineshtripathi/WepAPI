using Flex.Entity.Api.CustomFilters.Swagger;
using Flex.Entity.Api.Model;

namespace Flex.TagType.Api.Controllers.SwaggerExamples
{
    /// <summary>
    /// </summary>
    public class ApiResultTagTypeResponseExamples200 : IProvideExample
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
    public class ApiResultTagTypeResponseExamples400 : IProvideExample
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
                error = "TagType could not be deleted/created"
            };
        }
    }



    /// <summary>
    /// </summary>
    public class ApiResultTagTypeResponseExamples404 : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return new ApiResult {success = false, error = "TagType key = {key} not found." };
        }
    }

}
using Flex.Entity.Api.CustomFilters.Swagger;
using Flex.Entity.Api.Model;

namespace Flex.Entity.Api.Controllers.SwaggerExamples
{
    /// <summary>
    /// </summary>
    public class ApiResultEntityTypeResponseExamples200 : IProvideExample
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
    public class ApiResultEntityTypeResponseExamples400 : IProvideExample
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
                error = "Entity type is associated with an entity and could not be deleted/created"
            };
        }
    }

    /// <summary>
    /// </summary>
    public class ApiResultEntityTypeResponseExamples404 : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return new ApiResult { success = false, error = "entity type with prefix = L not found." };
        }
    }

    /// <summary>
    /// </summary>
    public class ApiResultResponseExamples401 : IProvideExample
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
                error = "Authorization has been denied for this request."
            };

        }
    }

    /// <summary>
    /// </summary>
    public class ApiResultResponseExamples405 : IProvideExample
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
                error = "The requested resource does not support http method 'POST'."
            };

        }
    }



    /// <summary>
    /// </summary>
    public class ApiResultResponseExamples500 : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return new ApiResult {success = false, error = "{exception}"};
        }
    }


}
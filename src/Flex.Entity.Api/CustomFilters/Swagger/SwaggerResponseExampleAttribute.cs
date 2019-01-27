using System;
using System.Net;

namespace Flex.Entity.Api.CustomFilters.Swagger
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SwaggerResponseExampleAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="responseType"></param>
        /// <param name="exampleType"></param>
        public SwaggerResponseExampleAttribute(HttpStatusCode statusCode, Type responseType, Type exampleType)
        {
            StatusCode = statusCode;
            ExampleType = exampleType;
            ResponseType = responseType;
        }

        /// <summary>
        /// 
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Type ResponseType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Type ExampleType { get; set; }
    }
}

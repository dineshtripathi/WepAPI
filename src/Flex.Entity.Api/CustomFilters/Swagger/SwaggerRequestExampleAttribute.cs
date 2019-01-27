using System;

namespace Flex.Entity.Api.CustomFilters.Swagger
{
    /// <summary>
    /// 
    /// </summary>
    public class SwaggerRequestExampleAttribute:Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseType"></param>
        /// <param name="exampleProvider"></param>
        public SwaggerRequestExampleAttribute(Type responseType, Type exampleProvider)
        {
            RequestType = responseType;
            ExampleType = exampleProvider;
        }
        /// <summary>
        /// 
        /// </summary>
        public Type ExampleType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type RequestType { get; set; }
    }
}

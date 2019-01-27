using System.Collections.Generic;
using Flex.Entity.Api.CustomFilters.Swagger;

namespace Flex.Entity.Api.Controllers.SwaggerExamples
{
    /// <summary>
    /// TagType for swagger
    /// </summary>
    public class TagTypeResponseExamples : IProvideExample
    {
        public object GetExample(string mediaType = "application/json")
        {
            return new List<Model.TagType>
            {
                new Model.TagType
                {
                    key = "temp"
                    
                }
                ,
                new Model.TagType
                {
                    key = "setpointh"
                }
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TagTypeResponseExamplesSingle : IProvideExample
    {
        public object GetExample(string mediaType = "application/json")
        {
            return new Model.TagType
            {
                key = "temp"
            };
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class TagTypeRequestExample : IProvideExample
    {
        public object GetExample(string mediaType = "application/json")
        {
            return new Model.TagType()
            {
                key = "temp"
            };
        }
    }
}
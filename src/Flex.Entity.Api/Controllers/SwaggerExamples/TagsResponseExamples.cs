using System;
using System.Collections.Generic;
using Flex.Entity.Api.CustomFilters.Swagger;
using Flex.Entity.Api.Model;

namespace Flex.Entity.Api.Controllers.SwaggerExamples
{
    /// <summary>
    /// Tags for swagger
    /// </summary>
    public class TagsResponseExamples : IProvideExample
    {
        public object GetExample(string mediaType = "application/json")
        {
            return new List<Model.TagAt>
            {
                new Model.TagAt
                {
                    key = "temp",
                    value = "32",
                    updated_at = DateTime.UtcNow
                }
                ,
                new Model.TagAt
                {
                    key = "setpointh",
                    value = "20",
                    updated_at = DateTime.UtcNow
                }
            };
        }
    }

    public class TagResponseExamplesSingle : IProvideExample
    {
        public object GetExample(string mediaType = "application/json")
        {
            return new Model.TagAt
            {
                key = "temp",
                value = "32",
                updated_at = DateTime.UtcNow
            };
        }
    }


    public class TagRequestExample : IProvideExample
    {
        public object GetExample(string mediaType = "application/json")
        {
            return new Model.TagRequest
            {
                value = "32",                
            };
        }
    }
}
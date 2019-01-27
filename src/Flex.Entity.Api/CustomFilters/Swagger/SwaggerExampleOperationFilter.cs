using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Swagger;

namespace Flex.Entity.Api.CustomFilters.Swagger
{
    /// <summary>
    /// 
    /// </summary>
    public class SwaggerExampleOperationFilter:IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {

            SetRequestModelExamples(operation, schemaRegistry, apiDescription);
            SetResponseModelExamples(operation, schemaRegistry, apiDescription);

            //var responseAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerResponseExampleAttribute>();
            //foreach (var respattr in responseAttributes)
            //{
            //    var schema = schemaRegistry.GetOrRegister(respattr.ResponseType);
            //    var response =
            //        operation.responses.First(
            //            x => x.Value.schema.type == schema.type && x.Value.schema.@ref == schema.@ref).Value;
            //    if (response != null)
            //    {
            //        var provider = (IProvideExample) Activator.CreateInstance(respattr.ExamplesType);
            //        response.examples = FormatAsJson(provider);
            //    }
            //}
        }

        private void SetResponseModelExamples(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var responseAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerResponseExampleAttribute>();

            foreach (var attr in responseAttributes)
            {

                var response =
                    operation.responses.FirstOrDefault(
                        x => x.Value != null && x.Key == ((int)attr.StatusCode).ToString());

                if (response.Equals(default(KeyValuePair<string, Response>)) == false)
                {
                    var provider = (IProvideExample)Activator.CreateInstance(attr.ExampleType);

                    response.Value.examples = FormatAsJson(provider);
                    response.Value.schema = schemaRegistry.GetOrRegister(attr.ResponseType);

                }
            }
        }

        private void SetRequestModelExamples(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var requestAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerRequestExampleAttribute>();

            foreach (var attr in requestAttributes)
            {
                var schema = schemaRegistry.GetOrRegister(attr.RequestType);

                var request = operation.parameters.FirstOrDefault(p => p.@in == "body");

                if (request == null && schema.@ref == null) continue;
                var provider = (IProvideExample)Activator.CreateInstance(attr.ExampleType);

                var parts = schema.@ref?.Split('/');
                var name = parts?.Last();

                if (name != null)
                {
                    var definitionToUpdate = schemaRegistry.Definitions[name];

                    if (definitionToUpdate != null)
                    {
                        definitionToUpdate.example = ((dynamic) FormatAsJson(provider));//["application/json"];
                    }
                }
            }
        }

        private static object FormatAsJson(IProvideExample provider)
        {
            var example = new Dictionary<string, object>()
            {
                {
                    "application/json", provider.GetExample()
                },
                {
                    "application/hal+json", provider.GetExample("application/hal+json")
                }
            };
            return ConvertToCamelCase(example);
        }

        private static object ConvertToCamelCase(Dictionary<string, object> example)
        {
            var jsonString = JsonConvert.SerializeObject(example,
                new JsonSerializerSettings() {ContractResolver = new CamelCasePropertyNamesContractResolver()});
            return JsonConvert.DeserializeObject(jsonString);
        }
    }
}

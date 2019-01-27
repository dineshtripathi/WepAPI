using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Flex.Entity.Api.CustomFilters.Swagger;
using Flex.Entity.Api.Helpers;
using Flex.Entity.Api.Model;
using Newtonsoft.Json;

namespace Flex.Entity.Api.Controllers.SwaggerExamples
{

    /// <summary>
    /// </summary>
    public class EntityAtResponseExamples : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return new List<Model.EntityAt>
            {
                new Model.EntityAt
                {
                    type = "Client",
                    code = "C1",
                    name = "Customer 1",
                    asset_parent = "",
                    service_parent = "",
                    updated_at = DateTime.UtcNow

                }
                ,
                new Model.EntityAt
                {
                    type = "Site",
                    code = "S1",
                    name = "Site 1",
                    asset_parent = "C1",
                    service_parent = "",
                    updated_at = DateTime.UtcNow

                }
            };
        }
    }

    /// <summary>
    /// </summary>
    public class EntityAtResponseExamplesSingle : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return
                new Model.EntityAt
                {
                    type = "Client",
                    code = "C1",
                    name = "Customer 1",
                    asset_parent = "",
                    service_parent = "",
                    updated_at = DateTime.UtcNow
                };
        }
    }
    /// <summary>
    /// </summary>
    public class EntitiesResponseExamples : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            if (mediaType == "application/hal+json")
            {
                string jsonExample = @"{
                            ""page_count"": 4,
                            ""page_number"": 0,
	                       ""_links"": {

                                  ""self"": {
                                        ""href"": ""/entities""

                                  },
		                        ""next"": {
                                        ""href"": ""/entities?top=1000&skip=1000""

                                  },
		                        ""previous"": {
                                        ""href"": ""/entities?top=1000&skip=0""

                                  },
		                        ""entities"": [{
			                        ""href"": ""/entities/C1""

                                  }, {
			                        ""href"": ""/entities/C2""
		                        }, {
			                        ""href"": ""/entities/S1""
		                        }, {
			                        ""href"": ""/entities/D1""
		                        }, {
			                        ""href"": ""/entities/D2""
		                        }, {
			                        ""href"": ""/entities/G1""
		                        }, {
			                        ""href"": ""/entities/F1""
		                        }, {
			                        ""href"": ""/entities/B1""
		                        }, {
			                        ""href"": ""/entities/L1""
		                        }]
	                        },
	                        ""_embedded"": {
		                        ""entities"": [{
			                        ""code"": ""C1"",
			                        ""type"": ""Client"",
			                        ""name"": ""J Sainsbury"",
			                        ""asset_parent"": ""OE1"",
			                        ""updated_at"": ""2016-10-17T16:29:23.2445552"",
			                        ""_links"": {
				                        ""self"": {
					                        ""href"": ""/entities/C1""
				                        },
				                        ""tags"": {
					                        ""href"": ""/entities/C1/tags""
				                        },
				                        ""asset_parent"": {
					                        ""href"": ""/entities/OE1""
				                        },
				                        ""asset_children"": {
					                        ""href"": ""/entities/C1/children/asset""
				                        },
				                        ""service_children"": {
					                        ""href"": ""/entities/C1/children/service""
				                        }
			                        }
		                        }, {
			                        ""code"": ""C2"",
			                        ""type"": ""Client"",
			                        ""name"": ""Tesco"",
			                        ""asset_parent"": ""OE1"",
			                        ""updated_at"": ""2016-10-17T16:29:23.436638"",
			                        ""_links"": {
				                        ""self"": {
					                        ""href"": ""/entities/C2""
				                        },
				                        ""tags"": {
					                        ""href"": ""/entities/C2/tags""
				                        },
				                        ""asset_parent"": {
					                        ""href"": ""/entities/OE1""
				                        },
				                        ""asset_children"": {
					                        ""href"": ""/entities/C2/children/asset""
				                        },
				                        ""service_children"": {
					                        ""href"": ""/entities/C2/children/service""
				                        }
			                        }
		                        }, {
			                        ""code"": ""S1"",
			                        ""type"": ""Site"",
			                        ""name"": ""Northcheam"",
			                        ""asset_parent"": ""C1"",
			                        ""updated_at"": ""2016-10-17T16:29:23.4487185"",
			                        ""_links"": {
				                        ""self"": {
					                        ""href"": ""/entities/S1""
				                        },
				                        ""tags"": {
					                        ""href"": ""/entities/S1/tags""
				                        },
				                        ""asset_parent"": {
					                        ""href"": ""/entities/C1""
				                        },
				                        ""asset_children"": {
					                        ""href"": ""/entities/S1/children/asset""
				                        },
				                        ""service_children"": {
					                        ""href"": ""/entities/S1/children/service""
				                        }
			                        }
		                        }, {
			                        ""code"": ""D1"",
			                        ""type"": ""Device"",
			                        ""name"": ""JACE"",
			                        ""asset_parent"": ""S1"",
			                        ""updated_at"": ""2016-10-17T16:29:23.4605689"",
			                        ""_links"": {
				                        ""self"": {
					                        ""href"": ""/entities/D1""
				                        },
				                        ""tags"": {
					                        ""href"": ""/entities/D1/tags""
				                        },
				                        ""asset_parent"": {
					                        ""href"": ""/entities/S1""
				                        },
				                        ""asset_children"": {
					                        ""href"": ""/entities/D1/children/asset""
				                        },
				                        ""service_children"": {
					                        ""href"": ""/entities/D1/children/service""
				                        }
			                        }
		                        }, {
			                        ""code"": ""D2"",
			                        ""type"": ""Device"",
			                        ""name"": ""JACE2"",
			                        ""asset_parent"": ""S1"",
			                        ""updated_at"": ""2016-10-17T16:29:23.4716678"",
			                        ""_links"": {
				                        ""self"": {
					                        ""href"": ""/entities/D2""
				                        },
				                        ""tags"": {
					                        ""href"": ""/entities/D2/tags""
				                        },
				                        ""asset_parent"": {
					                        ""href"": ""/entities/S1""
				                        },
				                        ""asset_children"": {
					                        ""href"": ""/entities/D2/children/asset""
				                        },
				                        ""service_children"": {
					                        ""href"": ""/entities/D2/children/service""
				                        }
			                        }
		                        }, {
			                        ""code"": ""G1"",
			                        ""type"": ""Grid"",
			                        ""name"": ""National Grid"",
			                        ""service_parent"": ""OE1"",
			                        ""updated_at"": ""2016-10-17T16:29:23.5185551"",
			                        ""_links"": {
				                        ""self"": {
					                        ""href"": ""/entities/G1""
				                        },
				                        ""tags"": {
					                        ""href"": ""/entities/G1/tags""
				                        },
				                        ""service_parent"": {
					                        ""href"": ""/entities/OE1""
				                        },
				                        ""asset_children"": {
					                        ""href"": ""/entities/G1/children/asset""
				                        },
				                        ""service_children"": {
					                        ""href"": ""/entities/G1/children/service""
				                        }
			                        }
		                        }, {
			                        ""code"": ""F1"",
			                        ""type"": ""Fleet"",
			                        ""name"": ""FFR"",
			                        ""service_parent"": ""G1"",
			                        ""updated_at"": ""2016-10-17T16:29:23.5866001"",
			                        ""_links"": {
				                        ""self"": {
					                        ""href"": ""/entities/F1""
				                        },
				                        ""tags"": {
					                        ""href"": ""/entities/F1/tags""
				                        },
				                        ""service_parent"": {
					                        ""href"": ""/entities/G1""
				                        },
				                        ""asset_children"": {
					                        ""href"": ""/entities/F1/children/asset""
				                        },
				                        ""service_children"": {
					                        ""href"": ""/entities/F1/children/service""
				                        }
			                        }
		                        }, {
			                        ""code"": ""B1"",
			                        ""type"": ""Bucket"",
			                        ""name"": ""Live"",
			                        ""service_parent"": ""F1"",
			                        ""updated_at"": ""2016-10-17T16:29:23.5926662"",
			                        ""_links"": {
				                        ""self"": {
					                        ""href"": ""/entities/B1""
				                        },
				                        ""tags"": {
					                        ""href"": ""/entities/B1/tags""
				                        },
				                        ""service_parent"": {
					                        ""href"": ""/entities/F1""
				                        },
				                        ""asset_children"": {
					                        ""href"": ""/entities/B1/children/asset""
				                        },
				                        ""service_children"": {
					                        ""href"": ""/entities/B1/children/service""
				                        }
			                        }
		                        }, {
			                        ""code"": ""L1"",
			                        ""type"": ""Load"",
			                        ""name"": ""TANK 1"",
			                        ""service_parent"": ""B1"",
			                        ""asset_parent"": ""D1"",
			                        ""updated_at"": ""2016-10-17T16:29:23.626566"",
			                        ""_links"": {
				                        ""self"": {
					                        ""href"": ""/entities/L1""
				                        },
				                        ""tags"": {
					                        ""href"": ""/entities/L1/tags""
				                        },
				                        ""asset_parent"": {
					                        ""href"": ""/entities/D1""
				                        },
				                        ""service_parent"": {
					                        ""href"": ""/entities/B1""
				                        },
				                        ""asset_children"": {
					                        ""href"": ""/entities/L1/children/asset""
				                        },
				                        ""service_children"": {
					                        ""href"": ""/entities/L1/children/service""
				                        }
			                        }
		                        }]
	                        }
                        }";
                return JsonConvert.DeserializeObject(jsonExample);
            }
            var details = (IEnumerable<EntityDetail>) (new EntityDetailResponseExamples()).GetExample();
            var entityDetails = details as IList<EntityDetail> ?? details.ToList();
            return new Model.Entities
            {
                entities = entityDetails
                ,
                count = entityDetails.Count()
            };
        }
    }

    /// <summary>
    /// </summary>
    public class EntityDetailResponseExamples : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return new List<Model.EntityDetail>
            {

                new Model.EntityDetail
                {
                    type = "Client",
                    code = "C1",
                    name = "Customer 1",
                    asset_parent = "",
                    service_parent = "",
                    updated_at = DateTime.UtcNow
                },
                new Model.EntityDetail
                {
                    type = "Site",
                    code = "S1",
                    name = "Site 1",
                    asset_parent = "C1",
                    service_parent = "",
                    updated_at = DateTime.UtcNow
                },
                new Model.EntityDetail
                {
                    type = "Device",
                    code = "D1",
                    name = "Device 1",
                    asset_parent = "S1",
                    service_parent = "",
                    updated_at = DateTime.UtcNow
                },
                new Model.EntityDetail
                {
                    type = "Load",
                    code = "L1",
                    name = "Load 1",
                    asset_parent = "D1",
                    service_parent = "",
                    updated_at = DateTime.UtcNow
                }
            };
        }
    }

    

    /// <summary>
    /// </summary>
    public class EntityResponseExamplesSingle : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return
                new Model.Entity
                {
                    type = "Client",
                    code = "C1",
                    name = "Customer 1",
                    asset_parent = "",
                    service_parent = ""
                };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EntityPatchRequestExampleSingle : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return   new Model.EntityPatchRequest
                {
                    name = "Load name changed",
                    asset_parent = "",
                    service_parent = "F1"
                };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EntityRequestExampleSingle : IProvideExample
    {
        /// <summary>
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public object GetExample(string mediaType = "application/json")
        {
            return
                new Model.EntityRequest
                {
                    typePrefix = "C",
                    name = "Customer 1",
                    asset_parent = "",
                    service_parent = ""
                };
        }
    }

}
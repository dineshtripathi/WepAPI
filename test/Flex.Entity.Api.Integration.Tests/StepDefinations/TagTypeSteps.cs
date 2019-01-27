using System;
using System.Collections.Generic;
using System.Linq;
using Flex.Entity.Api.Integration.Tests.Framework;
using Flex.Entity.Api.Model;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Flex.Entity.Api.Integration.Tests.StepDefinations
{
    [Binding]
    public class TagTypeSteps
    {
        private readonly RequestContext _requestContext;

        public TagTypeSteps(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }


        [Given(@"a request to create a new tag type with tag type name '(.*)'")]
        public void GivenARequestToCreateANewTagTypeWithTagTypeName(string p0, Table table)
        {
            _requestContext.RequestTable = table.CreateInstance<Request>();
            _requestContext.Code = p0;
        }
        
        [Given(@"a a request to delete a tag type with name '(.*)'")]
        public void GivenAARequestToDeleteATagTypeWithName(string p0, Table table)
        {
            _requestContext.RequestTable = table.CreateInstance<Request>();
            _requestContext.Code = p0;
        }

        [Given(@"a request for all tag types")]
        public void GivenARequestForAllTagTypes(Table table)
        {
            _requestContext.RequestTable = table.CreateInstance<Request>();
        }

        [Given(@"a request for an tag types with prefix '(.*)'")]
        public void GivenARequestForAnTagTypesWithPrefix(string p0, Table table)
        {
            _requestContext.RequestTable = table.CreateInstance<Request>();
            _requestContext.Code = p0;
        }


        [When(@"the client makes a POST request with the body '(.*)'")]
        [Scope(Feature = "TagTypes")]
        public void WhenTheClientMakesAPOSTRequestWithTheBody(string p0)
        {
            _requestContext.RequestTable.HttpMethod = RequestMethod.Post;
            _requestContext.Body = Utilities.DeserializeJson<TagType>(p0);
        }

        [Then(@"the response contains a collection of tag type details")]
        public void ThenTheResponseContainsACollectionOfTagTypeDetails()
        {
            FlexEntityApi<IEnumerable<TagType>>.SetBaseAddress(_requestContext.RequestTable.BaseUrl)
                            .WithUrl(_requestContext.RequestTable.RelativeUri)
                            .WithGet()
                            .WithAccept(_requestContext.RequestTable.Accept)
                            .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                            .WithTimeout(TimeSpan.FromSeconds(60))
                            .Result((actualEntityTypes, statusCode) =>
                            {
                                Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                Assert.IsTrue(actualEntityTypes.Any());
                             });
        }

        [Then(@"the response contains tag type details")]
        public void ThenTheResponseContainsTagTypeDetails()
        {
            FlexEntityApi<TagType>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                            WithUrl(_requestContext.RequestTable.RelativeUri)
                           .WithCode($"{_requestContext.Code}")
                           .WithGet()
                           .WithAccept(_requestContext.RequestTable.Accept)
                           .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                           .WithTimeout(TimeSpan.FromSeconds(60))
                           .Result((actualEntityType, statusCode) =>
                           {
                               Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                               Assert.AreEqual(actualEntityType.key, _requestContext.Code);
                             });
        }

        [Then(@"the response contains the newly created tags type details")]
        public void ThenTheResponseContainsTheNewlyCreatedTagsTypeDetails()
        {
            FlexEntityApi<TagType>.SetBaseAddress(_requestContext.RequestTable.BaseUrl)
                            .WithUrl(_requestContext.RequestTable.RelativeUri)
                            .WithPost(_requestContext.Body)
                            .WithAccept(_requestContext.RequestTable.Accept)
                            .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                            .WithTimeout(TimeSpan.FromSeconds(60))
                            .Result((actualEntityType, statusCode) =>
                            {
                                Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                Assert.IsTrue(actualEntityType.key == _requestContext.Code);
                            });
        }
        
       
    }
}

using System;
using System.Collections;
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
    public class EntityTypesSteps
    {
        private readonly RequestContext _requestContext;

        public EntityTypesSteps(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        [Given(@"a request for all entity types")]
        public void GivenARequestForAllEntityTypes(Table table)
        {
            _requestContext.RequestTable =  table.CreateInstance<Request>();
        }

        [Given(@"a request for an entity types with prefix '(.*)'")]
        public void GivenARequestForAnEntityTypesWithPrefix(string prefix, Table table)
        {
            _requestContext.RequestTable = table.CreateInstance<Request>();
            _requestContext.Code = prefix;
        }

        [Then(@"the response contains a collection of entity type details")]
        public void ThenTheResponseContainsACollectionOfEntityTypeDetails()
        {
            FlexEntityApi<IEnumerable<EntityType>>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithGet()
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((actualEntityTypes, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.IsTrue(actualEntityTypes.Any());
                                 //CollectionAssert.AreEqual((ICollection)actualBucket.OrderBy(x => x.BucketCode).ToList(), (ICollection)expectedBucket.OrderBy(x => x.BucketCode).ToList());
                             });
        }

        [Then(@"the response contains entity type details")]
        public void ThenTheResponseContainsEntityTypeDetails()
        {
            FlexEntityApi<EntityType>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithCode($"{_requestContext.Code}")
                             .WithGet()
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((actualEntityType, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.AreEqual(actualEntityType.prefix,_requestContext.Code);
                                 //CollectionAssert.AreEqual((ICollection)actualBucket.OrderBy(x => x.BucketCode).ToList(), (ICollection)expectedBucket.OrderBy(x => x.BucketCode).ToList());
                             });
        }
        [Then(@"the response contains error details")]
        public void ThenTheResponseContainsErrorDetails()
        {
            FlexEntityApi<ApiResult>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithCode($"{_requestContext.Code}")
                             .WithGet()
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((actualEntityType, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.IsFalse(actualEntityType.success);
                                 Assert.IsTrue(actualEntityType.error.Contains(_requestContext.Code));
                             });
        }

        [Given(@"a request to create a new entity type with prefix '(.*)'")]
        public void GivenARequestToCreateANewEntityType(string prefix, Table table)
        {
            _requestContext.RequestTable = table.CreateInstance<Request>();
            _requestContext.Code = prefix;

        }

        [When(@"the client makes a POST request with the body '(.*)'")]
        [Scope(Feature = "EntityTypes")]
        public void WhenTheClientMakesAPOSTRequestWithTheBody(string p0)
        {
            _requestContext.RequestTable.HttpMethod = RequestMethod.Post;
            _requestContext.Body = Utilities.DeserializeJson<EntityType>(p0);
        }


        [Then(@"the response contains the newly created entity type details\.")]
        public void ThenTheResponseContainsTheNewlyCreatedEntityTypeDetails_()
        {
            FlexEntityApi<EntityType>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithPost(_requestContext.Body)
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((actualEntityType, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.IsTrue(actualEntityType.prefix == _requestContext.Code);
                             });

        }

        [Then(@"the response contains the error details for the BadRequest")]
        public void ThenTheResponseContainsTheErrorDetailsForTheBadRequest()
        {
            FlexEntityApi<ApiResult>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithPost((EntityType)_requestContext.Body)
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((apiResult, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.IsFalse(apiResult.success);
                                 Assert.IsFalse(string.IsNullOrWhiteSpace(apiResult.error));
                             });
        }

        [Given(@"a request to delete an entity types with prefix '(.*)'")]
        public void GivenARequestToDeleteAnEntityTypesWithPrefix(string prefix, Table table)
        {
            _requestContext.RequestTable = table.CreateInstance<Request>();
            _requestContext.Code = prefix;

        }


        [When(@"the client makes a GET request without a bearer token")]
        public void WhenTheClientMakesAGETRequestWithoutABearerToken()
        {
            _requestContext.RequestTable.Authorization = string.Empty;
        }

        [Then(@"the response contains an error message")]
        public void ThenTheResponseContainsAnErrorMessage()
        {
            FlexEntityApi<ApiResult>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithCode($"{_requestContext.Code}")
                             .WithGet()
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((apiResult, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.IsFalse(apiResult.success);
                                 Assert.IsFalse(string.IsNullOrWhiteSpace(apiResult.error));
                             });
        }

        [When(@"the Client makes a Get request with a bearer token having no emailid")]
        public void WhenTheClientMakesAGetRequestWithABearerTokenHavingNoEmailid()
        {
            _requestContext.RequestTable.Authorization = (string)FeatureContext.Current["JWTTokenWithoutEmailId"];
        }

        [When(@"the Client makes a Get request with a bearer token having no permission")]
        public void WhenTheClientMakesAGetRequestWithABearerTokenHavingNoPermission()
        {
            _requestContext.RequestTable.Authorization = (string)FeatureContext.Current["JWTTokenWithoutPermission"];
        }



    }
}

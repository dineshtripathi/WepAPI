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
    public class EntitySteps
    {
        private readonly RequestContext _requestContext;

        public EntitySteps(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        [Given(@"a request to create a new entity with entity type '(.*)'")]
        public void GivenARequestToCreateANewEntityWithEntityType(string p0, Table table)
        {
            _requestContext.CacheDataForAsserts = p0;
            _requestContext.RequestTable = table.CreateInstance<Request>();

        }

        [When(@"the client makes a POST request with the body '(.*)'")]
        [Scope(Feature = "Entity")]
        public void WhenTheClientMakesAPOSTRequestWithTheBody(string p0)
        {
            _requestContext.RequestTable.HttpMethod = RequestMethod.Post;
            _requestContext.Body = Utilities.DeserializeJson<EntityRequest>(p0);
        }

        [Then(@"the response contains the newly created entity")]
        public void ThenTheResponseContainsTheNewlyCreatedEntity()
        {
            FlexEntityApi<Model.Entity>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithRequestMethod(_requestContext)
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((actualEntityType, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.IsTrue(actualEntityType.type == _requestContext.CacheDataForAsserts);
                             });
        }

        [Then(@"the response contains the creation failure error message")]
        public void ThenTheResponseContainsTheCreationFailureErrorMessage()
        {
            FlexEntityApi<Model.ApiResult>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithRequestMethod(_requestContext)
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((apiresult, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.IsFalse(apiresult.success);
                                 Assert.IsTrue(apiresult.error?.Length > 0);
                             });
        }

        [Given(@"a request to update an entity with an entity code '(.*)'")]
        public void GivenARequestToUpdateAnEntityWithAnEntityCode(string p0, Table table)
        {
            _requestContext.Code = p0;
            _requestContext.RequestTable = table.CreateInstance<Request>();
        }

        [When(@"the client makes a PATCH request with the body '(.*)'")]
        [Scope(Feature = "Entity")]
        public void WhenTheClientMakesAPATCHRequestWithTheBody(string p0)
        {
            _requestContext.RequestTable.HttpMethod = RequestMethod.Patch;
            _requestContext.Body = Utilities.DeserializeJson(p0);
        }

        [Then(@"the response contains the update success message")]
        public void ThenTheResponseContainsTheUpdateSuccessMessage()
        {
            FlexEntityApi<Model.ApiResult>.SetBaseAddress(_requestContext.RequestTable.BaseUrl)
                              .WithUrl(_requestContext.RequestTable.RelativeUri)
                              .WithCode(_requestContext.Code)
                             .WithRequestMethod(_requestContext)
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((apiresult, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.IsTrue(apiresult.success);
                                 Assert.IsFalse(apiresult.error?.Length > 0);
                             });
        }

        [Then(@"the response contains the update failure error message")]
        public void ThenTheResponseContainsTheUpdateFailureErrorMessage()
        {
            FlexEntityApi<Model.ApiResult>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                              .WithCode(_requestContext.Code)
                             .WithPatch(_requestContext.Body)
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((apiresult, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.IsFalse(apiresult.success);
                                 Assert.IsTrue(apiresult.error?.Length > 0);
                             });
        }

        [Given(@"a request to get an entity with an entity code '(.*)'")]
        public void GivenARequestToGetAnEntityWithAnEntityCode(string p0, Table table)
        {
            _requestContext.Code = p0;
            _requestContext.RequestTable = table.CreateInstance<Request>();
        }

        [Given(@"at a point in time '(.*)'")]
        public void GivenAtAPointInTime(string p0)
        {
            _requestContext.QueryStringParams = p0.Replace("{now}", DateTimeOffset.UtcNow.ToString("o"));

        }


        [Then(@"the get response contains the entity details")]
        public void ThenTheGetResponseContainsTheEntityDetails()
        {
            FlexEntityApi<Model.EntityAt>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithRequestMethod(_requestContext)
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((entity, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.AreEqual(entity.code.ToLower(),_requestContext.Code.ToLower());
                             });
        }


        [Given(@"a command to retrieve its children '(.*)'")]
        public void GivenACommandToRetrieveItsChildren(string p0)
        {
            if (!string.IsNullOrEmpty(_requestContext.Code))
            {
                _requestContext.Code += ("/" + p0);
            }

        }

        [Then(@"the get response contains the entity details and a collection of child entities")]
        public void ThenTheGetResponseContainsTheEntityDetailsAndACollectionOfChildEntities()
        {
            FlexEntityApi<IEnumerable<Model.EntityAt>>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithRequestMethod(_requestContext)
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((entityCollection, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.IsTrue(entityCollection.Any());
                             });
        }

        [Given(@"a request to get entities filtered by '(.*)'")]
        public void GivenARequestToGetEntitiesFilteredBy(string p0, Table table)
        {
            _requestContext.QueryStringParams = p0.Replace("{now}", DateTimeOffset.UtcNow.ToString("o"));
            _requestContext.RequestTable = table.CreateInstance<Request>();

        }

        [Then(@"the get response contains a collection of entities match the filter")]
        public void ThenTheGetResponseContainsACollectionOfEntitiesMatchTheFilter()
        {
            FlexEntityApi<Model.Entities>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                              WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithRequestMethod(_requestContext)
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60))
                             .Result((entities, statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                                 Assert.IsTrue(entities.entities.Any());
                             });
        }

        [Given(@"a request to delete an entity with an entity code '(.*)'")]
        public void GivenARequestToDeleteAnEntityWithAnEntityCode(string p0, Table table)
        {
            _requestContext.Code = p0;
            _requestContext.RequestTable = table.CreateInstance<Request>();
        }

        [Given(@"a flag indicating to delete all descendants '(.*)'")]
        public void GivenAFlagIndicatingToDeleteAllDescendants(string p0)
        {
                _requestContext.QueryStringParams = p0;
        }


    }
}

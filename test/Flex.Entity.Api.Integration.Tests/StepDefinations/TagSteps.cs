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
    public class TagSteps
    {
        private readonly RequestContext _requestContext;
        public TagSteps(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        [Given(@"a request to create an entity tag temp value which is not exist")]
        public void GivenARequestToCreateAnEntityTagTempValueWhichIsNotExist(Table table)
        {
            _requestContext.RequestTable = table.CreateInstance<Request>();
        }

        [Given(@"a request to update an entity tag temp value")]
        public void GivenARequestToUpdateAnEntityTagTempValue(Table table)
        {
            _requestContext.RequestTable = table.CreateInstance<Request>();
        }

       [Given(@"a request for an entity tag value with '(.*)'")]
        public void GivenARequestForAnEntityTagValueWith(string p0, Table table)
        {
            _requestContext.RequestTable = table.CreateInstance<Request>();
            _requestContext.Code = p0;
        }


        [When(@"the client makes a  PUT request with the body '(.*)'")]
        public void WhenTheClientMakesAPUTRequestWithTheBody(string p0)
        {
            _requestContext.RequestTable.HttpMethod = RequestMethod.Put;
            _requestContext.Body = Utilities.DeserializeJson<TagRequest>(p0);
        }
        

        [Then(@"the response value contains '(.*)', key should be '(.*)'  and update_at should be correct date")]
        public void ThenTheResponseValueContainsKeyShouldBeAndUpdate_AtShouldBeCorrectDate(string p0, string p1)
        {
            FlexEntityApi<TagAt>.SetBaseAddress(_requestContext.RequestTable.BaseUrl)
                .WithUrl(_requestContext.RequestTable.RelativeUri)
                .WithPut(_requestContext.Body)
                .WithAccept(_requestContext.RequestTable.Accept)
                .WithAuthorization((string) FeatureContext.Current["JWTToken"])
                .WithTimeout(TimeSpan.FromSeconds(60))
                .Result((tag, statusCode) =>
                {
                    Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode,"ERROR : Incorrect HTTP StatusCode. Expected : <" +
                        ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                    Assert.IsTrue(tag.key == p1);
                    p0 = p0 == "null" ? null : p0;
                    Assert.IsTrue(tag.value == p0);
                    Assert.IsTrue(tag.updated_at.Date.Day == DateTime.UtcNow.Day);
                    Assert.IsTrue(tag.updated_at.Date.Month == DateTime.UtcNow.Month);
                    Assert.IsTrue(tag.updated_at.Date.Year == DateTime.UtcNow.Year);
                });
        }


        [Then(@"the response contains a collection of entity OE tag values")]
        public void ThenTheResponseContainsACollectionOfEntityOETagValues()
        {

            FlexEntityApi<Tags>.SetBaseAddress(_requestContext.RequestTable.BaseUrl)
                           .WithUrl(_requestContext.RequestTable.RelativeUri)
                           .WithGet()
                           .WithAccept(_requestContext.RequestTable.Accept)
                           .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                           .WithTimeout(TimeSpan.FromSeconds(60))
                           .Result((tags, statusCode) =>
                           {
                               Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                               Assert.IsTrue(tags.tags.Any());
                           });
        }

        [Then(@"the response contains the requrested entity tag details")]
        public void ThenTheResponseContainsTheRequrestedEntityTagDetails()
        {
            FlexEntityApi<TagAt>.SetBaseAddress(_requestContext.RequestTable.BaseUrl).
                            WithUrl(_requestContext.RequestTable.RelativeUri)
                           .WithCode($"{_requestContext.Code}")
                           .WithGet()
                           .WithAccept(_requestContext.RequestTable.Accept)
                           .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                           .WithTimeout(TimeSpan.FromSeconds(60))
                           .Result((tag, statusCode) =>
                           {
                               Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                               Assert.AreEqual(tag.key, _requestContext.Code);
                           });
        }

        [Given(@"a request to delete an entity value with tag key '(.*)'")]
        public void GivenARequestToDeleteAnEntityValueWithTagKey(string p0, Table table)
        {
            _requestContext.RequestTable = table.CreateInstance<Request>();
            _requestContext.Code = p0;
        }
    }
}

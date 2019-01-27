using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flex.Entity.Api.Integration.Tests.Framework;
using Flex.Entity.Api.Model;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Flex.Entity.Api.Integration.Tests.StepDefinations
{
    [Binding]
    public sealed class CommonSteps
    {
        private readonly RequestContext _requestContext;

        public CommonSteps(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        [Then(@"the Api returns with response code '(.*)'")]
        public void ThenTheApiReturnsWithResponseCode(int responseCode)
        {
            ScenarioContext.Current.Add("ExpectedStatusCode", responseCode);
        }

      
        [When(@"the client makes a GET request")]
        public void WhenTheClientMakesAGETRequest()
        {
            _requestContext.RequestTable.HttpMethod = RequestMethod.Get;
        }

        [When(@"the client makes a DELETE request")]
        public void WhenTheClientMakesADELETERequest()
        {
            _requestContext.RequestTable.HttpMethod = RequestMethod.Delete;
        }

        [Then(@"the response is empty")]
        public void ThenTheResponseIsEmpty()
        {
                FlexEntityApi<string>.SetBaseAddress(_requestContext.RequestTable.BaseUrl)
                .WithUrl(_requestContext.RequestTable.RelativeUri)
                .WithCode($"{_requestContext.Code}")
                            .WithRequestMethod(_requestContext)
                            .WithAccept(_requestContext.RequestTable.Accept)
                            .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                            .WithTimeout(TimeSpan.FromSeconds(60))
                            .Result((int statusCode) =>
                             {
                                 Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                             });
        }


        [Then(@"the response contains sucess '(.*)' with error message '(.*)'")]
        public void ThenTheResponseContainsSucessWithErrorMessage(string p0, string p1)
        {
            var request = FlexEntityApi<ApiResult>.SetBaseAddress(_requestContext.RequestTable.BaseUrl)
                             .WithUrl(_requestContext.RequestTable.RelativeUri)
                             .WithAccept(_requestContext.RequestTable.Accept)
                             .WithAuthorization((string)FeatureContext.Current["JWTToken"])
                             .WithTimeout(TimeSpan.FromSeconds(60));

            GetRequestMethod(request);

            request.Result((actualEntityType, statusCode) =>
            {
                Assert.AreEqual(ScenarioContext.Current["ExpectedStatusCode"], statusCode, "ERROR : Incorrect HTTP StatusCode. Expected : <" + ScenarioContext.Current["ExpectedStatusCode"] + "> Actual : <" + statusCode + ">.");
                Assert.AreEqual(actualEntityType.success, bool.Parse(p0));
                Assert.AreEqual(actualEntityType.error, p1);
            });


        }

        private void GetRequestMethod(FlexEntityApi<ApiResult>.HttpRequest<ApiResult> request)
        {
            switch (_requestContext.RequestTable.HttpMethod)
            {
                case RequestMethod.Delete:
                    request.WithCode($"{_requestContext.Code}");
                    request.WithDelete();
                    break;
                case RequestMethod.Post:
                    request.WithPost(_requestContext.Body);
                    break;
                case RequestMethod.Get:
                    request.WithGet();
                    break;
                case RequestMethod.Patch:
                    request.WithPatch(_requestContext.Body);
                    break;
                case RequestMethod.Put:
                    request.WithPut(_requestContext.Body);
                    break;
            }
        }
    }

    public static class CommonHelperExtentions
    {
        internal static FlexEntityApi<T>.HttpRequest<T> WithRequestMethod<T>(this FlexEntityApi<T>.HttpRequest<T> request, RequestContext requestContext) where T : class
        {
            switch (requestContext.RequestTable.HttpMethod)
            {
                case RequestMethod.Delete:
                    request.WithCode($"{requestContext.Code}{requestContext.QueryStringParams ?? string.Empty}");
                    request.WithDelete();
                    break;
                case RequestMethod.Post:
                    request.WithPost(requestContext.Body);
                    break;
                case RequestMethod.Get:
                    request.WithCode($"{requestContext.Code}{requestContext.QueryStringParams ?? string.Empty}");
                    request.WithGet();
                    break;
                case RequestMethod.Patch:
                    request.WithPatch(requestContext.Body);
                    break;
                case RequestMethod.Put:
                    request.WithPut(requestContext.Body);
                    break;
            }
            return request;
        }

       

    }
}

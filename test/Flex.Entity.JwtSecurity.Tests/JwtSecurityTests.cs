using Flex.Entity.JWTSecurity;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Flex.Entity.Api;
using Flex.Entity.Api.App_Start;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace Flex.Entity.JwtSecurity.Tests
{
    [TestClass()]
    public class JwtSecurityTests
    {

        private HttpConfiguration config;
        private HttpServer server;
        private HttpClient client;
        private string SymmetricKey { get; set; }
        private string Audience { get; set; }
        public string Issuer { get; set; }
        public string Domain { get; set; }

        private string GenerateToken()
        {
            Audience = WebConfigurationManager.AppSettings["auth0:ClientId"];
            SymmetricKey = WebConfigurationManager.AppSettings["auth0:ClientSecret"];
            Issuer = WebConfigurationManager.AppSettings["auth0:Issuer"];
           return JwtTokenGenerator.GenerateToken(SymmetricKey, Audience, Issuer);
        }
        [TestMethod()]
        public void AuthenticateAsyncTest() 
        {
            HttpRequestMessage request = new HttpRequestMessage();
            HttpControllerContext controllerContext = new HttpControllerContext();
            controllerContext.Request = request;
            HttpActionContext context = new HttpActionContext();
            context.ControllerContext = controllerContext;
            HttpAuthenticationContext m = new HttpAuthenticationContext(context, null);
            HttpRequestHeaders headers = request.Headers;
            AuthenticationHeaderValue authorization = new AuthenticationHeaderValue("scheme");
            headers.Authorization = authorization;
            Assert.Fail();
        }

        [TestMethod]

        private void Setup_Host_JwtConfiguration()
        {
            GlobalConfiguration.Configure(config =>
            {

                HandlerConfig.Register(config);
                WebApiFilterConfig.Register(config);
                JsonWebTokenConfig.Register(config);
                WebApiConfig.Register(config);
            });
        }
        [TestMethod()]
        public void Get_and_check_if_the_token_is_valid()
        {
            var token = GenerateToken();
            ClaimsPrincipal principal= JsonWebToken.ValidateToken(token, SymmetricKey, Audience, false, Issuer);
            Assert.IsNotNull(principal);
        }
        [TestMethod()]
        public void IF_token_is_valid_create_claims()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AuthenticateAsyncTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ChallengeAsyncTest()
        {
            Assert.Fail();
        }

       

        [TestMethod()]
        public void ValidateTokenTest()
        {
         //   JwtTokenGenerator.GenerateToken()
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSecurityContextTest()
        {
            Assert.Fail();
        }

        public IRestResponse SendBearerToken()
        {
            var client = new RestClient("http://localhost:13659/entities/types/");
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization",
                "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6Im5vb3J1ZGRpbi5rYXBhc2lAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImFwcF9tZXRhZGF0YSI6eyJhc3NldCI6eyJyb290IjoianNiIiwicGVybWlzc2lvbiI6OX0sInNlcnZpY2UiOnsicm9vdCI6Im9lIiwicGVybWlzc2lvbiI6OX19LCJ1c2VyX21ldGFkYXRhIjp7ImNsaWVudCI6Im9lIiwiZGVmYXVsdGFwaSI6ImVudGl0aWVzIiwiZGVmYXVsdGxvYWQiOiJob3R3YXRlciIsImNvbG9yIjoicmVkIn0sIm5hbWUiOiJub29ydWRkaW4ua2FwYXNpQGdtYWlsLmNvbSIsImlzcyI6Imh0dHBzOi8vb3BlbmVuZXJnaS5ldS5hdXRoMC5jb20vIiwic3ViIjoiYXV0aDB8NTdlMDBhNTUxY2M5ODU4YzFlYjhiODZhIiwiYXVkIjoib3N5QWFrSGw4WU1BUlI1SUh0dnI0MXdxaXZPZnh6ZUkiLCJleHAiOjE0NzU4NDc1NTEsImlhdCI6MTQ3NTc2MTE1MX0.vLbxVM28yaZtXo77WGHqD8LAtYvKuS7xemSG8qsvfos");//"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJlbWFpbCI6Imh0YXQuc3ViQGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJ1c2VyX2lkIjoiYXV0aDB8NTdmMzgyZGVlM2I5N2JkYjM2ZDhhNWFmIiwicGljdHVyZSI6Imh0dHBzOi8vcy5ncmF2YXRhci5jb20vYXZhdGFyLzg3YjBhZmYxOGU0MDAzMGE2NGQ5MTE3NDA2YjEzNjM0P3M9NDgwJnI9cGcmZD1odHRwcyUzQSUyRiUyRmNkbi5hdXRoMC5jb20lMkZhdmF0YXJzJTJGaHQucG5nIiwibmlja25hbWUiOiJodGF0LnN1YiIsInVwZGF0ZWRfYXQiOiIyMDE2LTEwLTA0VDExOjA1OjIwLjE1N1oiLCJjcmVhdGVkX2F0IjoiMjAxNi0xMC0wNFQxMDoyMjoyMi45MzVaIiwibmFtZSI6Imh0YXQuc3ViQGdtYWlsLmNvbSIsInVzZXJfbWV0YWRhdGEiOnsiY2xpZW50Ijoib2UiLCJkZWZhdWx0YXBpIjoiZW50aXRpZXMiLCJkZWZhdWx0bG9hZCI6ImhvdHdhdGVyIiwiY29sb3IiOiJyZWQifSwiYXBwX21ldGFkYXRhIjp7ImFzc2V0Ijp7InJvb3QiOiJqc2IiLCJwZXJtaXNzaW9uIjo5fSwic2VydmljZSI6eyJyb290Ijoib2UiLCJwZXJtaXNzaW9uIjo5fX0sImlkZW50aXRpZXMiOlt7InVzZXJfaWQiOiI1N2YzODJkZWUzYjk3YmRiMzZkOGE1YWYiLCJwcm92aWRlciI6ImF1dGgwIiwiY29ubmVjdGlvbiI6IlVzZXJuYW1lLVBhc3N3b3JkLUF1dGhlbnRpY2F0aW9uIiwiaXNTb2NpYWwiOmZhbHNlfV0sImxhc3RfaXAiOiI4MC43MS4yNi45NyIsImxhc3RfbG9naW4iOiIyMDE2LTEwLTA0VDEwOjI2OjQ4Ljc2NFoiLCJsb2dpbnNfY291bnQiOjIsImJsb2NrZWRfZm9yIjpbXSwiZ3VhcmRpYW5fZW5yb2xsbWVudHMiOltdLCJnbG9iYWxfY2xpZW50X2lkIjoiSlAxcW5vdTAwdTBFdmhmYUVydU9pMWVDYUNzRFhXREoifQ.LyCWR8PFJdBMCn9naULYCKR-gleccJeCx8jqaOrGHkk");//

            IRestResponse response = client.Execute(request);
            return response;
        }
    }


}

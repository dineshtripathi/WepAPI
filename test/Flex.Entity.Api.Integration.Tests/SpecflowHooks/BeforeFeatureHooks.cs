using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Flex.Entity.Api.Helpers;
using Flex.Entity.Api.Integration.Tests.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Flex.Entity.Api.Integration.Tests.SpecflowHooks
{
    [Binding]
    public sealed class BeforeFeatureHooks
    {
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks

        [BeforeFeature]
        public static void BeforFeature()
        {
            FeatureContext.Current.Add("AUT", TestConfiguration.ApplicationUnderTestBaseUrl);
            //Get Auth Token and store in Context
            if (TestConfiguration.UseAuthentication)
            {
                #region TokenWithAllvalidPayloadParameter

                        if (FeatureContext.Current.ContainsKey("JWTToken"))
                            FeatureContext.Current.Remove("JWTToken");

                       var validTokenPayload = AuthorizedTokenPayload("nooruddin.kapasi@gmail.com",
                                                                    new Asset()
                                                                    {
                                                                        Permission = 0x02, Root = "oe1"
                                                                    }, 
                                                                    new Helpers.Service()
                                                                    {
                                                                        Root = "oe1", Permission = 0x20
                                                                    });
                       var id_validtoken= JwtHelper.GenerateToken(TestConfiguration.audience_secretKey, TestConfiguration.audience_clientId,
                            TestConfiguration.audience_issuer,
                            validTokenPayload
                            );

                        if (id_validtoken != null) FeatureContext.Current.Add("JWTToken", id_validtoken);
              
                #endregion

                #region TokenWithoutEmailid

                        if (FeatureContext.Current.ContainsKey("JWTTokenWithoutEmailId"))
                            FeatureContext.Current.Remove("JWTTokenWithoutEmailId");
                        string noEmailidConfigured = null;
                        var tokenPayloadWithoutEmaild = AuthorizedTokenPayload(noEmailidConfigured, new Asset()
                                                                            {
                                                                                Permission = 0x02, Root = "oe1"
                                                                            },
                                                                            new Helpers.Service()
                                                                            {
                                                                                Root = "oe1", Permission = 0x20
                                                                            });

                        var id_tokenwithoutEmailid = JwtHelper.GenerateToken(TestConfiguration.audience_secretKey, TestConfiguration.audience_clientId,
                           TestConfiguration.audience_issuer, tokenPayloadWithoutEmaild
                           );
                        if (id_tokenwithoutEmailid != null) FeatureContext.Current.Add("JWTTokenWithoutEmailId", id_tokenwithoutEmailid);

                #endregion TokenWithoutEmailid

                #region TokenWithoutPermission
                    if (FeatureContext.Current.ContainsKey("JWTTokenWithoutPermission"))
                        FeatureContext.Current.Remove("JWTTokenWithoutPermission");
                    var tokenPayloadWithoutPermission = AuthorizedTokenPayload("nooruddin.kapasi@gmail.com", new Asset()
                                                                        {
                                                                            Root = "oe1"
                                                                        },
                                                                        new Helpers.Service()
                                                                        {
                                                                            Root = "oe1",
                                                                        });

                    var id_tokenwithoutPermission = JwtHelper.GenerateToken(TestConfiguration.audience_secretKey, TestConfiguration.audience_clientId,
                       TestConfiguration.audience_issuer, tokenPayloadWithoutPermission
                       );
                    if (id_tokenwithoutPermission != null) FeatureContext.Current.Add("JWTTokenWithoutPermission", id_tokenwithoutPermission);

                #endregion TokenWithoutPermission

                #region TokenWithoutAsset
                    if (FeatureContext.Current.ContainsKey("JWTTokenWithoutAsset"))
                        FeatureContext.Current.Remove("JWTTokenWithoutAsset");
                    var tokenPayloadWithoutAsset = AuthorizedTokenPayload("nooruddin.kapasi@gmail.com", null,
                                                                        new Helpers.Service()
                                                                        {
                                                                            Root = "oe1",
                                                                        });

                    var id_tokenwithoutAsset = JwtHelper.GenerateToken(TestConfiguration.audience_secretKey, TestConfiguration.audience_clientId,
                       TestConfiguration.audience_issuer, tokenPayloadWithoutAsset
                       );
                    if (id_tokenwithoutAsset != null) FeatureContext.Current.Add("JWTTokenWithoutAsset", id_tokenwithoutAsset);

                #endregion TokenWithoutAsset

                #region TokenWithoutService
                    if (FeatureContext.Current.ContainsKey("JWTTokenWithoutService"))
                        FeatureContext.Current.Remove("JWTTokenWithoutService");
                    var tokenPayloadWithoutService = AuthorizedTokenPayload("nooruddin.kapasi@gmail.com", new Asset()
                                                                            {
                                                                                Root = "oe1"
                                                                            },null);

                    var id_tokenwithoutService = JwtHelper.GenerateToken(TestConfiguration.audience_secretKey, TestConfiguration.audience_clientId,
                       TestConfiguration.audience_issuer, tokenPayloadWithoutService
                       );
                    if (id_tokenwithoutService != null) FeatureContext.Current.Add("JWTTokenWithoutService", id_tokenwithoutService);

                #endregion TokenWithoutService

                #region TokenWithoutAppMetadata
                    if (FeatureContext.Current.ContainsKey("JWTTokenWithoutAppMetadata"))
                        FeatureContext.Current.Remove("JWTTokenWithoutAppMetadata");
                    var tokenPayloadWithoutAppMetadata = AuthorizedTokenPayload("nooruddin.kapasi@gmail.com", null, null);

                    var id_tokenwithoutAppMetadata = JwtHelper.GenerateToken(TestConfiguration.audience_secretKey, TestConfiguration.audience_clientId,
                       TestConfiguration.audience_issuer, tokenPayloadWithoutAppMetadata
                       );
                    if (id_tokenwithoutAppMetadata != null) FeatureContext.Current.Add("JWTTokenWithoutAppMetadata", id_tokenwithoutAppMetadata);

                #endregion TokenWithoutService
                // RestClient client = new RestClient(TestConfiguration.Auth0Url.GetBaseUrl());
                // RestRequest request = new RestRequest(TestConfiguration.Auth0Url.LocalPath, Method.POST)
                //  {
                //       JsonSerializer = new CustomJsonNetSerializer()
                //   };
                //   request.AddJsonBody(TestConfiguration.Auth0Json);
                //   var response = client.Execute(request);
                //   if (response.ErrorException == null)
                {
                    //   dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response.Content, new ExpandoObjectConverter());
                    //  if (obj.token_type == "bearer")
                    {
                       // FeatureContext.Current.Add("JWTToken", obj.id_token);
                    }

                }
            }

        }

        private static AppMetadata AuthorizedTokenPayload(string emailid,Asset asset,Helpers.Service service)
        {
            return new AppMetadata(emailid,asset,service);
        }
    }
}

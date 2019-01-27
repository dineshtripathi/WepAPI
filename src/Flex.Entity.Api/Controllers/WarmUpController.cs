using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Flex.Entity.Api.Helpers;
using Flex.Entity.Api.Model;
using JWT;

namespace Flex.Entity.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class WarmUpController : Controller
    {

        // GET: WarmUp
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string id = null)
        {
            var ok = false;
            int portNumber = 80;
            if (id != null)
            {
                var match = Regex.Match(id, @"^.*(thisrequestisfromappinit)(\{(\d+)\})?$");
                if (match.Success)
                {
                    int port;
                    if (Int32.TryParse(match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).LastOrDefault(),
                        out port))
                    {
                        portNumber = port;
                    }
                    ok = true;
                }
            }
            if (!ok)
                return HttpNotFound("resource not found");
            try
            {
                //Get a Token and call the api
                var audience = WebConfigurationManager.AppSettings["auth0:ClientId"];
                var symmetricKey = WebConfigurationManager.AppSettings["auth0:ClientSecret"];
                var issuer = WebConfigurationManager.AppSettings["auth0:Issuer"];

                var token = JwtHelper.GenerateToken(symmetricKey, audience, issuer);
                var baseUrl = Request.Url.GetBaseUrl();
                var apiBaseUri = new UriBuilder(baseUrl);
                if (portNumber != 80)
                {
                    apiBaseUri.Port = portNumber;
                }

                //{
                //    var apiUri = new Uri(apiBaseUri + "entities/types", UriKind.Absolute);
                //    WebRequest request = WebRequest.Create(apiUri);
                //    request.AuthenticationLevel = AuthenticationLevel.None;
                //    request.Headers.Add("Authorization", "Bearer " + token);
                //    var result = request.GetResponse();

                //}
                {
                    var apiUri = new Uri(apiBaseUri + "entities/types/C", UriKind.Absolute);
                    WebRequest request = WebRequest.Create(apiUri);
                    request.AuthenticationLevel = AuthenticationLevel.None;
                    request.Headers.Add("Authorization", "Bearer " + token);
                    var result = request.GetResponse();
                }

            }
            catch (Exception)
            {
                //Eat up exception during warmup
            }
            return Json(new ApiResult() {success = true, error = ""}, JsonRequestBehavior.AllowGet);
        }
    }
}
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Flex.Entity.Api.Helpers
{
    /// <summary>
    /// Controller to warmup the api
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetBaseUrl(this Uri uri)
        {
            return uri.Scheme + "://" + uri.DnsSafeHost +
                   ((uri.IsDefaultPort) ? "/" : ":" + uri.Port.ToString() + "/");
        }

        private static readonly MediaTypeWithQualityHeaderValue HalJsonMediaType = MediaTypeWithQualityHeaderValue.Parse("application/hal+json");
        private static readonly MediaTypeWithQualityHeaderValue HalXmlMediaType = MediaTypeWithQualityHeaderValue.Parse("application/hal+xml");
        private static readonly MediaTypeWithQualityHeaderValue JsonMediaType = MediaTypeWithQualityHeaderValue.Parse("application/json");
        private static readonly MediaTypeWithQualityHeaderValue XmlMediaType = MediaTypeWithQualityHeaderValue.Parse("application/xml");

        /// <summary>
        /// checkes if the Accept Header contains HAL media types
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public static bool DoesClientAcceptHalContent( this HttpRequestMessage requestMessage)
        {
            return (
                        (
                        requestMessage.Headers.Accept.Contains(HalJsonMediaType)
                        ||
                        requestMessage.Headers.Accept.Contains(HalXmlMediaType)
                        ) &&
                        (
                                !requestMessage.Headers.Accept.Contains(JsonMediaType)
                                &&
                                !requestMessage.Headers.Accept.Contains(XmlMediaType)
                        )
                    );
        }

    }
}
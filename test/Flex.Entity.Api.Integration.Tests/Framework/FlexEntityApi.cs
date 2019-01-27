using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;

namespace Flex.Entity.Api.Integration.Tests.Framework
{
    public class FlexEntityApi<T> where T : class
    {
        public static HttpRequest<T> SetBaseAddress(Uri baseUri)
        {
            return new HttpRequest<T>(baseUri);
        }

        public class HttpRequest<TE> where TE : class
        {


            private readonly RestClient _client;

            private string _actualJson;
            private int _httpResponseCode;
            private object _requestContent;

            public HttpRequest(Uri baseUri)
            {
                _client = new RestClient(baseUri);

            }

            private Uri Url { get; set; }

            private string Authorization { get; set; }

            private string Accept { get; set; }

            private int Timeout { get; set; }

            private string Code { get; set; }

            private RequestMethod HttpMethod { get; set; }

            public HttpRequest<TE> WithUrl(Uri relativeUri)
            {
                this.Url = relativeUri;
                return this;
            }

            public HttpRequest<TE> WithGet()
            {
                HttpMethod = RequestMethod.Get;
                return this;
            }

            public HttpRequest<TE> WithPost<TR>(TR postBody) where TR : class
            {
                HttpMethod = RequestMethod.Post;
                _requestContent = postBody;
                return this;
            }

            public HttpRequest<TE> WithPatch<TR>(TR patchAnonymous) where TR : class
            {
                HttpMethod = RequestMethod.Patch;
                _requestContent = patchAnonymous;
                return this;
            }

            public HttpRequest<TE> WithPut<TR>(TR patchBody)
            {
                HttpMethod = RequestMethod.Put;
                _requestContent = patchBody;
                return this;
            }

            public HttpRequest<TE> WithDelete()
            {
                HttpMethod = RequestMethod.Delete;
                return this;
            }

            public HttpRequest<TE> WithAccept(string accept)
            {
                _client.AddDefaultHeader("Accept", accept);
                return this;
            }

            public HttpRequest<TE> WithAuthorization(string authorization)
            {
                if (!string.IsNullOrWhiteSpace(authorization))
                {
                    _client.Authenticator = new JwtAuthenticator(authorization);
                }
                //_client.AddDefaultHeader("Authorization", authorization);
                return this;
            }

            public HttpRequest<TE> WithTimeout(TimeSpan timeout)
            {
                _client.Timeout = (int)timeout.TotalMilliseconds;
                return this;
            }


            public HttpRequest<TE> WithCode(string code)
            {
                Code = code;
                return this;
            }

            public void Result(Action<TE> processResult)
            {
                ExecuteHttpMethod().Wait();
                var responseValue = Utilities.DeserializeJson<TE>(_actualJson) ?? Utilities.DeserializeSimpleJson<TE>(_actualJson);//Utilities.Utilities.DeserializeJson<TE>(_actualJson);
                processResult(responseValue);

            }
            public void Result(Action<TE, int> processResult)
            {

                ExecuteHttpMethod().Wait();
                var responseValue = Utilities.DeserializeJson<TE>(_actualJson) ?? Utilities.DeserializeSimpleJson<TE>(_actualJson);//Utilities.Utilities.DeserializeJson<TE>(_actualJson);
                processResult(responseValue, _httpResponseCode);

            }

            public void Result(Action<int> processResult)
            {

                ExecuteHttpMethod().Wait();
                //var responseValue = Utilities.DeserializeJson<TE>(_actualJson) ?? Utilities.DeserializeSimpleJson<TE>(_actualJson);
                processResult(_httpResponseCode);
            }


            private async Task ExecuteHttpMethod()
            {
                switch (HttpMethod)
                {
                    case RequestMethod.Get:
                        await ExecuteGetRequest().ConfigureAwait(false);
                        break;
                    case RequestMethod.Post:
                        await ExecutePostRequest().ConfigureAwait(false);
                        break;
                    case RequestMethod.Patch:
                        await ExecutePatchRequest().ConfigureAwait(false);
                        break;
                    case RequestMethod.Put:
                        await ExecutePutRequest().ConfigureAwait(false);
                        break;
                    case RequestMethod.Delete:
                        await ExecuteDeleteRequest().ConfigureAwait(false);
                        break;
                }
            }

            private async Task ExecutePutRequest()
            {
                IRestResponse httpResponse = null;
                //HttpContent content = new StringContent(_requestContent);
                //content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(_contentType);

                if (Code != null)
                {
                    RestRequest request = new RestRequest(Url.OriginalString.EndsWith("/")? (Url.OriginalString + Code): (Url.OriginalString + "/"+ Code), Method.PUT)
                    {
                        JsonSerializer = new CustomJsonNetSerializer()
                    };
                    request.AddJsonBody(_requestContent);

                    httpResponse = await _client.ExecuteTaskAsync(request).ConfigureAwait(false);
                    
                }
                else
                {
                    RestRequest request = new RestRequest(Url, Method.PUT)
                    {
                        JsonSerializer = new CustomJsonNetSerializer()
                    };
                    request.AddJsonBody(_requestContent);

                    httpResponse = await _client.ExecuteTaskAsync(request).ConfigureAwait(false);
                }

                _actualJson = httpResponse.Content;

                _httpResponseCode = (int)httpResponse.StatusCode;

            }

            private async Task ExecutePatchRequest()
            {
                IRestResponse httpResponse = null;
                //HttpContent content = new StringContent(_requestContent);
                //content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(_contentType);

                if (Code != null)
                {
                    RestRequest request = new RestRequest(Url.OriginalString.EndsWith("/")? (Url.OriginalString + Code): (Url.OriginalString + "/"+ Code), Method.PATCH)
                    {
                        JsonSerializer = new CustomJsonNetSerializer()
                    };
                    request.AddJsonBody(_requestContent);

                    httpResponse = await _client.ExecuteTaskAsync(request).ConfigureAwait(false);

                }
                else
                {
                    RestRequest request = new RestRequest(Url, Method.PATCH)
                    {
                        JsonSerializer = new CustomJsonNetSerializer()
                    };
                    request.AddJsonBody(_requestContent);

                    httpResponse = await _client.ExecuteTaskAsync(request).ConfigureAwait(false);
                }

                _actualJson = httpResponse.Content;

                _httpResponseCode = (int)httpResponse.StatusCode;

            }

            private async Task ExecutePostRequest()
            {
                IRestResponse httpResponse = null;
                //HttpContent content = new StringContent(_requestContent);
                //content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(_contentType);

                if (Code != null)
                {
                    RestRequest request = new RestRequest(Url.OriginalString.EndsWith("/")? (Url.OriginalString + Code): (Url.OriginalString + "/"+ Code), Method.POST)
                    {
                        JsonSerializer = new CustomJsonNetSerializer()
                    };
                    request.AddJsonBody(_requestContent);

                    httpResponse = await _client.ExecuteTaskAsync(request).ConfigureAwait(false);

                }
                else
                {
                    RestRequest request = new RestRequest(Url, Method.POST)
                    {
                        JsonSerializer = new CustomJsonNetSerializer()
                    };
                    request.AddJsonBody(_requestContent);

                    httpResponse = await _client.ExecuteTaskAsync(request).ConfigureAwait(false);
                }

                _actualJson = httpResponse.Content;

                _httpResponseCode = (int)httpResponse.StatusCode;
            }

            private async Task ExecuteGetRequest()
            {
                IRestResponse httpResponse = null;
                //HttpContent content = new StringContent(_requestContent);
                //content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(_contentType);

                if (Code != null)
                {
                    
                    RestRequest request = new RestRequest(Url.OriginalString.EndsWith("/")? (Url.OriginalString + Code): (Url.OriginalString + "/"+ Code), Method.GET);
                    httpResponse = await _client.ExecuteTaskAsync(request).ConfigureAwait(false);

                }
                else
                {
                    RestRequest request = new RestRequest(Url, Method.GET);
                    httpResponse = await _client.ExecuteTaskAsync(request).ConfigureAwait(false);
                }

                _actualJson = httpResponse.Content;

                _httpResponseCode = (int)httpResponse.StatusCode;
            }

            private async Task ExecuteDeleteRequest()
            {
                IRestResponse httpResponse = null;

                if (Code != null)
                {

                    RestRequest request = new RestRequest(Url.OriginalString.EndsWith("/") ? (Url.OriginalString + Code) : (Url.OriginalString + "/" + Code), Method.DELETE);
                    httpResponse = await _client.ExecuteTaskAsync(request).ConfigureAwait(false);

                }
                else
                {
                    RestRequest request = new RestRequest(Url, Method.GET);
                    httpResponse = await _client.ExecuteTaskAsync(request).ConfigureAwait(false);
                }

                _actualJson = httpResponse.Content;

                _httpResponseCode = (int)httpResponse.StatusCode;
            }

        }
    }
}

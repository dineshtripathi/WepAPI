using System.Net.Http;

namespace Flex.Entity.Security
{
    public class ErrorResult
    {
        public string ReasonPhrase { get; set; }
        public HttpRequestMessage Request { get; set; }
    }
}
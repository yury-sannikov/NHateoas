using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NHateoas.Integration.Tests
{
    public class InMemorySerializationHandler : DelegatingHandler
    {

        public InMemorySerializationHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            request.Content = ConvertToStreamContent(request.Content);

            return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>((responseTask) =>
            {
                HttpResponseMessage response = responseTask.Result;

                response.Content = ConvertToStreamContent(response.Content);

                return response;
            }, cancellationToken);
        }

        private StreamContent ConvertToStreamContent(HttpContent originalContent)
        {
            if (originalContent == null)
            {
                return null;
            }

            var streamContent = originalContent as StreamContent;

            if (streamContent != null)
            {
                return streamContent;
            }

            var ms = new MemoryStream();

            // **** NOTE: ideally you should NOT be doing calling Wait() as its going to block this thread ****
            // if the original content is an ObjectContent, then this particular CopyToAsync() call would cause the MediaTypeFormatters to 
            // take part in Serialization of the ObjectContent and the result of this serialization is stored in the provided target memory stream.
            originalContent.CopyToAsync(ms).Wait();


            ms.Position = 0;

            streamContent = new StreamContent(ms);

            // copy headers from the original content
            foreach (KeyValuePair<string, IEnumerable<string>> header in originalContent.Headers)
            {
                streamContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return streamContent;
        }
    }
}

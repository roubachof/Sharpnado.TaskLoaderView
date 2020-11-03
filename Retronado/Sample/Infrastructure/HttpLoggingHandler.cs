using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Infrastructure
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        public HttpLoggingHandler(HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var req = request;
            string id = Guid.NewGuid().ToString();
            string msg = $"[{id} -   Request]";

            Debug.WriteLine($"{msg}========Start==========");
            Debug.WriteLine($"{msg} {req.Method} {req.RequestUri.PathAndQuery} {req.RequestUri.Scheme}/{req.Version}");
            Debug.WriteLine($"{msg} Host: {req.RequestUri.Scheme}://{req.RequestUri.Host}");

            foreach (var header in req.Headers)
            {
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (req.Content != null)
            {
                foreach (var header in req.Content.Headers)
                {
                    Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (req.Content is StringContent
                    || IsTextBasedContentType(req.Headers)
                    || IsTextBasedContentType(req.Content.Headers))
                {
                    string result = await req.Content.ReadAsStringAsync();

                    Debug.WriteLine($"{msg} Content:");
                    Debug.WriteLine($"{msg} {result}");
                }
            }

            var start = DateTime.Now;

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var end = DateTime.Now;

            Debug.WriteLine($"{msg} Duration: {end - start}");
            Debug.WriteLine($"{msg}==========End==========");

            msg = $"[{id} - Response]";
            Debug.WriteLine($"{msg}=========Start=========");

            var resp = response;

            Debug.WriteLine(
                $"{msg} {req.RequestUri.Scheme.ToUpper()}/{resp.Version} {(int)resp.StatusCode} {resp.ReasonPhrase}");

            foreach (var header in resp.Headers)
            {
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (resp.Content != null)
            {
                foreach (var header in resp.Content.Headers)
                {
                    Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (resp.Content is StringContent
                    || IsTextBasedContentType(resp.Headers)
                    || IsTextBasedContentType(resp.Content.Headers))
                {
                    start = DateTime.Now;
                    string result = await resp.Content.ReadAsStringAsync();
                    end = DateTime.Now;

                    Debug.WriteLine($"{msg} Content:");
                    Debug.WriteLine($"{msg} {result}");
                    Debug.WriteLine($"{msg} Duration: {end - start}");
                }
            }

            Debug.WriteLine($"{msg}==========End==========");
            return response;
        }

        private readonly string[] types = { "html", "text", "xml", "json", "txt", "x-www-form-urlencoded" };

        private bool IsTextBasedContentType(HttpHeaders headers)
        {
            IEnumerable<string> values;
            if (!headers.TryGetValues("Content-Type", out values))
            {
                return false;
            }

            string header = string.Join(" ", values).ToLowerInvariant();

            return types.Any(t => header.Contains(t));
        }
    }
}
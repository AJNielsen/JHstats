using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace JHStatsWebsite.Tests.MessageHandlers
{
    public class TestMessageHandler : DelegatingHandler
    {
        private Stream stream;
        private StreamWriter streamWriter;

        public TestMessageHandler()
        {
            stream = new MemoryStream();
            streamWriter = new StreamWriter(stream);
        }

        public void AddMessagesToStream(IEnumerable<string> messages)
        {
            stream.Position = stream.Length;
            foreach(string msg in messages)
            {
                streamWriter.WriteLine(msg);
            }
            streamWriter.Flush();
            stream.Position = 0;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            if(stream != null)
            {
                httpResponseMessage.Content = new StreamContent(stream);
            }

            return Task.FromResult(httpResponseMessage);
        }

        protected override void Dispose(bool disposing)
        {
            streamWriter?.Dispose();
            stream?.Dispose();
            base.Dispose(disposing);
        }
    }
}

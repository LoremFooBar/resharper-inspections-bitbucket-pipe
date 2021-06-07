using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Resharper.CodeInspections.BitbucketPipe.Options;

namespace Resharper.CodeInspections.BitbucketPipe.Tests
{
    public class BitbucketClientMock
    {
        public BitbucketClient BitbucketClient { get; }
        public Mock<HttpMessageHandler> HttpMessageHandlerMock { get; }

        public BitbucketClientMock(bool useAuthentication, bool createBuildStatus)
        {
            HttpMessageHandlerMock = new Mock<HttpMessageHandler>();
            HttpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            var httpClient = new HttpClient(HttpMessageHandlerMock.Object);

            var authOptionsPoco = useAuthentication
                ? new BitbucketAuthenticationOptions {Username = "user", AppPassword = "pass"}
                : new BitbucketAuthenticationOptions {Username = "", AppPassword = ""};

            var pipeOptionsPoco = new PipeOptions {CreateBuildStatus = createBuildStatus};

            var authOptions = Mock.Of<IOptions<BitbucketAuthenticationOptions>>(_ => _.Value == authOptionsPoco);
            var pipeOptions = Mock.Of<IOptions<PipeOptions>>(_ => _.Value == pipeOptionsPoco);

            BitbucketClient =
                new BitbucketClient(httpClient, authOptions, pipeOptions, NullLogger<BitbucketClient>.Instance);
        }
    }
}

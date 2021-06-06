using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BitbucketClientTests
{
    public class BitbucketClientSpecificationBase : SpecificationBase
    {
        protected BitbucketClient BitbucketClient { get; private set; }
        protected Mock<HttpMessageHandler> HttpMessageHandlerMock { get; private set; }
        protected virtual bool UseAuthentication => true;
        protected virtual bool CreateBuildStatus => true;

        protected override void Given()
        {
            base.Given();

            EnvironmentSetup.SetupEnvironment();

            HttpMessageHandlerMock = new Mock<HttpMessageHandler>();
            HttpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            var httpClient = new HttpClient(HttpMessageHandlerMock.Object);

            var authOptionsPoco = UseAuthentication
                ? new BitbucketAuthenticationOptions {Username = "user", AppPassword = "pass"}
                : new BitbucketAuthenticationOptions {Username = "", AppPassword = ""};

            var pipeOptionsPoco = new PipeOptions {CreateBuildStatus = CreateBuildStatus};

            var authOptions = Mock.Of<IOptions<BitbucketAuthenticationOptions>>(_ => _.Value == authOptionsPoco);
            var pipeOptions = Mock.Of<IOptions<PipeOptions>>(_ => _.Value == pipeOptionsPoco);

            BitbucketClient =
                new BitbucketClient(httpClient, authOptions, pipeOptions, NullLogger<BitbucketClient>.Instance);
        }
    }
}

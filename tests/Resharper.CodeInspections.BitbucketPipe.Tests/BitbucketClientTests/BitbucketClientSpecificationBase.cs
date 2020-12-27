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
        protected virtual bool UseOAuth => true;

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

            var options = UseOAuth
                ? new BitbucketAuthenticationOptions {OAuthKey = "key", OAuthSecret = "secret"}
                : new BitbucketAuthenticationOptions {OAuthKey = "", OAuthSecret = ""};

            var authOptions = Mock.Of<IOptions<BitbucketAuthenticationOptions>>(_ => _.Value == options);

            BitbucketClient = new BitbucketClient(httpClient, authOptions, NullLogger<BitbucketClient>.Instance);
        }
    }
}

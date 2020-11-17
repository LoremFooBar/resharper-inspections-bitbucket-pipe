using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BitbucketClientTests
{
    public class When_Sending_A_Report_To_Bitbucket : SpecificationBase
    {
        private BitbucketClient _bitbucketClient;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;

        protected override void Given()
        {
            base.Given();

            EnvironmentSetup.SetupEnvironment();

            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            var authOptions = Mock.Of<IOptions<BitbucketAuthorizationOptions>>(options =>
                options.Value == new BitbucketAuthorizationOptions {AccessToken = ""});

            _bitbucketClient = new BitbucketClient(httpClient, authOptions, NullLogger<BitbucketClient>.Instance);
        }

        protected override async Task WhenAsync()
        {
            await base.WhenAsync();

            await _bitbucketClient.CreateReportAsync(new PipelineReport(), Enumerable.Repeat(new Annotation(), 101));
        }

        [Then]
        public void It_Should_Send_Report_And_Annotations_To_Bitbucket()
        {
            _httpMessageHandlerMock
                .Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Exactly(3), ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Then]
        public void It_Should_Serialize_Report_And_Annotations_Using_Snake_Case()
        {
            _httpMessageHandlerMock
                .Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(message =>
                        message.Content.ReadAsStringAsync().Result.Contains("\"report_type\":")),
                    ItExpr.IsAny<CancellationToken>());

            _httpMessageHandlerMock
                .Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Exactly(2), ItExpr.Is<HttpRequestMessage>(message =>
                        message.Content.ReadAsStringAsync().Result.Contains("\"annotation_type\":")),
                    ItExpr.IsAny<CancellationToken>());
        }
    }
}

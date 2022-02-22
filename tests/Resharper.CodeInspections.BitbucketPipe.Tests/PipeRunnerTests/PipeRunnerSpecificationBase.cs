using System.Net;
using Moq;
using Moq.Contrib.HttpClient;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests
{
    public class PipeRunnerSpecificationBase : SpecificationBase
    {
        protected TestPipeRunner TestPipeRunner;
        protected Mock<HttpMessageHandler> MessageHandlerMock;

        protected override void Given()
        {
            base.Given();

            MessageHandlerMock = new Mock<HttpMessageHandler>();
            // return OK for any non-GET request
            MessageHandlerMock
                .SetupRequest(request => request.Method != HttpMethod.Get)
                .ReturnsResponse(HttpStatusCode.OK);
        }

        protected override async Task WhenAsync()
        {
            await base.WhenAsync();

            await TestPipeRunner.RunPipeAsync();
        }
    }
}

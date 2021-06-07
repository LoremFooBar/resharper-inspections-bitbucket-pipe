using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests
{
    public class PipeRunnerSpecificationBase : SpecificationBase
    {
        protected BitbucketClientMock BitbucketClientMock { get; set; }
        protected Mock<HttpMessageHandler> HttpMessageHandlerMock => BitbucketClientMock.HttpMessageHandlerMock;

        protected override async Task WhenAsync()
        {
            await new TestPipeRunner(BitbucketClientMock).RunPipeAsync();
        }

        protected void VerifySendAsyncCalls(Times times, Expression<Func<HttpRequestMessage, bool>> requestMatch)
        {
            HttpMessageHandlerMock.Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", times, ItExpr.Is(requestMatch),
                    ItExpr.IsAny<CancellationToken>());
        }
    }
}

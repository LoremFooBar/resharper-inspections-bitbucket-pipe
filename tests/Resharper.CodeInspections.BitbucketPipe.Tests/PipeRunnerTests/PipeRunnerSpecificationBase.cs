using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests
{
    public class PipeRunnerSpecificationBase : SpecificationBase
    {
        protected BitbucketClientMock BitbucketClientMock { get; set; }
        protected Mock<HttpMessageHandler> HttpMessageHandlerMock => BitbucketClientMock.HttpMessageHandlerMock;

        protected IEnvironmentVariableProvider EnvironmentVariableProvider { get; set; }

        protected override async Task WhenAsync()
        {
            await new TestPipeRunner(BitbucketClientMock, EnvironmentVariableProvider).RunPipeAsync();
        }

        protected void VerifySendAsyncCalls(Times times, Expression<Func<HttpRequestMessage, bool>> requestMatch)
        {
            HttpMessageHandlerMock.Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", times, ItExpr.Is(requestMatch),
                    ItExpr.IsAny<CancellationToken>());
        }
    }
}

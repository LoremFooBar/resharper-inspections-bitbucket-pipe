using Moq;
using Moq.Protected;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BitbucketClientTests
{
    public class When_Sending_A_Report_To_Bitbucket : BitbucketClientSpecificationBase
    {
        protected override async Task WhenAsync()
        {
            await base.WhenAsync();

            await BitbucketClient.CreateReportAsync(new PipelineReport(), Enumerable.Repeat(new Annotation(), 101));
        }

        [Then]
        public void It_Should_Send_Report_And_Annotations_To_Bitbucket()
        {
            HttpMessageHandlerMock
                .Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Exactly(3), ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Then]
        public void It_Should_Serialize_Report_And_Annotations_Using_Snake_Case()
        {
            HttpMessageHandlerMock
                .Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(message =>
                        message.Content.ReadAsStringAsync().Result.Contains("\"report_type\":")),
                    ItExpr.IsAny<CancellationToken>());

            HttpMessageHandlerMock
                .Protected()
                .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Exactly(2), ItExpr.Is<HttpRequestMessage>(
                        message =>
                            message.Content.ReadAsStringAsync().Result.Contains("\"annotation_type\":")),
                    ItExpr.IsAny<CancellationToken>());
        }
    }
}

using Moq;
using Moq.Protected;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BitbucketClientTests;

public class When_Sending_Build_Status_To_Bitbucket_Using_Authentication : BitbucketClientSpecificationBase
{
    protected override async Task WhenAsync()
    {
        await base.WhenAsync();

        await BitbucketClient.CreateBuildStatusAsync(new PipelineReport
        {
            Result = Result.Passed,
            TotalIssues = 0,
        });
    }

    [Then]
    public void It_Should_Send_Build_Status_To_Bitbucket()
    {
        HttpMessageHandlerMock
            .Protected()
            .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Exactly(1), ItExpr.Is<HttpRequestMessage>(
                    message =>
                        message.Content.ReadAsStringAsync().Result.Contains("\"key\":")),
                ItExpr.IsAny<CancellationToken>());
    }
}

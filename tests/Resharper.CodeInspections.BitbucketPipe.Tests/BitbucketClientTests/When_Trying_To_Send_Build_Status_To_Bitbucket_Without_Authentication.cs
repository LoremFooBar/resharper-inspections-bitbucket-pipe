using Moq;
using Moq.Protected;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BitbucketClientTests;

public class When_Trying_To_Send_Build_Status_To_Bitbucket_Without_Authentication : BitbucketClientSpecificationBase
{
    protected override bool UseAuthentication => false;

    protected override async Task WhenAsync()
    {
        await base.WhenAsync();

        await BitbucketClient.CreateBuildStatusAsync(new PipelineReport());
    }

    [Then]
    public void It_Should_Not_Make_A_Request_To_Bitbucket()
    {
        HttpMessageHandlerMock
            .Protected()
            .Verify<Task<HttpResponseMessage>>("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
    }
}

using Moq;
using Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BitbucketClientTests;

public class BitbucketClientSpecificationBase : SpecificationBase
{
    private BitbucketClientSimpleMock BitbucketClientMock { get; set; }
    protected BitbucketClient BitbucketClient => BitbucketClientMock.BitbucketClient;
    protected Mock<HttpMessageHandler> HttpMessageHandlerMock => BitbucketClientMock.HttpMessageHandlerMock;
    protected virtual bool UseAuthentication => true;
    private static bool CreateBuildStatus => true;

    protected override void Given()
    {
        base.Given();

        var environmentInfo = new BitbucketEnvironmentInfo
            { Workspace = "workspace", RepoSlug = "repo-slug", CommitHash = "f46f058a160a42c68e4b30ee4598cbfc" };
        BitbucketClientMock = new BitbucketClientSimpleMock(UseAuthentication, CreateBuildStatus, environmentInfo);
    }
}

using Moq;
using Moq.Contrib.HttpClient;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests;

public class When_Running_Pipe_Without_Credentials : PipeRunnerSpecificationBase
{
    protected override void Given()
    {
        base.Given();

        var environmentVariableProviderMock =
            new EnvironmentVariableProviderMock(TestData.EmptyReportFilePath, new Dictionary<string, string>
            {
                ["CREATE_BUILD_STATUS"] = "true",
            });

        TestPipeRunner = new TestPipeRunner(environmentVariableProviderMock.Object, MessageHandlerMock);
    }

    [Then]
    public void It_Should_Not_Send_Build_Status_To_Bitbucket()
    {
        MessageHandlerMock.VerifyRequest(
            request => request.RequestUri!.PathAndQuery.EndsWith("statuses/build", StringComparison.Ordinal),
            Times.Never());
    }
}

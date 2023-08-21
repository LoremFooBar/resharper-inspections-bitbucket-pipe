using Moq;
using Moq.Contrib.HttpClient;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests;

public class When_Running_Pipe_Against_Empty_Report : PipeRunnerSpecificationBase
{
    private ExitCode _exitCode;

    protected override void Given()
    {
        base.Given();

        var environmentVariableProviderMock = new EnvironmentVariableProviderMock(TestData.EmptyReportFilePath,
            new Dictionary<string, string>
            {
                ["BITBUCKET_USERNAME"] = "user",
                ["BITBUCKET_APP_PASSWORD"] = "password",
                ["CREATE_BUILD_STATUS"] = "true",
            });

        TestPipeRunner = new TestPipeRunner(environmentVariableProviderMock.Object, MessageHandlerMock);
    }

    protected override async Task WhenAsync()
    {
        await base.WhenAsync();

        _exitCode = await TestPipeRunner.RunPipeAsync();
    }

    [Then]
    public void It_Should_Send_Report_To_Bitbucket()
    {
        MessageHandlerMock.VerifyRequest(request =>
            request.RequestUri!.PathAndQuery.EndsWith("reports/resharper-inspections", StringComparison.Ordinal) &&
            request.Method == HttpMethod.Put, Times.Once());
    }

    [Then]
    public void It_Should_Not_Send_Annotations_To_Bitbucket()
    {
        MessageHandlerMock.VerifyRequest(
            request => request.RequestUri!.PathAndQuery.Contains("annotations"),
            Times.Never());
    }

    [Then]
    public void It_Should_Send_Build_Status_To_Bitbucket()
    {
        MessageHandlerMock.VerifyRequest(
            request => request.RequestUri!.PathAndQuery.Contains("statuses/build"),
            Times.Once());
    }

    [Then]
    public void It_Should_Exit_With_Code_0()
    {
        _exitCode.Code.Should().Be(0);
    }
}

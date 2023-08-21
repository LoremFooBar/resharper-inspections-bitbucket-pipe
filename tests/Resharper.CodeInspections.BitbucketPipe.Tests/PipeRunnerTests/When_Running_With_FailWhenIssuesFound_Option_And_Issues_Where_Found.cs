using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests;

public class When_Running_With_FailWhenIssuesFound_Option_And_Issues_Where_Found : PipeRunnerSpecificationBase
{
    private ExitCode _exitCode;

    protected override void Given()
    {
        base.Given();

        var environmentVariableProviderMock = new EnvironmentVariableProviderMock(TestData.NonEmptyReportFilePath,
            new Dictionary<string, string>
            {
                ["FAIL_WHEN_ISSUES_FOUND"] = "TRUE",
            });

        TestPipeRunner = new TestPipeRunner(environmentVariableProviderMock.Object, MessageHandlerMock);
    }

    protected override async Task WhenAsync()
    {
        await base.WhenAsync();

        _exitCode = await TestPipeRunner.RunPipeAsync();
    }

    [Then]
    public void It_Should_Return_Exit_Code_15()
    {
        _exitCode.Code.Should().Be(15);
    }
}

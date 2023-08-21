using System.Net.Http.Json;
using Moq;
using Moq.Contrib.HttpClient;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests;

public class
    When_Running_Pipe_With_Option_To_Filter_Issues_By_Diff : PipeRunnerSpecificationBase
{
    protected override void Given()
    {
        base.Given();

        MessageHandlerMock
            .SetupRequest(request =>
                request.Method == HttpMethod.Get && request.RequestUri!.PathAndQuery.Contains("/diff"))
            .ReturnsResponse(TestData.DiffText, "text/plain");

        var environmentVariableProviderMock = new EnvironmentVariableProviderMock(
            TestData.NonEmptyReportForDiffFilePath,
            new Dictionary<string, string>
            {
                ["INCLUDE_ONLY_ISSUES_IN_DIFF"] = "true",
            });

        TestPipeRunner = new TestPipeRunner(environmentVariableProviderMock.Object, MessageHandlerMock);
    }

    protected override async Task WhenAsync()
    {
        await base.WhenAsync();

        await TestPipeRunner.RunPipeAsync();
    }

    [Then]
    public void It_Should_Send_Report_To_Bitbucket()
    {
        MessageHandlerMock.VerifyRequest(request =>
                request.RequestUri!.PathAndQuery.EndsWith("/reports/resharper-inspections", StringComparison.Ordinal) &&
                request.Method == HttpMethod.Put,
            Times.Once());
    }

    [Then]
    public void It_Should_Send_Three_Annotations_To_Bitbucket()
    {
        MessageHandlerMock.VerifyRequest(async request =>
        {
            bool isAnnotationsRequest =
                request.RequestUri!.PathAndQuery.EndsWith("/annotations", StringComparison.Ordinal) &&
                request.Method == HttpMethod.Post;

            if (!isAnnotationsRequest) return false;

            var annotations = await request.Content!.ReadFromJsonAsync<List<Annotation>>();

            return annotations?.Count == 3;
        }, Times.Once());
    }
}

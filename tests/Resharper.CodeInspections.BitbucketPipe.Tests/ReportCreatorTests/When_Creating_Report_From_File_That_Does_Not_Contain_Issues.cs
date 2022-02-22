using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.ModelCreators;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.ReportCreatorTests;

public class When_Creating_Report_From_File_That_Does_Not_Contain_Issues : SpecificationBase
{
    private SimpleReport _report;
    private ReSharperReportCreator _reSharperReportCreator;

    protected override void Given()
    {
        base.Given();

        var environmentInfo = new BitbucketEnvironmentInfo
        {
            Workspace = "workspace",
            RepoSlug = "repo-slug",
            CommitHash = "f46f058a160a42c68e4b30ee4598cbfc",
        };

        var bitbucketClientSimpleMock = new BitbucketClientSimpleMock(true, true, environmentInfo);
        var pipeOptions = Mock.Of<IOptions<PipeOptions>>(options => options.Value.IncludeOnlyIssuesInDiff == false);
        _reSharperReportCreator = new ReSharperReportCreator(pipeOptions, bitbucketClientSimpleMock.BitbucketClient,
            NullLogger<ReSharperReportCreator>.Instance);
    }

    protected override async Task WhenAsync()
    {
        _report = await _reSharperReportCreator.CreateFromFileAsync(TestData.EmptyReportFilePath);
    }

    [Then]
    public void It_Should_Create_Report_Without_Issues()
    {
        _report.HasAnyIssues.Should().BeFalse();
    }
}

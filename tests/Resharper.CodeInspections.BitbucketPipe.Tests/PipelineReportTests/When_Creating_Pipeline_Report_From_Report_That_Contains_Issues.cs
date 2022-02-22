using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.ModelCreators;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipelineReportTests;

public class When_Creating_Pipeline_Report_From_Report_That_Contains_Issues : SpecificationBase
{
    private PipelineReport _pipelineReport;
    private SimpleReport _report;

    protected override async Task GivenAsync()
    {
        await base.GivenAsync();

        var environmentInfo = new BitbucketEnvironmentInfo
        {
            Workspace = "workspace",
            RepoSlug = "repo-slug",
            CommitHash = "f46f058a160a42c68e4b30ee4598cbfc",
        };

        var bitbucketClientSimpleMock = new BitbucketClientSimpleMock(true, true, environmentInfo);
        var pipeOptions = new OptionsWrapper<PipeOptions>(new PipeOptions());
        var reSharperReportCreator = new ReSharperReportCreator(pipeOptions, bitbucketClientSimpleMock.BitbucketClient,
            NullLogger<ReSharperReportCreator>.Instance);

        _report = await reSharperReportCreator.CreateFromFileAsync(TestData.NonEmptyReportFilePath);
    }

    protected override void When()
    {
        _pipelineReport = PipelineReport.CreateFromIssuesReport(_report);
    }

    [Then]
    public void It_Should_Create_Pipeline_Report_That_Contains_Issues()
    {
        _pipelineReport.TotalIssues.Should().Be(4);
        _pipelineReport.Result.Should().Be(Result.Failed);
        _pipelineReport.Details.Should().ContainAll("resharper-inspections-bitbucket-pipe", "4");
    }
}

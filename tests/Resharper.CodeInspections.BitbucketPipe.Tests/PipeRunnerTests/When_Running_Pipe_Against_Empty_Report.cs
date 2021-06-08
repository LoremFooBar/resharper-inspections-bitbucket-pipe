using System.Net.Http;
using Moq;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests
{
    public class When_Running_Pipe_Against_Empty_Report : PipeRunnerSpecificationBase
    {
        protected override void Given()
        {
            base.Given();

            // //EnvironmentSetup.SetupEnvironment(TestUtils.GetEmptyReportFilePath());
            // var environment = new Dictionary<string, string>
            // {
            //     ["BITBUCKET_WORKSPACE"] = "workspace",
            //     ["BITBUCKET_REPO_SLUG"] = "repo-slug",
            //     ["BITBUCKET_COMMIT"] = "f46f058a160a42c68e4b30ee4598cbfc",
            //     ["INSPECTIONS_XML_PATH"] = TestUtils.GetEmptyReportFilePath()
            // };
            // var envMock = new Mock<IEnvironmentVariableProvider>();
            // envMock.Setup(_ => _.GetEnvironmentVariable(It.IsAny<string>()))
            //     .Returns((string varName) => environment[varName]);
            //
            var environmentInfo = new BitbucketEnvironmentInfo
            {
                Workspace = "workspace",
                RepoSlug = "repo-slug",
                CommitHash = "f46f058a160a42c68e4b30ee4598cbfc"
            };

            BitbucketClientMock = new BitbucketClientMock(true, true, environmentInfo);

            EnvironmentVariableProvider =
                new EnvironmentVariableProviderMock(ExampleReports.GetEmptyReportFilePath()).Object;
        }

        [Then]
        public void It_Should_Send_Report_To_Bitbucket()
        {
            VerifySendAsyncCalls(Times.Once(), request =>
                request.RequestUri.PathAndQuery.EndsWith("reports/resharper-inspections") &&
                request.Method == HttpMethod.Put);
        }

        [Then]
        public void It_Should_Not_Send_Annotations_To_Bitbucket()
        {
            VerifySendAsyncCalls(Times.Never(), request => request.RequestUri.PathAndQuery.Contains("annotations"));
        }

        [Then]
        public void It_Should_Send_Build_Status_To_Bitbucket()
        {
            VerifySendAsyncCalls(Times.Once(), request => request.RequestUri.PathAndQuery.Contains("statuses/build"));
        }
    }
}

using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests
{
    public class When_Running_Pipe_Against_Report_That_Contains_Issues : PipeRunnerSpecificationBase
    {
        protected override void Given()
        {
            base.Given();

            EnvironmentSetup.SetupEnvironment(TestUtils.GetNonEmptyReportFilePath());
            BitbucketClientMock = new BitbucketClientMock(true, true);
        }

        protected override async Task WhenAsync()
        {
            await new TestPipeRunner(BitbucketClientMock).RunPipeAsync();
        }

        [Then]
        public void It_Should_Send_Report_To_Bitbucket()
        {
            VerifySendAsyncCalls(Times.Once(),
                request => request.RequestUri.PathAndQuery.EndsWith("/reports/resharper-inspections") &&
                           request.Method == HttpMethod.Put);
        }

        [Then]
        public void It_Should_Send_Annotations_To_Bitbucket()
        {
            VerifySendAsyncCalls(Times.Once(),
                request => request.RequestUri.PathAndQuery.EndsWith("/annotations") &&
                           request.Method == HttpMethod.Post);
        }

        [Then]
        public void It_Should_Send_Build_Status_To_Bitbucket()
        {
            VerifySendAsyncCalls(Times.Once(), request => request.RequestUri.PathAndQuery.EndsWith("statuses/build"));
        }
    }
}

using System.Threading.Tasks;
using Moq;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests
{
    public class When_Running_Pipe_Without_Credentials : PipeRunnerSpecificationBase
    {
        protected override void Given()
        {
            base.Given();

            EnvironmentSetup.SetupEnvironment(TestUtils.GetEmptyReportFilePath());
            BitbucketClientMock = new BitbucketClientMock(false, true);
        }

        protected override async Task WhenAsync()
        {
            await new TestPipeRunner(BitbucketClientMock).RunPipeAsync();
        }

        [Then]
        public void It_Should_Not_Send_Build_Status_To_Bitbucket()
        {
            VerifySendAsyncCalls(Times.Never(), request => request.RequestUri.PathAndQuery.EndsWith("statuses/build"));
        }
    }
}

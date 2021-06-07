using System.Net.Http;
using Moq;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.BitbucketClientTests
{
    public class BitbucketClientSpecificationBase : SpecificationBase
    {
        private BitbucketClientMock BitbucketClientMock { get; set; }
        protected BitbucketClient BitbucketClient => BitbucketClientMock.BitbucketClient;
        protected Mock<HttpMessageHandler> HttpMessageHandlerMock => BitbucketClientMock.HttpMessageHandlerMock;
        protected virtual bool UseAuthentication => true;
        protected virtual bool CreateBuildStatus => true;

        protected override void Given()
        {
            base.Given();

            EnvironmentSetup.SetupEnvironment("inspect.xml");
            BitbucketClientMock = new BitbucketClientMock(UseAuthentication, CreateBuildStatus);
        }
    }
}

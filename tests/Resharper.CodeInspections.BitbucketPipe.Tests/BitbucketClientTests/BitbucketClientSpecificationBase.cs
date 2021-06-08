using System.Net.Http;
using Moq;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;
using Resharper.CodeInspections.BitbucketPipe.Utils;

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

            // //EnvironmentSetup.SetupEnvironment("inspect.xml");
            // var environment = new Dictionary<string, string>
            // {
            //     ["BITBUCKET_WORKSPACE"] = "workspace",
            //     ["BITBUCKET_REPO_SLUG"] = "repo-slug",
            //     ["BITBUCKET_COMMIT"] = "f46f058a160a42c68e4b30ee4598cbfc",
            //     ["INSPECTIONS_XML_PATH"] = "inspect.xml"
            // };
            // var envMock = new Mock<IEnvironmentVariableProvider>();
            // envMock.Setup(_ => _.GetEnvironmentVariable(It.IsAny<string>()))
            //     .Returns((string varName) => environment[varName]);
            var environmentInfo = new BitbucketEnvironmentInfo
                {Workspace = "workspace", RepoSlug = "repo-slug", CommitHash = "f46f058a160a42c68e4b30ee4598cbfc"};
            BitbucketClientMock = new BitbucketClientMock(UseAuthentication, CreateBuildStatus, environmentInfo);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests
{
    public class TestPipeRunner : PipeRunner
    {
        private readonly BitbucketClientMock _bitbucketClientMock;

        public TestPipeRunner(BitbucketClientMock bitbucketClientMock,
            IEnvironmentVariableProvider environmentVariableProvider) : base(environmentVariableProvider) =>
            _bitbucketClientMock = bitbucketClientMock;

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            // add mock BitbucketClient
            var bitbucketClientService =
                services.FirstOrDefault(service => service.ServiceType == typeof(BitbucketClient));
            services.Remove(bitbucketClientService);
            services.AddSingleton(_bitbucketClientMock.BitbucketClient);

            var environmentVariableProviderService =
                services.FirstOrDefault(service => service.ServiceType == typeof(IEnvironmentVariableProvider));
            services.Remove(environmentVariableProviderService);
            var environment = new Dictionary<string, string>
            {
                ["INSPECTIONS_XML_PATH"] = "inspect.xml"
            };
            var envMock = new Mock<IEnvironmentVariableProvider>();
            envMock.Setup(_ => _.GetEnvironmentVariable(It.IsAny<string>()))
                .Returns((string varName) => environment[varName]);

            services.AddSingleton(envMock.Object);
        }
    }
}

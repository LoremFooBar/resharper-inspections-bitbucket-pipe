using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests
{
    public class TestPipeRunner : PipeRunner
    {
        private readonly BitbucketClientMock _bitbucketClientMock;

        public TestPipeRunner(BitbucketClientMock bitbucketClientMock) => _bitbucketClientMock = bitbucketClientMock;

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            var serviceDescriptor = services.FirstOrDefault(service => service.ServiceType == typeof(BitbucketClient));
            services.Remove(serviceDescriptor);
            services.AddSingleton(_bitbucketClientMock.BitbucketClient);
        }
    }
}

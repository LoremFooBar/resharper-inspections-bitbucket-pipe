using System;
using System.Linq;
using System.Net.Http;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests
{
    public class TestPipeRunner : PipeRunner
    {
        private Action<IServiceCollection> _configureTestServices;

        public TestPipeRunner([NotNull] IEnvironmentVariableProvider environmentVariableProvider) : base(
            environmentVariableProvider)
        {
        }

        public TestPipeRunner([NotNull] IEnvironmentVariableProvider environmentVariableProvider,
            [NotNull] Mock<HttpMessageHandler> bitbucketMessageHandler) : this(environmentVariableProvider)
        {
            _configureTestServices = services =>
            {
                var bitbucketClientDescriptor =
                    services.First(descriptor => descriptor.ServiceType == typeof(BitbucketClient));
                services.Remove(bitbucketClientDescriptor);

                services
                    .AddHttpClient<BitbucketClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => bitbucketMessageHandler.Object);
            };
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            _configureTestServices?.Invoke(services);
        }

        public void ConfigureTestServices(Action<IServiceCollection> configureTestServices) =>
            _configureTestServices = configureTestServices;
    }
}

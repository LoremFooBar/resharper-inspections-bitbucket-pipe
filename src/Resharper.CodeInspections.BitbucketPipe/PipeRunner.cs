using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.ModelCreators;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Utils;
using Serilog;

namespace Resharper.CodeInspections.BitbucketPipe
{
    public class PipeRunner
    {
        private readonly IEnvironmentVariableProvider _environmentVariableProvider;
        private readonly PipeEnvironment _pipeEnvironment;

        public PipeRunner(IEnvironmentVariableProvider environmentVariableProvider)
        {
            _environmentVariableProvider = environmentVariableProvider;
            _pipeEnvironment = new PipeEnvironment(_environmentVariableProvider);
        }

        public async Task RunPipeAsync()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            var pipeOptions = serviceProvider.GetRequiredService<IOptions<PipeOptions>>();
            var annotationsCreator = serviceProvider.GetRequiredService<AnnotationsCreator>();
            var reportCreator = serviceProvider.GetRequiredService<ReSharperReportCreator>();
            var issuesReport = await reportCreator.CreateFromFileAsync(pipeOptions.Value.InspectionsXmlPathOrPattern);
            var pipelineReport = PipelineReport.CreateFromIssuesReport(issuesReport);
            var annotations = annotationsCreator.CreateAnnotationsFromIssuesReport(issuesReport);

            var bitbucketClient = serviceProvider.GetRequiredService<BitbucketClient>();
            await bitbucketClient.CreateReportAsync(pipelineReport, annotations);
            await bitbucketClient.CreateBuildStatusAsync(pipelineReport);
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            var authOptions = new BitbucketAuthenticationOptions
            {
                Username = _environmentVariableProvider.GetEnvironmentVariable("BITBUCKET_USERNAME"),
                AppPassword = _environmentVariableProvider.GetEnvironmentVariable("BITBUCKET_APP_PASSWORD")
            };

            bool createBuildStatus =
                _environmentVariableProvider.GetEnvironmentVariableOrDefault("CREATE_BUILD_STATUS", "true")
                    .Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);

            string inspectionsXmlPathOrPattern =
                _environmentVariableProvider.GetRequiredEnvironmentVariable("INSPECTIONS_XML_PATH");
            Log.Debug("INSPECTIONS_XML_PATH={XmlPath}", inspectionsXmlPathOrPattern);

            bool onlyIssuesInDiff =
                _environmentVariableProvider.GetEnvironmentVariableOrDefault("INCLUDE_ONLY_ISSUES_IN_DIFF", "false")
                    .Equals("true", StringComparison.OrdinalIgnoreCase);

            var pipeOptions = new PipeOptions
            {
                CreateBuildStatus = createBuildStatus,
                InspectionsXmlPathOrPattern = inspectionsXmlPathOrPattern,
                IncludeOnlyIssuesInDiff = onlyIssuesInDiff
            };

            SetupBitbucketClient(services, authOptions);

            services
                .AddLogging(builder => builder.AddSerilog())
                // todo try to replace `Configure` with `AddOptions` and `AddSingleton`
                .Configure<BitbucketAuthenticationOptions>(options =>
                {
                    options.Username = authOptions.Username;
                    options.AppPassword = authOptions.AppPassword;
                })
                .Configure<PipeOptions>(options => {
                    options.CreateBuildStatus = pipeOptions.CreateBuildStatus;
                    options.InspectionsXmlPathOrPattern = pipeOptions.InspectionsXmlPathOrPattern;
                    options.IncludeOnlyIssuesInDiff = pipeOptions.IncludeOnlyIssuesInDiff;
                })
                .AddSingleton(_environmentVariableProvider)
                .AddSingleton(_pipeEnvironment)
                .AddSingleton<BitbucketEnvironmentInfo>()
                .AddSingleton<AnnotationsCreator>()
                .AddSingleton<ReSharperReportCreator>();
        }

        private void SetupBitbucketClient(IServiceCollection services, BitbucketAuthenticationOptions authOptions)
        {
            var httpClientBuilder = services.AddHttpClient<BitbucketClient>();

            if (authOptions.UseAuthentication) {
                Log.Debug("Authenticating using app password");
                httpClientBuilder.ConfigureHttpClient(client =>
                    client.SetBasicAuthentication(authOptions.Username, authOptions.AppPassword));
            }
            else if (!_pipeEnvironment.IsDevelopment) {
                // set proxy for pipe when running in pipelines
                const string proxyUrl = "http://host.docker.internal:29418";

                Log.Debug("Using proxy {Proxy}", proxyUrl);
                Log.Information("Not using authentication - can't create build status");
                httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {Proxy = new WebProxy(proxyUrl)});
            }
            else {
                Log.Error("Could not authenticate to Bitbucket!");
            }
        }
    }
}

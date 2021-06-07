using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Utils;
using Serilog;

namespace Resharper.CodeInspections.BitbucketPipe
{
    public class PipeRunner
    {
        public async Task RunPipeAsync()
        {
            string filePathOrPattern = EnvironmentUtils.GetRequiredEnvironmentVariable("INSPECTIONS_XML_PATH");
            Log.Debug("INSPECTIONS_XML_PATH={XmlPath}", filePathOrPattern);

            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            var issuesReport = await Report.CreateFromFileAsync(filePathOrPattern);
            var pipelineReport = PipelineReport.CreateFromIssuesReport(issuesReport);
            var annotations = Annotation.CreateFromIssuesReport(issuesReport);

            var bitbucketClient = serviceProvider.GetRequiredService<BitbucketClient>();
            await bitbucketClient.CreateReportAsync(pipelineReport, annotations);
            await bitbucketClient.CreateBuildStatusAsync(pipelineReport);
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            var authOptions = new BitbucketAuthenticationOptions
            {
                Username = Environment.GetEnvironmentVariable("BITBUCKET_USERNAME"),
                AppPassword = Environment.GetEnvironmentVariable("BITBUCKET_APP_PASSWORD")
            };

            var pipeOptions = new PipeOptions
            {
                CreateBuildStatus = EnvironmentUtils.GetEnvironmentVariableOrDefault("CREATE_BUILD_STATUS", "true")
                    .Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase)
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
                .Configure<PipeOptions>(options => options.CreateBuildStatus = pipeOptions.CreateBuildStatus);
        }

        private static void SetupBitbucketClient(IServiceCollection services, BitbucketAuthenticationOptions authOptions)
        {
            var httpClientBuilder = services.AddHttpClient<BitbucketClient>();

            if (authOptions.UseAuthentication) {
                Log.Debug("Authenticating using app password");
                httpClientBuilder.ConfigureHttpClient(client =>
                    client.SetBasicAuthentication(authOptions.Username, authOptions.AppPassword));
            }
            else if (!EnvironmentUtils.IsDevelopment) {
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

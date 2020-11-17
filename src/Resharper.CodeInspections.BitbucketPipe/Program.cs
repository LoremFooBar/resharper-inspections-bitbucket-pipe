using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Serilog;

namespace Resharper.CodeInspections.BitbucketPipe
{
    internal static class Program
    {
        [PublicAPI]
        private static async Task Main(string[] args)
        {
            bool isDebug =
                Environment.GetEnvironmentVariable("DEBUG")?.Equals("true", StringComparison.OrdinalIgnoreCase)
                ?? false;
            Log.Logger = LoggerInitializer.CreateLogger(isDebug);

            Log.Logger.Debug("DEBUG={isDebug}", isDebug);

            string filePathOrPattern = Utils.GetRequiredEnvironmentVariable("INSPECTIONS_XML_PATH");
            Log.Logger.Debug("INSPECTIONS_XML_PATH={isDebug}", filePathOrPattern);

            var serviceProvider = ConfigureServices();

            var issuesReport = await CreateIssuesReport(filePathOrPattern);

            var pipelineReport = PipelineReport.CreateFromIssuesReport(issuesReport);
            var annotations = Annotation.CreateFromIssuesReport(issuesReport);

            var bitbucketClient = serviceProvider.GetRequiredService<IBitbucketClient>();
            await bitbucketClient.CreateReportAsync(pipelineReport, annotations);
        }

        private static async Task<Report> CreateIssuesReport(string filePathOrPattern)
        {
            var currentDir = new DirectoryInfo(Environment.CurrentDirectory);
            Log.Logger.Debug("Current directory: {directory}", currentDir);

            var reportFile = currentDir.GetFiles(filePathOrPattern).FirstOrDefault();
            if (reportFile == null) {
                throw new FileNotFoundException($"could not find report file in directory {currentDir.FullName}",
                    filePathOrPattern);
            }

            Log.Logger.Debug("Found inspections file: {file}", reportFile.FullName);

            await using var fileStream = reportFile.OpenRead();
            Log.Logger.Debug("Deserializing report...");
            var issuesReport = (Report) new XmlSerializer(typeof(Report)).Deserialize(fileStream);
            Log.Logger.Debug("Report deserialized successfully");
            return issuesReport;
        }

        private static ServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            var httpClientBuilder = serviceCollection.AddHttpClient<IBitbucketClient, BitbucketClient>();
            if (!Utils.IsDevelopment) {
                // set proxy for pipe when running in pipelines
                const string proxyUrl = "http://host.docker.internal:29418";

                Log.Logger.Debug("Using proxy {proxy}", proxyUrl);
                httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {Proxy = new WebProxy(proxyUrl)});
            }

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, false)
                .AddJsonFile($"appsettings.{Utils.EnvironmentName}.json", true, false);
            var configurationRoot = configurationBuilder.Build();

            serviceCollection
                .AddSingleton<IConfiguration>(configurationRoot)
                .Configure<BitbucketAuthorizationOptions>(
                    configurationRoot.GetSection(BitbucketAuthorizationOptions.BitbucketAuthorization))
                .AddLogging(builder => builder.AddSerilog());

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}

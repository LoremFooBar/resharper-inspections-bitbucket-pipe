using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient;
using Resharper.CodeInspections.BitbucketPipe.Configuration;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.ModelCreators;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Utils;
using Serilog;

namespace Resharper.CodeInspections.BitbucketPipe;

public class PipeRunner
{
    private readonly IEnvironmentVariableProvider _environmentVariableProvider;
    private readonly PipeEnvironment _pipeEnvironment;

    public PipeRunner(IEnvironmentVariableProvider environmentVariableProvider)
    {
        _environmentVariableProvider = environmentVariableProvider;
        _pipeEnvironment = new PipeEnvironment(_environmentVariableProvider);
    }

    public async Task<ExitCode> RunPipeAsync()
    {
        Log.Logger = LoggerInitializer.CreateLogger(_pipeEnvironment.IsDebugMode);
        Log.Debug("DEBUG={IsDebug}", _pipeEnvironment.IsDebugMode);

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var services = serviceCollection.BuildServiceProvider();

        var pipeOptions = services.GetRequiredService<IOptions<PipeOptions>>();
        var annotationsCreator = services.GetRequiredService<AnnotationsCreator>();
        var reportCreator = services.GetRequiredService<ReSharperReportCreator>();

        var issuesReport = await reportCreator.CreateFromFileAsync(pipeOptions.Value.InspectionsXmlPathOrPattern);
        var pipelineReport = PipelineReport.CreateFromIssuesReport(issuesReport);
        var annotations = annotationsCreator.CreateAnnotationsFromIssuesReport(issuesReport);

        var bitbucketClient = services.GetRequiredService<BitbucketClient>();
        await bitbucketClient.CreateReportAsync(pipelineReport, annotations);
        await bitbucketClient.CreateBuildStatusAsync(pipelineReport);

        return pipeOptions.Value.FailWhenIssuesFound && issuesReport.TotalIssues > 0
            ? ExitCode.IssuesFound
            : ExitCode.Success;
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.Add(new PipeConfigurationSource(_environmentVariableProvider));
        var config = configurationBuilder.Build();

        SetupBitbucketClient(services,
            config.GetRequiredSection(BitbucketAuthenticationOptions.SectionName)
                .Get<BitbucketAuthenticationOptions>() ??
            throw new Exception("Unexpected configuration error occured"));

        services
            .AddLogging(builder => builder.AddSerilog())
            .Configure<BitbucketAuthenticationOptions>(
                config.GetRequiredSection(BitbucketAuthenticationOptions.SectionName))
            .Configure<PipeOptions>(config.GetRequiredSection(PipeOptions.SectionName))
            .AddSingleton(_environmentVariableProvider)
            .AddSingleton(_pipeEnvironment)
            .AddSingleton<BitbucketEnvironmentInfo>()
            .AddSingleton<AnnotationsCreator>()
            .AddSingleton<ReSharperReportCreator>();
    }

    private static void SetupBitbucketClient(IServiceCollection services, BitbucketAuthenticationOptions authOptions) =>
        services.AddHttpClient<BitbucketClient>().ConfigureHttpClient(client =>
            client.SetBasicAuthentication(authOptions.Email, authOptions.ApiToken));
}

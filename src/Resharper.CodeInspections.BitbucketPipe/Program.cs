using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Model.ReSharper;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Serilog;
using static Resharper.CodeInspections.BitbucketPipe.LoggerInitializer;

namespace Resharper.CodeInspections.BitbucketPipe
{
    internal static class Program
    {
        // ReSharper disable once InconsistentNaming
        private static async Task Main()
        {
            bool isDebug =
                Environment.GetEnvironmentVariable("DEBUG")?.Equals("true", StringComparison.OrdinalIgnoreCase)
                ?? false;
            Log.Logger = CreateLogger(isDebug);

            Log.Debug("DEBUG={IsDebug}", isDebug);

            string filePathOrPattern = Utils.GetRequiredEnvironmentVariable("INSPECTIONS_XML_PATH");
            Log.Debug("INSPECTIONS_XML_PATH={XmlPath}", filePathOrPattern);

            var serviceProvider = await ConfigureServicesAsync();

            var issuesReport = await Report.CreateFromFileAsync(filePathOrPattern);
            var pipelineReport = PipelineReport.CreateFromIssuesReport(issuesReport);
            var annotations = Annotation.CreateFromIssuesReport(issuesReport);

            var bitbucketClient = serviceProvider.GetRequiredService<BitbucketClient>();
            await bitbucketClient.CreateReportAsync(pipelineReport, annotations);
            await bitbucketClient.CreateBuildStatusAsync(pipelineReport);
        }

        private static async Task<ServiceProvider> ConfigureServicesAsync()
        {
            var serviceCollection = new ServiceCollection();
            var httpClientBuilder = serviceCollection.AddHttpClient<BitbucketClient>();

            var authOptions = new BitbucketAuthenticationOptions
            {
                OAuthKey = Environment.GetEnvironmentVariable("BITBUCKET_OAUTH_KEY"),
                OAuthSecret = Environment.GetEnvironmentVariable("BITBUCKET_OAUTH_SECRET")
            };

            if (authOptions.UseOAuth) {
                Log.Debug("Authenticating using OAuth");
                string accessToken = await GetAccessTokenAsync(authOptions);
                httpClientBuilder.ConfigureHttpClient(client =>
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(OidcConstants.AuthenticationSchemes.AuthorizationHeaderBearer,
                            accessToken));
            }
            else if (!Utils.IsDevelopment) {
                // set proxy for pipe when running in pipelines
                const string proxyUrl = "http://host.docker.internal:29418";

                Log.Debug("Using proxy {Proxy}", proxyUrl);
                httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {Proxy = new WebProxy(proxyUrl)});
            }
            else {
                Log.Error("Could not authenticate to Bitbucket!");
            }

            serviceCollection
                .AddLogging(builder => builder.AddSerilog())
                .Configure<BitbucketAuthenticationOptions>(options =>
                {
                    options.OAuthKey = authOptions.OAuthKey;
                    options.OAuthSecret = authOptions.OAuthSecret;
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }

        private static async Task<string> GetAccessTokenAsync(BitbucketAuthenticationOptions options)
        {
            Log.Debug("Getting access token...");

            using var httpClient = new HttpClient();
            var tokenRequest = new ClientCredentialsTokenRequest
            {
                ClientId = options.OAuthKey,
                ClientSecret = options.OAuthSecret,
                Scope = "repository:write",
                Address = "https://bitbucket.org/site/oauth2/access_token"
            };

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest);

            if (!tokenResponse.IsError) {
                Log.Debug("Got access token");
                return tokenResponse.AccessToken;
            }

            Log.Error("Error getting access token: {@Error}",
                new
                {
                    tokenResponse.Error, tokenResponse.ErrorDescription, tokenResponse.ErrorType,
                    tokenResponse.HttpStatusCode
                });

            throw new OAuthException(tokenResponse);
        }
    }
}

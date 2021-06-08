using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CommitStatuses;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe
{
    public class BitbucketClient
    {
        private readonly HttpClient _httpClient;
        private readonly BitbucketEnvironmentInfo _bitbucketEnvironmentInfo;
        private readonly PipeOptions _pipeOptions;
        private readonly BitbucketAuthenticationOptions _authOptions;
        private readonly ILogger<BitbucketClient> _logger;

        public BitbucketClient(HttpClient client, IOptions<BitbucketAuthenticationOptions> authOptions,
            IOptions<PipeOptions> pipeOptions, BitbucketEnvironmentInfo bitbucketEnvironmentInfo,
            ILogger<BitbucketClient> logger)
        {
            _httpClient = client;
            _bitbucketEnvironmentInfo = bitbucketEnvironmentInfo;
            _pipeOptions = pipeOptions.Value;
            _authOptions = authOptions.Value;
            _logger = logger;

            ConfigureHttpClient();

            _logger.LogDebug("Base address: {BaseAddress}", client.BaseAddress);
        }

        private void ConfigureHttpClient()
        {
            string baseUriScheme = _authOptions.UseAuthentication ? "https" : "http";
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.BaseAddress =
                new Uri(
                    $"{baseUriScheme}://api.bitbucket.org/2.0/repositories/" +
                    $"{_bitbucketEnvironmentInfo.Workspace}/{_bitbucketEnvironmentInfo.RepoSlug}/" +
                    $"commit/{_bitbucketEnvironmentInfo.CommitHash}/");
            if (_authOptions.UseAuthentication) {
                _logger.LogDebug("Authenticating using app password");
                _httpClient.SetBasicAuthentication(_authOptions.Username, _authOptions.AppPassword);
            }
        }

        public async Task CreateReportAsync(PipelineReport report, IEnumerable<Annotation> annotations)
        {
            string serializedReport = Serialize(report);

            _logger.LogDebug("Sending request: PUT reports/{ExternalId}", report.ExternalId);
            _logger.LogDebug("Sending report: {Report}", serializedReport);

            var response = await _httpClient.PutAsync($"reports/{HttpUtility.UrlEncode(report.ExternalId)}",
                CreateStringContent(serializedReport));

            await VerifyResponseAsync(response);

            await CreateReportAnnotationsAsync(report, annotations);
        }

        private async Task CreateReportAnnotationsAsync(PipelineReport report, IEnumerable<Annotation> annotations)
        {
            const int maxAnnotations = 1000;
            const int maxAnnotationsPerRequest = 100;
            int numOfAnnotationsUploaded = 0;
            var annotationsList = annotations.ToList(); // avoid multiple enumerations

            _logger.LogDebug("Total annotations: {TotalAnnotations}", annotationsList.Count);

            while (numOfAnnotationsUploaded < annotationsList.Count &&
                   numOfAnnotationsUploaded + maxAnnotationsPerRequest <= maxAnnotations) {
                var annotationsToUpload =
                    annotationsList.Skip(numOfAnnotationsUploaded).Take(maxAnnotationsPerRequest).ToList();

                string serializedAnnotations = Serialize(annotationsToUpload);

                _logger.LogDebug("POSTing {TotalAnnotations} annotation(s), starting with location {AnnotationsStart}",
                    annotationsToUpload.Count.ToString(), numOfAnnotationsUploaded);
                _logger.LogDebug("Annotations in request: {Annotations}", serializedAnnotations);

                var response = await _httpClient.PostAsync(
                    $"reports/{HttpUtility.UrlEncode(report.ExternalId)}/annotations",
                    CreateStringContent(serializedAnnotations));

                await VerifyResponseAsync(response);

                numOfAnnotationsUploaded += annotationsToUpload.Count;
            }
        }

        public async Task CreateBuildStatusAsync(PipelineReport report)
        {
            if (!_pipeOptions.CreateBuildStatus) {
                return;
            }

            if (!_authOptions.UseAuthentication) {
                _logger.LogWarning("Will not create build status because authentication info was not provided");
                return;
            }

            var buildStatus = BuildStatus.CreateFromPipelineReport(report, _bitbucketEnvironmentInfo.Workspace,
                _bitbucketEnvironmentInfo.RepoSlug);
            string serializedBuildStatus = Serialize(buildStatus);

            _logger.LogDebug("POSTing build status: {BuildStatus}", serializedBuildStatus);

            var response = await _httpClient.PostAsync("statuses/build", CreateStringContent(serializedBuildStatus));

            await VerifyResponseAsync(response);
        }

        private static string Serialize(object obj)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
                IgnoreNullValues = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            return JsonSerializer.Serialize(obj, jsonSerializerOptions);
        }

        private static StringContent CreateStringContent(string str) =>
            new StringContent(str, Encoding.Default, "application/json");

        private async Task VerifyResponseAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) {
                string error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error response: {Error}", error);
            }

            response.EnsureSuccessStatusCode();
        }
    }
}

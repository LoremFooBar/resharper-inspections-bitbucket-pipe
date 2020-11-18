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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;
using Resharper.CodeInspections.BitbucketPipe.Options;

namespace Resharper.CodeInspections.BitbucketPipe
{
    public class BitbucketClient : IBitbucketClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BitbucketClient> _logger;

        public BitbucketClient(HttpClient client, IOptions<BitbucketAuthorizationOptions> options,
            ILogger<BitbucketClient> logger)
        {
            _httpClient = client;
            _logger = logger;

            string workspace = Utils.GetRequiredEnvironmentVariable("BITBUCKET_WORKSPACE");
            string repoSlug = Utils.GetRequiredEnvironmentVariable("BITBUCKET_REPO_SLUG");
            string commitHash = Utils.GetRequiredEnvironmentVariable("BITBUCKET_COMMIT");

            if (Utils.IsDevelopment) {
                string? accessToken = options.Value.AccessToken;
                if (!string.IsNullOrEmpty(accessToken)) {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }

            client.BaseAddress =
                new Uri(
                    $"https://api.bitbucket.org/2.0/repositories/{workspace}/{repoSlug}/commit/{commitHash}/");

            _logger.LogDebug("Base address: {baseAddress}", client.BaseAddress);
        }

        public async Task CreateReportAsync(PipelineReport report, IEnumerable<Annotation> annotations)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
                IgnoreNullValues = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string serializedReport = JsonSerializer.Serialize(report, serializerOptions);

            _logger.LogDebug("Sending request: PUT reports/{externalId}", report.ExternalId);
            _logger.LogDebug("Sending report: {report}", serializedReport);

            var createReportResponse = await _httpClient.PutAsync($"reports/{HttpUtility.UrlEncode(report.ExternalId)}",
                new StringContent(serializedReport, Encoding.Default, "application/json"));

            if (!createReportResponse.IsSuccessStatusCode) {
                string error = await createReportResponse.Content.ReadAsStringAsync();
                _logger.LogError("Error response: {error}", error);
            }

            createReportResponse.EnsureSuccessStatusCode();

            const int maxAnnotations = 1000;
            const int maxAnnotationsPerRequest = 100;
            int numOfAnnotationsUploaded = 0;
            var annotationsList = annotations.ToList(); // avoid multiple enumerations

            _logger.LogDebug("Total annotations: {totalAnnotations}", annotationsList.Count);

            while (numOfAnnotationsUploaded < annotationsList.Count &&
                   numOfAnnotationsUploaded + maxAnnotationsPerRequest <= maxAnnotations) {
                var annotationsToUpload =
                    annotationsList.Skip(numOfAnnotationsUploaded).Take(maxAnnotationsPerRequest).ToList();

                string serializedAnnotations = JsonSerializer.Serialize(annotationsToUpload, serializerOptions);

                _logger.LogDebug("POSTing {totalAnnotations} annotation(s), starting with location {annotationsStart}",
                    annotationsToUpload.Count, numOfAnnotationsUploaded);
                _logger.LogDebug("Annotations in request: {annotations}", serializedAnnotations);

                var annotationsResponse = await _httpClient.PostAsync(
                    $"reports/{HttpUtility.UrlEncode(report.ExternalId)}/annotations",
                    new StringContent(serializedAnnotations, Encoding.Default, "application/json"));

                if (!annotationsResponse.IsSuccessStatusCode) {
                    string error = await annotationsResponse.Content.ReadAsStringAsync();
                    _logger.LogError("Error response: {error}", error);
                }

                annotationsResponse.EnsureSuccessStatusCode();

                numOfAnnotationsUploaded += annotationsToUpload.Count;
            }
        }
    }
}

﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient
{
    public partial class BitbucketClient
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
                    $"{_bitbucketEnvironmentInfo.Workspace}/{_bitbucketEnvironmentInfo.RepoSlug}/");
            if (_authOptions.UseAuthentication) {
                _logger.LogDebug("Authenticating using app password");
                _httpClient.SetBasicAuthentication(_authOptions.Username, _authOptions.AppPassword);
            }
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
            new(str, Encoding.Default, "application/json");

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

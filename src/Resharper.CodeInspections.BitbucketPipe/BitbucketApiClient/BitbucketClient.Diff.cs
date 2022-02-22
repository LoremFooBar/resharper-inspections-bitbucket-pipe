using System.Net;
using DiffPatch;
using Microsoft.Extensions.Logging;
using Resharper.CodeInspections.BitbucketPipe.Model.Diff;

namespace Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient;

public partial class BitbucketClient
{
    public async Task<IReadOnlyDictionary<string, AddedLinesInFile>> GetCodeChangesAsync()
    {
        string requestUri = _bitbucketEnvironmentInfo.IsPullRequest
            ? $"pullrequests/{_bitbucketEnvironmentInfo.PullRequestId}/diff"
            : $"diff/{_bitbucketEnvironmentInfo.CommitHash}";

        _logger.LogDebug("GET diff {RequestUri}", requestUri);
        var response = await _httpClient.GetAsync(requestUri);
        string diffStr;

        // this happens due to credentials not being forward on redirect
        // https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler.allowautoredirect?view=net-6.0#remarks
        if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized) {
            var response2 = await _httpClient.GetAsync(response.RequestMessage!.RequestUri);
            await VerifyResponseAsync(response2);
            diffStr = await response2.Content.ReadAsStringAsync();
        }
        else {
            await VerifyResponseAsync(response);
            diffStr = await response.Content.ReadAsStringAsync();
        }

        var fileDiffs = DiffParserHelper.Parse(diffStr);

        var diffDictionary = fileDiffs
            .Where(fd => !fd.Deleted)
            .Select(fd => new
            {
                fd.To,
                LineNumbers = fd.Chunks.SelectMany(chunk =>
                    chunk.Changes.Where(change => change.Add).Select(change => change.Index)).ToList(),
            })
            .GroupBy(x => x.To)
            .ToDictionary(x => x.Key, x =>
                new AddedLinesInFile(x.Key, x.SelectMany(y => y.LineNumbers).ToList()));

        return diffDictionary;
    }
}

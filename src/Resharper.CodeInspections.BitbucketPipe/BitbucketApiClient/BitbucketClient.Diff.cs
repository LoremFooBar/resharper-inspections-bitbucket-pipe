using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiffPatch;
using DiffPatch.Data;
using Microsoft.Extensions.Logging;

namespace Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient
{
    public partial class BitbucketClient
    {
        public async Task<Dictionary<string, List<ChunkRange>>> GetCodeChangesAsync()
        {
            string requestUri = _bitbucketEnvironmentInfo.IsPullRequest
                ? $"pullrequests/{_bitbucketEnvironmentInfo.PullRequestId}/diff"
                : $"diff/{_bitbucketEnvironmentInfo.CommitHash}";

            _logger.LogDebug("GET diff {RequestUri}", requestUri);
            string diffStr = await _httpClient.GetStringAsync(requestUri);

            var fileDiffs = DiffParserHelper.Parse(diffStr);
            var diffDictionary = fileDiffs
                .Where(fd => !fd.Deleted)
                .ToDictionary(
                    fd => fd.To,
                    fd => fd.Chunks.Select(chunk => chunk.RangeInfo.NewRange).ToList());

            return diffDictionary;
        }
    }
}

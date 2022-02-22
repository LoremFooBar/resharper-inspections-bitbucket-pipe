using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Resharper.CodeInspections.BitbucketPipe.BitbucketApiClient;
using Resharper.CodeInspections.BitbucketPipe.Options;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

/// <summary>
/// BitbucketClient mock that returns OK for all requests
/// </summary>
public class BitbucketClientSimpleMock
{
    public BitbucketClientSimpleMock(bool useAuthentication, bool createBuildStatus,
        BitbucketEnvironmentInfo environmentInfo)
    {
        HttpMessageHandlerMock = new Mock<HttpMessageHandler>();
        var httpClient = HttpMessageHandlerMock.CreateClient();

        HttpMessageHandlerMock
            .SetupAnyRequest()
            .ReturnsResponse(HttpStatusCode.OK);

        var authOptionsPoco = useAuthentication
            ? new BitbucketAuthenticationOptions { Username = "user", AppPassword = "pass" }
            : new BitbucketAuthenticationOptions { Username = "", AppPassword = "" };
        var authOptions = new OptionsWrapper<BitbucketAuthenticationOptions>(authOptionsPoco);

        var pipeOptionsPoco = new PipeOptions { CreateBuildStatus = createBuildStatus };
        var pipeOptions = new OptionsWrapper<PipeOptions>(pipeOptionsPoco);

        BitbucketClient =
            new BitbucketClient(httpClient, authOptions, pipeOptions, environmentInfo,
                NullLogger<BitbucketClient>.Instance);
    }

    public BitbucketClient BitbucketClient { get; }
    public Mock<HttpMessageHandler> HttpMessageHandlerMock { get; }
}

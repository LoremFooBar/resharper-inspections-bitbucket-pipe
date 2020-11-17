using System;

namespace Resharper.CodeInspections.BitbucketPipe.Options
{
    [Serializable]
    public class BitbucketAuthorizationOptions
    {
        public const string BitbucketAuthorization = "BitbucketAuthorization";

        public string? AccessToken { get; set; }
    }
}

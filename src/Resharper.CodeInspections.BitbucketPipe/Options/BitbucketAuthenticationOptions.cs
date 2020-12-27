using System;

namespace Resharper.CodeInspections.BitbucketPipe.Options
{
    [Serializable]
    public class BitbucketAuthenticationOptions
    {
        public const string BitbucketAuthorization = "BitbucketAuthentication";

        public string? OAuthKey { get; set; }
        public string? OAuthSecret { get; set; }

        public bool UseOAuth => !string.IsNullOrWhiteSpace(OAuthKey) && !string.IsNullOrWhiteSpace(OAuthSecret);
    }
}

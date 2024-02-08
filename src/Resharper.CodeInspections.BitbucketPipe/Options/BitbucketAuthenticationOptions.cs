using System.Diagnostics.CodeAnalysis;

namespace Resharper.CodeInspections.BitbucketPipe.Options;

[Serializable]
public class BitbucketAuthenticationOptions
{
    public const string SectionName = "BitbucketAuthenticationOptions";

    public string? Username { get; set; }
    public string? AppPassword { get; set; }

    [MemberNotNullWhen(true, nameof(Username), nameof(AppPassword))]
    public bool UseAuthentication => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(AppPassword);
}

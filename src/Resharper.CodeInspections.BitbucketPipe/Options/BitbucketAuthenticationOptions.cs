namespace Resharper.CodeInspections.BitbucketPipe.Options;

[Serializable]
public class BitbucketAuthenticationOptions
{
    public const string SectionName = "BitbucketAuthenticationOptions";

    public required string Email { get; set; }
    public required string ApiToken { get; set; }
}

namespace Resharper.CodeInspections.BitbucketPipe.Options;

public class BitbucketAuthenticationOptions
{
    public string? Username { get; set; }
    public string? AppPassword { get; set; }

    public bool UseAuthentication => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(AppPassword);
}

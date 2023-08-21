namespace Resharper.CodeInspections.BitbucketPipe;

public struct ExitCode
{
    private ExitCode(int code) => Code = code;

    public int Code { get; }

    public static ExitCode Success { get; } = new(0);
    public static ExitCode IssuesFound { get; } = new(15);

    public static implicit operator int(ExitCode exitCode) => exitCode.Code;
}

using JetBrains.Annotations;
using Moq;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

public class EnvironmentVariableProviderMock
{
    private EnvironmentVariableProviderMock([CanBeNull] IReadOnlyDictionary<string, string> environment = null)
    {
        Dictionary<string, string> unionEnv;

        if (environment != null) {
            var defaultEnv = DefaultEnvironment.Where(kv => !environment.ContainsKey(kv.Key));
            unionEnv = environment.Concat(defaultEnv).ToDictionary(kv => kv.Key, kv => kv.Value);
        }
        else
            unionEnv = DefaultEnvironment;

        var envMock = new Mock<IEnvironmentVariableProvider> { CallBase = true };
        envMock.Setup(p => p.GetString(It.IsAny<string>()))
            .Returns((string varName) =>
            {
                unionEnv.TryGetValue(varName, out string val);

                return val;
            });
        Object = envMock.Object;
    }

    public EnvironmentVariableProviderMock(string inspectionsFilePath,
        IReadOnlyDictionary<string, string> environment = null) :
        this(new Dictionary<string, string>(environment ?? new Dictionary<string, string>())
            { ["INSPECTIONS_XML_PATH"] = inspectionsFilePath }) { }

    public IEnvironmentVariableProvider Object { get; }

    private Dictionary<string, string> DefaultEnvironment { get; } = new()
    {
        ["BITBUCKET_WORKSPACE"] = "workspace",
        ["BITBUCKET_REPO_SLUG"] = "repo-slug",
        ["BITBUCKET_COMMIT"] = "f46f058a160a42c68e4b30ee4598cbfc",
        ["INSPECTIONS_XML_PATH"] = "inspect.xml",
        ["BITBUCKET_CLONE_DIR"] = Path.GetTempPath(),
    };
}

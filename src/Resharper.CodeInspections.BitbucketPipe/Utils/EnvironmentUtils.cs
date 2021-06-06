using System;
using static System.Environment;

namespace Resharper.CodeInspections.BitbucketPipe.Utils
{
    internal static class EnvironmentUtils
    {
        public static bool IsDebugMode { get; } =
            GetEnvironmentVariable("DEBUG")?.Equals("true", StringComparison.OrdinalIgnoreCase)
            ?? false;

        public static string GetRequiredEnvironmentVariable(string variableName) =>
            GetEnvironmentVariable(variableName) ??
            throw new RequiredEnvironmentVariableNotFoundException(variableName);

        public static string GetEnvironmentVariableOrDefault(string variableName, string defaultValue) =>
            GetEnvironmentVariable(variableName) ?? defaultValue;

        // ReSharper disable once MemberCanBePrivate.Global
        public static string EnvironmentName { get; } =
            GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "Production";

        // ReSharper disable once UnusedMember.Global
        public static bool IsDevelopment { get; } =
            EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase);
    }
}

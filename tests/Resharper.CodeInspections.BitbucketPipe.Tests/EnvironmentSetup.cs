﻿using System;

namespace Resharper.CodeInspections.BitbucketPipe.Tests
{
    public static class EnvironmentSetup
    {
        public static void SetupEnvironment(string inspectionsXmlPath)
        {
            Environment.SetEnvironmentVariable("INSPECTIONS_XML_PATH", inspectionsXmlPath);

            Environment.SetEnvironmentVariable("BITBUCKET_WORKSPACE", "workspace");
            Environment.SetEnvironmentVariable("BITBUCKET_REPO_SLUG", "repo-slug");
            Environment.SetEnvironmentVariable("BITBUCKET_COMMIT", "f46f058a160a42c68e4b30ee4598cbfc");

        }
    }
}

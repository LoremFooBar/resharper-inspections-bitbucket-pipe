# Bitbucket Pipelines Pipe: ReSharper Inspections Report

Create a report with annotations from a ReSharper inspections XML, and a
corresponding build status with the status of the report.

> **Note:** JetBrains will soon deprecate the XML output format. For SARIF
> format, use
> [loremfoobar/sarif-bitbucket-pipe](https://bitbucket.org/loremfoobar/sarif-bitbucket-pipe)

## YAML Definition

Add the following snippet to the script section of
your `bitbucket-pipelines.yml` file:

```yaml
script:
  - pipe: docker://loremfoobar/resharper-inspections-bitbucket-pipe:1.1.0
    variables:
      INSPECTIONS_XML_PATH: "<string>"
      ACCOUNT_EMAIL: "<string>"
      API_TOKEN: "<string>"
      # CREATE_BUILD_STATUS: "<boolean>" # Optional, default "true"
      # INCLUDE_ONLY_ISSUES_IN_DIFF: "<boolean>" # Optional, default "false"
      # DEBUG: "<boolean>" # Optional
```

## Variables

| Variable                    | Usage                                                                                                                                                                                                                 |
|-----------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| INSPECTIONS_XML_PATH (\*)   | Path to inspections xml file, relative to current directory. You can use patterns that <br/> are supported by [DirectoryInfo.GetFiles](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo.getfiles). |
| ACCOUNT_EMAIL (\*)          | Atlassian account email, required to create build status and to get PR diff.                                                                                                                                          |
| API_TOKEN (\*)              | API token, required to create build status and to get PR diff.                                                                                                                                                        |
| CREATE_BUILD_STATUS         | Whether to create a new build status reflecting the results of the report. Default: `true`.                                                                                                                           |
| FAIL_WHEN_ISSUES_FOUND      | Whether to fail current build step if any issues found. Default: `false`.                                                                                                                                             |
| INCLUDE_ONLY_ISSUES_IN_DIFF | Whether to include only issues found in changes of current PR/commit. Default: `false`.                                                                                                                               |
| DEBUG                       | Turn on extra debug information. Default: `false`.                                                                                                                                                                    |

_(\*) = required variable._

## Prerequisites

### Inspections File

You need to create the inspections XML file before calling the pipe. To create
the inspections XML file, see
[InspectCode Command-Line Tool](https://www.jetbrains.com/help/resharper/InspectCode.html).

### API Token

An API token is required for the following features:

| Feature               | Required scope               |
|-----------------------|------------------------------|
| Create a report       | `read:repository:bitbucket`  |
| Create a build status | `read:repository:bitbucket`  |
| Get commit diff       | `read:repository:bitbucket`  |
| Get PR diff           | `read:pullrequest:bitbucket` |

See Atlassian documentation on how
to [create an API token](https://support.atlassian.com/bitbucket-cloud/docs/create-an-api-token/).

## Examples

Basic example:

```yaml
script:
  - pipe: docker://loremfoobar/resharper-inspections-bitbucket-pipe:1.1.0
    variables:
      INSPECTIONS_XML_PATH: "inspect.xml"
      ACCOUNT_EMAIL: $EMAIL
      API_TOKEN: $API_TOKEN
```

With pattern:

```yaml
script:
  - pipe: docker://loremfoobar/resharper-inspections-bitbucket-pipe:1.1.0
    variables:
      INSPECTIONS_XML_PATH: "src/*/inspect.xml"
      ACCOUNT_EMAIL: $EMAIL
      API_TOKEN: $API_TOKEN
```

With build status creation disabled:

```yaml
script:
  - pipe: docker://loremfoobar/resharper-inspections-bitbucket-pipe:1.1.0
    variables:
      INSPECTIONS_XML_PATH: "inspect.xml"
      ACCOUNT_EMAIL: $EMAIL
      API_TOKEN: $API_TOKEN
      CREATE_BUILD_STATUS: "false"
```

## Support

If you're reporting an issue, please include:

- the version of the pipe
- relevant logs and error messages
- steps to reproduce

## License

[MIT License](LICENSE)

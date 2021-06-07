# Bitbucket Pipelines Pipe: ReSharper Inspections Report

Create a report with annotations from a ReSharper inspections XML, and a
corresponding build status with the status of the report.

## YAML Definition

Add the following snippet to the script section of
your `bitbucket-pipelines.yml` file:

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.3
    variables:
      INSPECTIONS_XML_PATH: "<string>"
      # BITBUCKET_USERNAME: "<string>" # Optional
      # BITBUCKET_APP_PASSWORD: "<string>" # Optional
      # CREATE_BUILD_STATUS: "<boolean>" # Optional, default "true"
      # DEBUG: "<boolean>" # Optional
```

## Variables

| Variable                  | Usage |
| ------------------------- | ----- |
| INSPECTIONS_XML_PATH (\*) | Path to inspections xml file, relative to current directory. You can use patterns that <br/> are supported by [DirectoryInfo.GetFiles](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo.getfiles). |
| BITBUCKET_USERNAME        | Bitbucket username, required to create build status. Note, that this should be an account name, not the email. |
| BITBUCKET_APP_PASSWORD    | Bitbucket app password, required to create build status. |
| CREATE_BUILD_STATUS       | Whether to create build status reflecting the results of the report. Default: `true`. |
| DEBUG                     | Turn on extra debug information. Default: `false`. |

_(\*) = required variable._

## Prerequisites

### Inspections File

You need to create the inspections XML file before calling the pipe. To create
the inspections XML file see
[InspectCode Command-Line Tool](https://www.jetbrains.com/help/resharper/InspectCode.html)
.

### App Password Required for Build Status

Build status will be created only if username and app password are provided.
To have this pipe create build status, you need to
[generate an app password](https://confluence.atlassian.com/bitbucket/app-passwords-828781300.html).
Only the Repositories Read permission is required.

## Examples

Basic example:

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.3
    variables:
      INSPECTIONS_XML_PATH: "inspect.xml"
```

With pattern:

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.3
    variables:
      INSPECTIONS_XML_PATH: "src/*/inspect.xml"
```

With app password (you should use secure variables for username and app password):

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.3
    variables:
      INSPECTIONS_XML_PATH: "src/*/inspect.xml"
      BITBUCKET_USERNAME: $USERNAME
      BITBUCKET_APP_PASSWORD: $APP_PASSWORD
```

Temporarily disable build status creation:

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.3
    variables:
      INSPECTIONS_XML_PATH: "src/*/inspect.xml"
      BITBUCKET_USERNAME: $USERNAME
      BITBUCKET_APP_PASSWORD: $APP_PASSWORD
      CREATE_BUILD_STATUS: "false"
```

## Support

If you're reporting an issue, please include:

- the version of the pipe
- relevant logs and error messages
- steps to reproduce

## License

[MIT License](LICENSE)

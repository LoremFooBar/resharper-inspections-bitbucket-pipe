# Bitbucket Pipelines Pipe: ReSharper Inspections Report

Create a report with annotations from a ReSharper inspections XML, and a
corresponding build status with the status of the report.

## YAML Definition

Add the following snippet to the script section of your `bitbucket-pipelines.yml` file:

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.2
    variables:
      INSPECTIONS_XML_PATH: "<string>"
      # BITBUCKET_OAUTH_KEY: "<string>" # Optional
      # BITBUCKET_OAUTH_SECRET: "<string>" # Optional
      # DEBUG: '<boolean>' # Optional
```

## Variables

| Variable                  | Usage |
| ------------------------- | ----- |
| INSPECTIONS_XML_PATH (\*) | Path to inspections xml file, relative to current directory. You can use patterns that <br/> are supported by [DirectoryInfo.GetFiles](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo.getfiles) |
| BITBUCKET_OAUTH_KEY       | OAuth consumer key |
| BITBUCKET_OAUTH_SECRET    | OAuth consumer secret |
| DEBUG                     | Turn on extra debug information. Default: `false`. |

_(\*) = required variable._

## Prerequisites

### Inspections File

You need to create the inspections XML file before calling the pipe.
To create the inspections XML file see
[InspectCode Command-Line Tool](https://www.jetbrains.com/help/resharper/InspectCode.html).

### OAuth Required for Build Status

Build status will be created only if OAuth key and secret are provided. Otherwise, only
a pipeline report will be created.

OAuth consumer configuration:

1. Set a callback URL - you can use your Bitbucket workspace URL.
1. Check the "This is a private consumer" checkbox to enable `client_credentials`.
2. Allow Repository write permissions

## Examples

Basic example:

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.2
    variables:
      INSPECTIONS_XML_PATH: "inspect.xml"
```

With pattern:

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.2
    variables:
      INSPECTIONS_XML_PATH: "src/*/inspect.xml"
```

With OAuth (you should use secure variables for key and secret):

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.2
    variables:
      INSPECTIONS_XML_PATH: "src/*/inspect.xml"
      BITBUCKET_OAUTH_KEY: $OAUTH_KEY
      BITBUCKET_OAUTH_SECRET: $OAUTH_SECRET
```

## Support

If you're reporting an issue, please include:

-   the version of the pipe
-   relevant logs and error messages
-   steps to reproduce

## License

[MIT License](LICENSE)

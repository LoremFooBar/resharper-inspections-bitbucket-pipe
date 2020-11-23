# Bitbucket Pipelines Pipe: ReSharper Inspections Report

Create a report with annotations from a ReSharper inspections XML.

## YAML Definition

Add the following snippet to the script section of your `bitbucket-pipelines.yml` file:

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.1.5
    variables:
      INSPECTIONS_XML_PATH: "<string>"
      # DEBUG: '<boolean>' # Optional, not yet implemented
```

## Variables

| Variable                  | Usage |
| ------------------------- | ----- |
| INSPECTIONS_XML_PATH (\*) | Path to inspections xml file, relative to current directory. You can use patterns that <br/> are supported by [DirectoryInfo.GetFiles](https://docs.microsoft.com/en-us/dotnet/api/system.io.directoryinfo.getfiles) |
| DEBUG                     | Turn on extra debug information. Default: `false`.                                                                                                                                                                   |

_(\*) = required variable._

## Prerequisites

You need to create the inspections XML file before calling the pipe.
To create the inspections XML file see
[InspectCode Command-Line Tool](https://www.jetbrains.com/help/resharper/InspectCode.html).

## Examples

Basic example:

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.1.5
    variables:
      INSPECTIONS_XML_PATH: "inspect.xml"
```

With pattern:

```yaml
script:
  - pipe: docker://lazyboy1/resharper-inspections-bitbucket-pipe:0.1.5
    variables:
      INSPECTIONS_XML_PATH: "src/*/inspect.xml"
```

## Support

TBD how to report issues?

If you're reporting an issue, please include:

-   the version of the pipe
-   relevant logs and error messages
-   steps to reproduce

## License

Unlicensed at the moment.

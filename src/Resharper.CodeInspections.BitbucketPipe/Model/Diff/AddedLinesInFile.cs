namespace Resharper.CodeInspections.BitbucketPipe.Model.Diff;

// ReSharper disable once NotAccessedPositionalProperty.Global
public record AddedLinesInFile(string FilePath, List<int> LinesAdded);

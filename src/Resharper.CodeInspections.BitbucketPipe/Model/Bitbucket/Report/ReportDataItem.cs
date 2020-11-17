using System;

namespace Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report
{
    [Serializable]
    public class ReportDataItem
    {
        public ReportDataType Type { get; set; }
        public string? Title { get; set; }
        public string? Value { get; set; }
    }
}

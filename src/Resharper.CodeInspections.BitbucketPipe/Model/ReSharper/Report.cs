using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Serilog;

namespace Resharper.CodeInspections.BitbucketPipe.Model.ReSharper
{
    [XmlRoot(ElementName = "Report")]
    public class Report
    {
        [XmlElement(ElementName = "Information")]
        public Information Information { get; set; } = null!;

        [XmlElement(ElementName = "IssueTypes")]
        public IssueTypes IssueTypes { get; set; } = null!;

        [XmlElement(ElementName = "Issues")]
        public Issues Issues { get; set; } = null!;

        [XmlIgnore]
        public int TotalIssues => AllIssues.Count;

        [XmlIgnore]
        public bool HasAnyIssues => Issues.Projects?.Any() == true;

        [XmlIgnore]
        public List<Issue> AllIssues =>
            Issues.Projects?.Where(p => p.Issues != null).SelectMany(x => x.Issues!).ToList() ?? new List<Issue>();

        public static async Task<Report> CreateFromFileAsync(string filePathOrPattern)
        {
            var currentDir = new DirectoryInfo(Environment.CurrentDirectory);
            Log.Logger.Debug("Current directory: {Directory}", currentDir);

            var reportFile = currentDir.GetFiles(filePathOrPattern).FirstOrDefault();
            if (reportFile == null) {
                throw new FileNotFoundException($"could not find report file in directory {currentDir.FullName}",
                    filePathOrPattern);
            }

            Log.Logger.Debug("Found inspections file: {File}", reportFile.FullName);

            await using var fileStream = reportFile.OpenRead();
            Log.Logger.Debug("Deserializing report...");
            object deserializedReport = new XmlSerializer(typeof(Report)).Deserialize(fileStream) ??
                                        throw new SerializationException("Failed to deserialize the report xml");
            Log.Logger.Debug("Report deserialized successfully");
            return (Report) deserializedReport;
        }
    }
}

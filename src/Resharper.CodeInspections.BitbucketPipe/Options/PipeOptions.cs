namespace Resharper.CodeInspections.BitbucketPipe.Options
{
    public class PipeOptions
    {
        public bool CreateBuildStatus { get; set; }
        public string InspectionsXmlPathOrPattern { get; set; } = "";
    }
}

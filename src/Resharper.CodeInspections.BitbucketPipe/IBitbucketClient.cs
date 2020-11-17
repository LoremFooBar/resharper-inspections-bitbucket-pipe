using System.Collections.Generic;
using System.Threading.Tasks;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.Report;

namespace Resharper.CodeInspections.BitbucketPipe
{
    public interface IBitbucketClient
    {
        public Task CreateReportAsync(PipelineReport report, IEnumerable<Annotation> annotations);
    }
}

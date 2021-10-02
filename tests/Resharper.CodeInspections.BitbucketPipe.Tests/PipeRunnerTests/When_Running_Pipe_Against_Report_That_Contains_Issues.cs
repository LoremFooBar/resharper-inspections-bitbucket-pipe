using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using Moq;
using Moq.Contrib.HttpClient;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests
{
    public class When_Running_Pipe_Against_Report_That_Contains_Issues : PipeRunnerSpecificationBase
    {
        protected override void Given()
        {
            base.Given();

            var environmentVariableProvider =
                new EnvironmentVariableProviderMock(TestData.NonEmptyReportFilePath, new Dictionary<string, string>
                {
                    ["BITBUCKET_USERNAME"] = "user",
                    ["BITBUCKET_APP_PASSWORD"] = "password",
                    ["CREATE_BUILD_STATUS"] = "true"
                });

            TestPipeRunner = new(environmentVariableProvider.Object, MessageHandlerMock);
        }

        [Then]
        public void It_Should_Send_Report_To_Bitbucket()
        {
            MessageHandlerMock.VerifyRequest(
                request => request.RequestUri!.PathAndQuery.EndsWith("/reports/resharper-inspections") &&
                           request.Method == HttpMethod.Put,
                Times.Once());
        }

        [Then]
        public void It_Should_Send_Four_Annotations_To_Bitbucket()
        {
            MessageHandlerMock.VerifyRequest(async request =>
                {
                    bool isAnnotationsRequest = request.RequestUri!.PathAndQuery.EndsWith("/annotations") &&
                                                request.Method == HttpMethod.Post;
                    if (!isAnnotationsRequest) {
                        return false;
                    }

                    var annotations = await request.Content!.ReadFromJsonAsync<List<Annotation>>();
                    return annotations?.Count == 4;
                },
                Times.Once());
        }

        [Then]
        public void It_Should_Send_Build_Status_To_Bitbucket()
        {
            MessageHandlerMock.VerifyRequest(
                request => request.RequestUri!.PathAndQuery.EndsWith("statuses/build"),
                Times.Once());
        }
    }
}

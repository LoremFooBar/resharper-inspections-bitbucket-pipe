﻿using System.Net.Http.Json;
using Moq;
using Moq.Contrib.HttpClient;
using Resharper.CodeInspections.BitbucketPipe.Model.Bitbucket.CodeAnnotations;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Tests.Helpers;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.PipeRunnerTests;

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
                ["CREATE_BUILD_STATUS"] = "true",
            });

        TestPipeRunner = new TestPipeRunner(environmentVariableProvider.Object, MessageHandlerMock);
    }

    protected override async Task WhenAsync()
    {
        await base.WhenAsync();

        await TestPipeRunner.RunPipeAsync();
    }

    [Then]
    public void It_Should_Send_Report_To_Bitbucket()
    {
        MessageHandlerMock.VerifyRequest(
            request => request.RequestUri!.PathAndQuery.EndsWith("/reports/resharper-inspections",
                           StringComparison.Ordinal) &&
                       request.Method == HttpMethod.Put,
            Times.Once());
    }

    [Then]
    public void It_Should_Send_Four_Annotations_To_Bitbucket()
    {
        MessageHandlerMock.VerifyRequest(async request =>
            {
                bool isAnnotationsRequest =
                    request.RequestUri!.PathAndQuery.EndsWith("/annotations", StringComparison.Ordinal) &&
                    request.Method == HttpMethod.Post;

                if (!isAnnotationsRequest) return false;

                var annotations = await request.Content!.ReadFromJsonAsync<List<Annotation>>();

                return annotations?.Count == 4;
            },
            Times.Once());
    }

    [Then]
    public void It_Should_Send_Build_Status_To_Bitbucket()
    {
        MessageHandlerMock.VerifyRequest(
            request => request.RequestUri!.PathAndQuery.EndsWith("statuses/build", StringComparison.Ordinal),
            Times.Once());
    }
}

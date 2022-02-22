﻿using Moq;
using Resharper.CodeInspections.BitbucketPipe.Tests.BDD;
using Resharper.CodeInspections.BitbucketPipe.Utils;

namespace Resharper.CodeInspections.BitbucketPipe.Tests.EnvironmentVariableProviderTests
{
    public class When_Getting_Required_Environment_Variable_That_Does_Not_Exist : SpecificationBase
    {
        private IEnvironmentVariableProvider _environmentVariableProvider;
        private Func<string> _func;

        protected override void Given()
        {
            base.Given();

            var envMock = new Mock<IEnvironmentVariableProvider> {CallBase = true};
            envMock
                .Setup(_ => _.GetEnvironmentVariable(It.IsAny<string>()))
                .Returns<string>((_) => null);

            _environmentVariableProvider = envMock.Object;
        }

        protected override void When()
        {
            base.When();

            _func = () => _environmentVariableProvider.GetRequiredEnvironmentVariable("Lies");
        }

        [ThenAttribute]
        public void It_Should_Throw_RequiredEnvironmentVariableNotFoundException()
        {
            _func.Should().Throw<RequiredEnvironmentVariableNotFoundException>();
        }
    }
}

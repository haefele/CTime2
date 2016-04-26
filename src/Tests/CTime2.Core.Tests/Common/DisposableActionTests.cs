using System;
using CTime2.Core.Common;
using Xunit;

namespace CTime2.Core.Tests.Common
{
    public class DisposableActionTests
    {
        [Fact]
        public void ShouldWorkWithNullArgument()
        {
            //Arrange
            var action = new DisposableAction(null);

            //Act
            action.Dispose();

            //Assert
            Assert.True(true);
        }

        [Fact]
        public void WillExecuteTheActionOnDispose()
        {
            //Arrange
            bool wasRun = false;
            var action = new DisposableAction(() => wasRun = true);

            //Act
            action.Dispose();

            //Assert
            Assert.True(wasRun);
        }
    }
}
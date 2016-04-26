using System;
using CTime2.Core.Common;
using Xunit;

namespace CTime2.Core.Tests.Common
{
    public class CTimeExceptionTests
    {
        [Fact]
        public void TakesMessage()
        {
            var message = "Hello world";
            var exception = new CTimeException(message);

            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void TakesMessageAndInnerException()
        {
            var message = "Hello world";
            var inner = new Exception();
            var exception = new CTimeException(message, inner);

            Assert.Equal(message, exception.Message);
            Assert.Equal(inner, exception.InnerException);
        }
    }
}
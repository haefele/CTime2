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
        public void ThrowsWithNullArguments()
        {
            //Arrange
            string badMessage = null;
            string goodMessage = "Message";
            Exception badInner = null;
            Exception goodInner = new Exception("My inner exception");

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new CTimeException(badMessage));
            Assert.Throws<ArgumentNullException>(() => new CTimeException(badMessage, badInner));
            Assert.Throws<ArgumentNullException>(() => new CTimeException(badMessage, goodInner));
            Assert.Throws<ArgumentNullException>(() => new CTimeException(goodMessage, badInner));
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
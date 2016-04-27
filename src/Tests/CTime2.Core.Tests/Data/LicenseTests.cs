using System;
using CTime2.Core.Data;
using Xunit;

namespace CTime2.Core.Tests.Data
{
    public class LicenseTests
    {
        [Fact]
        public void TakesLicenseName()
        {
            //Arrange
            var name = "My super secret license";

            //Act
            var license = new License(name);

            //Assert
            Assert.Equal(name, license.Name);
        }

        [Fact]
        public void DoesNotTakeNullName()
        {
            //Arrange
            string name = null;

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new License(name));
        }
    }
}
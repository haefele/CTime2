using System;
using CTime2.Core.Services.Band;
using Xunit;

namespace CTime2.Core.Tests.Services.Band
{
    public class BandInfoTests
    {
        [Fact]
        public void TakesName()
        {
            var name = "MyBandName";

            var info = new BandInfo(name);

            Assert.Equal(name, info.Name);
        }

        [Fact]
        public void ThrowsWithNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new BandInfo(null));
        }
    }
}
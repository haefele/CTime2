using System;
using CTime2.Core.Data;
using Xunit;

namespace CTime2.Core.Tests.Data
{
    public class TimeTests
    {
        [Fact]
        public void CanSetAndGetAllProperties()
        {
            //Arrange
            var time = new Time();

            //Act
            var day = time.Day = new DateTime(2000, 12, 1, 12, 0, 0);
            var hours = time.Hours = TimeSpan.FromHours(2);
            var state = time.State = TimeState.Entered;
            var clockInTime = time.ClockInTime = new DateTime(2000, 12, 2);
            var clockOutTime = time.ClockOutTime = new DateTime(2000, 12, 3);

            //Assert
            Assert.Equal(day, time.Day);
            Assert.Equal(hours, time.Hours);
            Assert.Equal(state, time.State);
            Assert.Equal(clockInTime, time.ClockInTime);
            Assert.Equal(clockOutTime, time.ClockOutTime);
        }
    }
}
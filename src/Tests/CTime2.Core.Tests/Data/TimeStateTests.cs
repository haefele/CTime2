using CTime2.Core.Data;
using Xunit;

namespace CTime2.Core.Tests.Data
{
    public class TimeStateTests
    {
        [Theory]
        [InlineData(TimeState.Entered, true)]
        [InlineData(TimeState.HomeOffice, false)]
        [InlineData(TimeState.Left, false)]
        [InlineData(TimeState.ShortBreak, false)]
        [InlineData(TimeState.Trip, false)]
        [InlineData(TimeState.Entered | TimeState.HomeOffice, true)]
        [InlineData(TimeState.Entered | TimeState.Left, true)]
        [InlineData(TimeState.Entered | TimeState.ShortBreak, true)]
        [InlineData(TimeState.Entered | TimeState.Trip, true)]
        public void IsEnteredWorks(TimeState state, bool expectedResult)
        {
            bool isEntered = state.IsEntered();

            Assert.Equal(expectedResult, isEntered);
        }

        [Fact]
        public void IsEnteredWorksWithNull()
        {
            TimeState? state = null;

            bool isEntered = state.IsEntered();

            Assert.False(isEntered);
        }

        [Theory]
        [InlineData(TimeState.Left, true)]
        [InlineData(TimeState.HomeOffice, false)]
        [InlineData(TimeState.Entered, false)]
        [InlineData(TimeState.ShortBreak, false)]
        [InlineData(TimeState.Trip, false)]
        [InlineData(TimeState.Left | TimeState.HomeOffice, true)]
        [InlineData(TimeState.Left | TimeState.Entered, true)]
        [InlineData(TimeState.Left | TimeState.ShortBreak, true)]
        [InlineData(TimeState.Left | TimeState.Trip, true)]
        public void IsLeftWorks(TimeState state, bool expectedResult)
        {
            bool isLeft = state.IsLeft();

            Assert.Equal(expectedResult, isLeft);
        }

        [Fact]
        public void IsLeftWorksWithNull()
        {
            TimeState? state = null;

            bool isLeft = state.IsLeft();

            Assert.False(isLeft);
        }
        
        [Theory]
        [InlineData(TimeState.HomeOffice, true)]
        [InlineData(TimeState.Left, false)]
        [InlineData(TimeState.Entered, false)]
        [InlineData(TimeState.ShortBreak, false)]
        [InlineData(TimeState.Trip, false)]
        [InlineData(TimeState.HomeOffice | TimeState.Left, true)]
        [InlineData(TimeState.HomeOffice | TimeState.Entered, true)]
        [InlineData(TimeState.HomeOffice | TimeState.ShortBreak, true)]
        [InlineData(TimeState.HomeOffice | TimeState.Trip, true)]
        public void IsHomeOfficeWorks(TimeState state, bool expectedResult)
        {
            bool isLeft = state.IsHomeOffice();

            Assert.Equal(expectedResult, isLeft);
        }

        [Fact]
        public void IsHomeOfficeWorksWithNull()
        {
            TimeState? state = null;

            bool isHomeOffice = state.IsHomeOffice();

            Assert.False(isHomeOffice);
        }



        [Theory]
        [InlineData(TimeState.Trip, true)]
        [InlineData(TimeState.Left, false)]
        [InlineData(TimeState.Entered, false)]
        [InlineData(TimeState.ShortBreak, false)]
        [InlineData(TimeState.HomeOffice, false)]
        [InlineData(TimeState.Trip | TimeState.Left, true)]
        [InlineData(TimeState.Trip | TimeState.Entered, true)]
        [InlineData(TimeState.Trip | TimeState.ShortBreak, true)]
        [InlineData(TimeState.Trip | TimeState.HomeOffice, true)]
        public void IsTripWorks(TimeState state, bool expectedResult)
        {
            bool isLeft = state.IsTrip();

            Assert.Equal(expectedResult, isLeft);
        }

        [Fact]
        public void IsTripWorksWithNull()
        {
            TimeState? state = null;

            bool isHomeOffice = state.IsTrip();

            Assert.False(isHomeOffice);
        }
    }
}
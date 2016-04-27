using CTime2.Core.Data;
using Xunit;

namespace CTime2.Core.Tests.Data
{
    public class AttendingUserTests
    {
        [Fact]
        public void CanSetAndGetAllProperties()
        {
            var user = new AttendingUser();
            
            var name = user.Name = "Name";
            var firstName = user.FirstName = "FirstName";
            var isAttending = user.IsAttending = true;
            var imageAsPng = user.ImageAsPng = new byte[2];
            
            Assert.Equal(name, user.Name);
            Assert.Equal(firstName, user.FirstName);
            Assert.Equal(isAttending, user.IsAttending);
            Assert.Equal(imageAsPng, user.ImageAsPng);
        }
    }
}
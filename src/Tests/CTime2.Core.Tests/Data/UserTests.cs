using CTime2.Core.Data;
using Xunit;

namespace CTime2.Core.Tests.Data
{
    public class UserTests
    {
        [Fact]
        public void CanSetAndGetAllProperties()
        {
            var user = new User();

            var id = user.Id = "Id";
            var name = user.Name = "Name";
            var firstName = user.FirstName = "FirstName";
            var email = user.Email = "Email";
            var imageAsPng = user.ImageAsPng = new byte[2];

            Assert.Equal(id, user.Id);
            Assert.Equal(name, user.Name);
            Assert.Equal(firstName, user.FirstName);
            Assert.Equal(email, user.Email);
            Assert.Equal(imageAsPng, user.ImageAsPng);
        }
    }
}
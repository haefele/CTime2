namespace CTime2.Core.Data
{
    public class AttendingUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public bool IsAttending { get; set; }
        public byte[] ImageAsPng { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}
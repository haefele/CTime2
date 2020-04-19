namespace CTime2.Core.Data
{
    public class AttendingUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public AttendanceState AttendanceState { get; set; }
        public byte[] ImageAsPng { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string[] Departments { get; set; }
    }
}
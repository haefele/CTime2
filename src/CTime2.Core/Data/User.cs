namespace CTime2.Core.Data
{
    public class User
    {
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] ImageAsPng { get; set; }
        public bool SupportsGeoLocation { get; set; }
        public byte[] CompanyImageAsPng { get; set; }
    }
}
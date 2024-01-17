namespace Attendance.Services.DTOs
{
    public class PortDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}

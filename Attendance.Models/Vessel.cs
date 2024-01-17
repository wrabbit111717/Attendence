using System;

namespace Attendance.Models
{
    public class Vessel
    {
        public string VesselName { get; set; }
        public int VesselId { get; set; }
        public string IMO { get; set; }
        public string FLAG { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public double? GrossTonage { get; set; }
        public double? DeadWeight { get; set; }
        public int? Locked { get; set; }
        public int? FleetId { get; set; }
        public string VesselCode { get; set; }
    }
}
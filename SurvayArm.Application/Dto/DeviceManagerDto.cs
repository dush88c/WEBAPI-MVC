
namespace SurvayArm.Application.Dto
{
    public class DeviceManagerDto 
    {
        public int Id { get; set; }
        public int DeviceBrandId { get; set; } 
        public string DeviceModelName { get; set; }
        public string DeviceId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
    }
}

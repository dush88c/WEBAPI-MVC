
namespace SurvayArm.Application.Dto
{
   public class SurvaySupervisorDto 
    {
        public int Id { get; set; }
        public int SurvayId { get; set; }
        public string SupervisorId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public UserDto AspNetUser { get; set; }
        public SurvayDto Survay { get; set; }
    }
}

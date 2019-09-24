using System.Collections.Generic;

namespace SurvayArm.Application.Dto
{
    public class UserSurvayDto 
    {
        public int Id { get; set; }
        public int SurvayId { get; set; }
        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        
        public UserDto AspNetUser { get; set; }
        public SurvayDto Survay { get; set; }
    }

}

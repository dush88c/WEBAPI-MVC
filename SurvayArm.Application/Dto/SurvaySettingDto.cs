using System.Collections.Generic;

namespace SurvayArm.Application.Dto
{
   public class SurvaySettingDto
    {
        public int SurvayId { get; set; }
        public int Target { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }

        public SurvayDto Survay { get; set; }
        
        public List<SurvayTargetDto> SurvayTargets { get; set; }
    }
}

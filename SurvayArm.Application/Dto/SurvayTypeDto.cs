using System.Collections.Generic;

namespace SurvayArm.Application.Dto
{
   public class SurvayTypeDto
    {
        public SurvayTypeDto()
        {
            Fields = new List<FieldDto>();
        }

        public int Id { get; set; }
        public int SurvayId { get; set; }
        public string Description { get; set; }
        public int LanguageType { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public List<FieldDto> Fields { get; set; }
        public SurvayDto Survay { get; set; }
        public ICollection<AnswerSurvayDto> AnswerSurvays { get; set; }
    }
}

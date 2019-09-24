using System.Collections.Generic;

namespace SurvayArm.Application.Dto
{
    public class FieldDto
    {
        public FieldDto()
        {
            this.FieldOption = new FieldOptionDto();
            
        }

        public int Id { get; set; }
        public int SurvayId { get; set; }
        public string Label { get; set; } 
        public string Field_Type { get; set; }
        public bool Required { get; set; }
        public string VideoUrl { get; set; }
        public bool IsImageUpload { get; set; }
        public bool IsVoiceUpload { get; set; }
        public int OrderNo { get; set; }

        public SurvayTypeDto Survay { get; set; }
        public FieldOptionDto FieldOption { get; set; }
        public List<FieldDependantDto> FieldDependants { get; set; }
        public List<FieldDependantDto> FieldDependants1 { get; set; } 
    }
}

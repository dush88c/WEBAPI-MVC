using System;

namespace SurvayArm.Application.Dto
{
    public class SurvayTargetDto
    {
        public int Id { get; set; }
        public int SurvaySettingId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int Target { get; set; }
        public int OptionId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public SurvaySettingDto SurvaySetting { get; set; }
        public DistrictDto District { get; set; }
        public ProvinceDto Province { get; set; }

    }
}
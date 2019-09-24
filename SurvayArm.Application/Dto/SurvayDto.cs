using System.Collections.Generic;

namespace SurvayArm.Application.Dto
{
    public class SurvayDto
    {
        public SurvayDto()
        {
            SurvayTypes = new List<SurvayTypeDto>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int NoOfQuestion { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public List<Language> Languages { get; set; }

        public SurvaySettingDto SurvaySetting { get; set; }
        
        public  List<SurvaySupervisorDto> SurvaySupervisors { get; set; }
        
        public List<SurvayTypeDto> SurvayTypes { get; set; }

        public int CountHasDone { get; set; }

        public int Target { get; set; }

    }

    public class Language
    {
        public string Name { get; set; }
        public int Code { get; set; }
    }
}

using SurvayArm.Application.Dto;
using System.Collections.Generic;

namespace SurvayArm.Application.IService
{
    public interface ISurvayTypeService  
    {
        IEnumerable<SurvayTypeDto> GetSurvayTypes(); 

        IEnumerable<SurvayTypeDto> GetActiveSurvayTypes();

        SurvayTypeDto GetSurvayTypeById(int id);   

        void InsertSurvayType(SurvayTypeDto dto);

        void DeleteSurvay(int id); 

        void UpdateSurvay(SurvayTypeDto dto);

        bool Exists(int id);

        IEnumerable<SurvayTypeDto> GetSurvayTypesBySurvayId(int survayCode);

        SurvayTypeDto GetSurvaysByCodeAndLanguage(int survayCode, int language);
    }
}

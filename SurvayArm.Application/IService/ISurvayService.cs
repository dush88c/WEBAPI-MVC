using SurvayArm.Application.Dto;
using System.Collections.Generic;

namespace SurvayArm.Application.IService
{
    public interface ISurvayService 
    {
        IEnumerable<SurvayDto> GetSurvays();

        IEnumerable<SurvayDto> GetActiveSurvay();       

        IEnumerable<SurvayDto> GetActiveSurvay(int skip, int pageSize, string sortBy, string filetrBy);

        IEnumerable<SurvayDto> GetAllWithPagination(int skip, int pageSize, string sortBy, string filetrBy);

        SurvayDto GetSurvayById(int id); 

        void InsertSurvay(SurvayDto dto);

        void InsertSurvayWithDifferentLanguage(SurvayDto dto);

        void DeleteSurvay(int id); 

        void UpdateSurvay(SurvayDto dto);

        bool Exists(int id);

        void ManageActivation(SurvayDto dto); 

        void Save();

        int GetCount(string sortBy, string filetrBy);

        IEnumerable<SurvayDto> GetActiveSurvayDisctincByCode();

        IEnumerable<SurvayDto> GetSurvaysByCode(int survayCode);

        SurvayDto GetSurvaysByCodeAndLanguage(int survayCode, int language);
    }
}

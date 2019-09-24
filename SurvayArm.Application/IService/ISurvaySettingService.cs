using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurvayArm.Application.Dto;

namespace SurvayArm.Application.IService
{
   public interface ISurvaySettingService
    {
        IEnumerable<SurvaySettingDto> GetActiveSurvaySettings();

        SurvaySettingDto GetSurvaySettingById(int id);

        void InsertSurvaySetting(SurvaySettingDto dto);
         
        void UpdateSurvaySetting(SurvaySettingDto dto);

        void DeleteSurvaySetting(int id); 

        void Save();
    }
}

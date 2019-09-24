using System.Collections.Generic;
using SurvayArm.Application.Dto;

namespace SurvayArm.Application.IService
{
   public interface IFieldOptionService  
    {
        void UpdateFieldOption(FieldOptionDto dto); 

        FieldOptionDto GetFieldOptionByFieldId(int fieldId);  

        void Save();

        void Delete(int id);
    }
}

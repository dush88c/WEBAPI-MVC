using System.Collections.Generic;
using SurvayArm.Application.Dto;

namespace SurvayArm.Application.IService
{
   public interface IFieldService 
    {
        void UpdateField(IList<FieldDto> dto , int id);

        IEnumerable<FieldDto> GetFieldsBySurvayId(int survayId);

        void Save();
    }
}

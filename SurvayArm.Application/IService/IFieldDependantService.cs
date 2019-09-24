using System.Collections.Generic;
using SurvayArm.Application.Dto;

namespace SurvayArm.Application.IService
{
    public interface IFieldDependantService
    {
        IEnumerable<FieldDependantDto> GetFieldDependantsWithInclude(); 

        void InsertFieldDependant(IEnumerable<FieldDependantDto> dto, int survayId);

        void UpdateFieldDependant(IEnumerable<FieldDependantDto> dtos); 

        bool Exists(int id);

        void Save();

        void DeleteFieldDependancyBySurvayId(int survayId);
    }
}
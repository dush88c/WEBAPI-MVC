using System.Collections.Generic;
using SurvayArm.Application.Dto;

namespace SurvayArm.Application.IService
{
   public interface IOptionService   
    {
        void UpdateOption(IList<OptionDto> dto , int fieldOptionId);  

        IList<OptionDto> GetOptionByFieldOptionId(int fieldOptionId);

        bool DeleteOPtionsByFieldOptionId(int fieldOptionId);

        void Save();
    }
}

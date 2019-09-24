using System.Collections.Generic;
using SurvayArm.Application.Dto;

namespace SurvayArm.Application.IService
{
    public interface IProvinceService 
    {
        List<ProvinceDto> GetActiveAll();
       
    }
}

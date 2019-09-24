using System.Collections.Generic;
using SurvayArm.Application.Dto;

namespace SurvayArm.Application.IService
{
    public interface IDistricService
    {
        List<DistrictDto> GetActiveAll();
        List<DistrictDto> GetProvinceId(int provinceId);
    }
}

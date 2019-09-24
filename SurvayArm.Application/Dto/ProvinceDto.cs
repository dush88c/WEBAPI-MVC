
using System.Collections.Generic;

namespace SurvayArm.Application.Dto
{
    public class ProvinceDto 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public  List<DistrictDto> Districts { get; set; }
    }
}

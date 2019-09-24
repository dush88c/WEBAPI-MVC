
namespace SurvayArm.Application.Dto
{
    public class DistrictDto 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProvinceId { get; set; }
        public bool IsActive { get; set; }

        public  ProvinceDto Province { get; set; }
    }
}

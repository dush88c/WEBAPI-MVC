
namespace SurvayArm.Application.Dto
{
    public class MatrixHeaderDto  
    {
        public int Id { get; set; }
        public int FieldOptionId { get; set; }
        public string Label { get; set; }

        public string HeaderId
        {
            get
            {
                return $"HEADER_{Id}";
            }
        }

        public FieldOptionDto FieldOption { get; set; }
    }
}

namespace SurvayArm.Application.Dto
{
    public class MatrixRowDto 
    {
        public int Id { get; set; }
        public int FieldOptionId { get; set; }
        public string Label { get; set; }

        public string RowId { get
            {
                return $"ROW_{Id}";
            }
        }

        public FieldOptionDto FieldOption { get; set; }
    }
}

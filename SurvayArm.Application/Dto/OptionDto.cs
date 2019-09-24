namespace SurvayArm.Application.Dto
{
    public class OptionDto
    {
        public int Id { get; set; }
        public int FieldOptionId { get; set; }
        public string Label { get; set; }
        public bool Checked { get; set; }

        public FieldOptionDto FieldOption { get; set; }
    }
}

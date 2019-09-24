namespace SurvayArm.Application.Dto
{
    public class MatrixAnswerDto 
    {
        public int Id { get; set; }
        public int AnswerFieldId { get; set; }
        public string Row { get; set; }
        public string HeaderList { get; set; } 

        public AnswerFieldDto AnswerField { get; set; }

    }
}

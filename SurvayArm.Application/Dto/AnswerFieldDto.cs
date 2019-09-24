using System.Collections.Generic;

namespace SurvayArm.Application.Dto
{
  public class AnswerFieldDto
    {
        public AnswerFieldDto()
        {
            MatrixAnswers = new List<MatrixAnswerDto>();
        }

        public int Id { get; set; }
        public int AnswerSurvayId { get; set; }
        public int FieldId { get; set; }
        public string Type { get; set; }
        public string Answer { get; set; }
        public string AttachmentType { get; set; }
        public string IncludeOther { get; set; } 
        public AnswerSurvayDto AnswerSurvay { get; set; }
        public FieldDto Field { get; set; }
        public List<MatrixAnswerDto> MatrixAnswers { get; set; }
    }
}

using SurvayArm.Application.Dto;

namespace SurvayArm.API.Models.AppModel
{
    public class SurvayAnswerModel
    {
        public AnswerSurvayDto Answer { get; set; }
        public ClientDto Client { get; set; }

    }
}
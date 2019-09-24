using SurvayArm.Application.Dto;
using System.Collections.Generic;
using System.Data;

namespace SurvayArm.Application.IService
{
  public  interface IAnswerSurvayService
    {
        List<AnswerSurvayDto> GetAll();
        void Insert(AnswerSurvayDto dto);
        DataSet ExportToCsv(int survayId);
        int GetSurvayCountHasDone(int survayId);
    }
}

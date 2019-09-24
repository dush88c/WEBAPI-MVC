using System.Collections.Generic;
using SurvayArm.Application.Dto;

namespace SurvayArm.Application.IService
{
  public  interface ISurvaySupervisorService
  {
      List<SurvaySupervisorDto> GetSupervisorsBySurvayId(int survayId);

      void UpdateSurvaySupervisors(List<SurvaySupervisorDto> supervisors, int survayId);
  }
}

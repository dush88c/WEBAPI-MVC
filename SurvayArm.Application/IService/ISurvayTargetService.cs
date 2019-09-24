
using System.Collections.Generic;
using SurvayArm.Application.Dto;

namespace SurvayArm.Application.IService
{
    public interface ISurvayTargetService
    {
        void InsertSurvayTargets(List<SurvayTargetDto> dto);

        void UpdateSurvayTargets(List<SurvayTargetDto> dto, int survaySettingId);

    }
}

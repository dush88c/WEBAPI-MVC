using SurvayArm.Application.Dto;
using System.Collections.Generic;

namespace SurvayArm.Application.IService
{
   public interface IUserSurvayService 
    {
        List<UserSurvayDto> GetAll();
        List<UserSurvayDto> GetAllActive();
        UserSurvayDto GetByUserSurvayId(int userSurvayId); 
        List<UserSurvayDto> GetBySurvayId(int survayId);
        List<UserSurvayDto> GetByUserId(string userId);
        void Insert(List<UserSurvayDto> dto ,int survayId);
        void Update(List<UserSurvayDto> dto);
        IEnumerable<UserSurvayDto> GetActiveSurvaysAssignedForLoggedUser(string userId); 

    }
}

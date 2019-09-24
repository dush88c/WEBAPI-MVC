using SurvayArm.Application.Dto;
using System.Collections.Generic;

namespace SurvayArm.Application.IService
{
   public  interface IClientService 
    {
        List<ClientDto> GetAll();
        int Insert(ClientDto dto);
        List<ClientDto> Find(ClientDto criteria); 
    }
}

using SurvayArm.Application.Dto;
using System.Collections.Generic;

namespace SurvayArm.Application.IService
{
   public interface IDeviceManagerService 
    {
        List<DeviceManagerDto> GetAll();
        List<DeviceManagerDto> GetActive();
        DeviceManagerDto GetById(int id);
        DeviceManagerDto GetByDeviceId(string deviceId);
        bool Exist(string deviceId); 
        bool RegisteredByDeviceId(string deviceId); 
        void Insert(DeviceManagerDto dto);
        void Update(DeviceManagerDto dto);
        IEnumerable<DeviceManagerDto> GetAllWithPagination(int skip, int take, string sortBy, string filterBy);
        int GetCount(string sortBy, string filterBy);
    }
}

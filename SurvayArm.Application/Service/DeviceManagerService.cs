using SurvayArm.Application.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using SurvayArm.Application.Dto;
using SurvayArm.Data.UOW;
using AutoMapper;
using log4net;
using SurvayArm.Data.Model;

namespace SurvayArm.Application.Service
{
    public class DeviceManagerService : IDeviceManagerService, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public DeviceManagerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(SurvayService));
            _mapper = mapper;

        }
        public List<DeviceManagerDto> GetActive()
        {
            try
            {
              var entities =   _unitOfWork.GetRepository<DeviceManager>().GetMany(x=>x.IsActive);
                if (entities != null)
                {
                    var dtos = _mapper.Map<List<DeviceManager>, List<DeviceManagerDto>>(entities.ToList());
                    return dtos;
                }
                _log.Info($"INFO :  No Devices found");
                return null;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public List<DeviceManagerDto> GetAll()
        {
            try
            {
                var entities = _unitOfWork.GetRepository<DeviceManager>().GetAll();
                if (entities != null)
                {
                    var dtos = _mapper.Map<List<DeviceManager>, List<DeviceManagerDto>>(entities.ToList());
                    return dtos;
                }
                _log.Info($"INFO :  No Devices found");
                return null;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public DeviceManagerDto GetByDeviceId(string deviceId)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<DeviceManager>().GetSingle(x=> x.DeviceId.ToLower() == deviceId.ToLower()  && x.IsActive);
                var dtos = _mapper.Map<DeviceManager, DeviceManagerDto>(entity);
                return dtos;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public DeviceManagerDto GetById(int id)
        {
            try
            {
                var entity  = _unitOfWork.GetRepository<DeviceManager>().GetSingle(x=>x.Id == id);
                var dtos = _mapper.Map<DeviceManager,DeviceManagerDto>(entity);
                return dtos;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void Insert(DeviceManagerDto dto)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<DeviceManager>();
                var entity = _mapper.Map<DeviceManagerDto, DeviceManager>(dto);
                repo.Insert(entity);
                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void Update(DeviceManagerDto dto)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<DeviceManager>();
                var entity = _mapper.Map<DeviceManagerDto, DeviceManager>(dto);

                repo.Update(entity);
                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public bool  Exist(string deviceId)
        {
            try
            {
                var isExist  = _unitOfWork.GetRepository<DeviceManager>().CheckWithSpecificValue(x =>
                                                        string.Equals(x.DeviceId, deviceId, StringComparison.InvariantCultureIgnoreCase));

                return isExist;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public IEnumerable<DeviceManagerDto> GetAllWithPagination(int skip, int pageSize, string sortBy, string filterBy)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<DeviceManager>().GetAllPaginatedWithInclude(sortBy, filterBy, skip ,pageSize);
                var dto = _mapper.Map<List<DeviceManager>, List<DeviceManagerDto>>(entity.ToList());
                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public int GetCount(string sortBy, string filterBy)
        {
            try
            {
                return _unitOfWork.GetRepository<DeviceManager>().GetCount(sortBy, filterBy).Count();

            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }


        public bool RegisteredByDeviceId(string deviceId)
        {
            try
            {
                var isExist = _unitOfWork.GetRepository<DeviceManager>().CheckWithSpecificValue(x => x.IsActive && 
                                                       string.Equals(x.DeviceId, deviceId, StringComparison.InvariantCultureIgnoreCase));

                return isExist;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        private void Save()
        {
            _unitOfWork.Save();
        }

        #region Implementing IDiosposable...

        #region private dispose variable declaration...

        private bool _disposed;

        #endregion

        /// <summary>
        ///     Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
            }
            _disposed = true;
        }

        /// <summary>
        ///     Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }        

        #endregion
    }
}

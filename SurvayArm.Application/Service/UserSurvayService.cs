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
    public class UserSurvayService : IUserSurvayService, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _log;
        private List<string> IncludeList => new List<string>() { "AspNetUser", "Survay", "Survay.SurvaySetting", "Survay.SurvayTypes" };

        public UserSurvayService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(ClientService));
            _mapper = mapper;
        }

        public List<UserSurvayDto> GetAll()
        {
            try
            {
                var entities = _unitOfWork.GetRepository<UserSurvay>().GetAll();
                var dtos = _mapper.Map<List<UserSurvay>, List<UserSurvayDto>>(entities.ToList());
                return dtos;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public UserSurvayDto GetByUserSurvayId(int userSurvayId) {
            try
            {
                var entity = _unitOfWork.GetRepository<UserSurvay>().GetById(userSurvayId);
                var dto = _mapper.Map<UserSurvay,UserSurvayDto>(entity);
                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public List<UserSurvayDto> GetAllActive() 
        {
            try
            {
                var entities = _unitOfWork.GetRepository<UserSurvay>().GetMany(x=>x.IsActive);
                var dtos = _mapper.Map<List<UserSurvay>, List<UserSurvayDto>>(entities.ToList());
                return dtos;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public List<UserSurvayDto> GetBySurvayId(int survayId)
        {
            try
            {
                var entities = _unitOfWork.GetRepository<UserSurvay>().GetMany(x => x.SurvayId == survayId);
                var dtos = _mapper.Map<List<UserSurvay>, List<UserSurvayDto>>(entities.ToList());
                return dtos;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public List<UserSurvayDto> GetByUserId(string userId)
        {
            try
            {
                var entities = _unitOfWork.GetRepository<UserSurvay>().GetManyWithInclude(x => x.UserId.ToLower() == userId.ToLower(), IncludeList.ToArray());
                var dtos = _mapper.Map<List<UserSurvay>, List<UserSurvayDto>>(entities.ToList());
                return dtos;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void Insert(List<UserSurvayDto> dto ,int survayId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<UserSurvay>();
                var entities = _mapper.Map<List<UserSurvayDto>, List<UserSurvay>>(dto);

                repo.Delete(x => x.SurvayId == survayId);
                repo.Insert(entities);
                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void Update(List<UserSurvayDto> dtos)
        {
            try
            {
                foreach (var item in dtos)
                {
                    var dbDto = GetByUserSurvayId(item.Id);
                    if (dbDto != null)
                    {
                        dbDto.UserId = item.UserId;
                        dbDto.SurvayId = item.SurvayId;
                        dbDto.IsActive = item.IsActive;
                        dbDto.UpdatedBy = item.UpdatedBy;
                        dbDto.UpdatedDate = DateTime.Now;

                        var entity = _mapper.Map<UserSurvayDto,UserSurvay>(dbDto);
                        _unitOfWork.GetRepository<UserSurvay>().Update(entity);                        
                    }
                    _log.Error($"Error : UserSurvay is not existing in DB , UserSurvay details :  {item.Id} {item.UserId} {item.SurvayId}");
                }

                Save();                
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public IEnumerable<UserSurvayDto> GetActiveSurvaysAssignedForLoggedUser(string userId)
        {
            try
            {
                var entities = _unitOfWork.GetRepository<UserSurvay>().GetManyWithInclude(x => x.Survay.IsActive
                                                                        && x.UserId== userId , IncludeList.ToArray()).OrderBy(x => x.Survay.CreatedDate);
                var dto = _mapper.Map<List<UserSurvay>, List<UserSurvayDto>>(entities.ToList());
                return dto;
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

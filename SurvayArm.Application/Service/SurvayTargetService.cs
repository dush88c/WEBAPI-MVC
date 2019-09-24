using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using log4net;
using SurvayArm.Application.Dto;
using SurvayArm.Application.IService;
using SurvayArm.Data.Model;
using SurvayArm.Data.UOW;

namespace SurvayArm.Application.Service
{
    public class SurvayTargetService : ISurvayTargetService ,IDisposable 
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public SurvayTargetService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(SurvayTargetService));
            _mapper = mapper;

        } 
         
        public List<SurvayTargetDto> GetSurvayTargetBySurvaySettingId(int survaySettingId)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<SurvayTarget>().GetByIdWithInclude(x => x.SurvaySettingId == survaySettingId).ToList();
                var dto = _mapper.Map<List<SurvayTarget>, List<SurvayTargetDto>>(entity); 
                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }
        public void InsertSurvayTargets(List<SurvayTargetDto> dto)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<SurvayTarget>();
                var entity = _mapper.Map<List<SurvayTargetDto>, List<SurvayTarget>>(dto);

                repo.Insert(entity);
                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void UpdateSurvayTargets(List<SurvayTargetDto> dto ,int survaySettingId)
        {

            try
            {
                var savedTargets = GetSurvayTargetBySurvaySettingId(survaySettingId);
                bool isDbSaved = false;
                var deletedTargets = savedTargets.Where(saved => dto.FirstOrDefault(x => x.Id == saved.Id && x.Id != 0) == null).ToList();

                var updatedTargets = savedTargets.Where(saved => dto.FirstOrDefault(x => x.Id == saved.Id) != null).ToList();

                var repo = _unitOfWork.GetRepository<SurvayTarget>(); 
                foreach (var deleted in deletedTargets)
                {
                    repo.Delete(deleted?.Id);
                    isDbSaved = true;
                }

                if (updatedTargets.Any())
                {
                    var updatedTargetsEntities = _mapper.Map<List<SurvayTargetDto>, List<SurvayTarget>>(updatedTargets);
                    if (updatedTargetsEntities != null)
                    {
                        foreach (var updatedTargetsEntity in updatedTargetsEntities)
                        {
                            var latestTarget = dto.SingleOrDefault(x => x.Id == updatedTargetsEntity.Id);
                            if (latestTarget == null) continue;
                            updatedTargetsEntity.Target = latestTarget.Target;
                            updatedTargetsEntity.ProvinceId = latestTarget.ProvinceId;
                            updatedTargetsEntity.DistrictId = latestTarget.DistrictId;
                            updatedTargetsEntity.OptionId = latestTarget.OptionId;
                            updatedTargetsEntity.UpdatedDate = latestTarget.UpdatedDate;
                            updatedTargetsEntity.UpdatedBy = latestTarget.UpdatedBy;
                        }
                        repo.Update(updatedTargetsEntities);
                        isDbSaved = true;
                    }
                }

                var newTragets = dto.Where(x => x.Id <= 0).ToList();

                if (newTragets.Any())
                {
                    var newTragetsEntities = _mapper.Map<List<SurvayTargetDto>, List<SurvayTarget>>(newTragets);
                    repo.Insert(newTragetsEntities);
                    isDbSaved = true;
                }
                if (isDbSaved)
                {
                    Save();
                }
               
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }


        public void Save()
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

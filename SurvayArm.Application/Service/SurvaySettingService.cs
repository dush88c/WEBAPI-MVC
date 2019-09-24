
using SurvayArm.Application.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using log4net;
using SurvayArm.Application.Dto;
using SurvayArm.Data.Model;
using SurvayArm.Data.UOW;

namespace SurvayArm.Application.Service
{
   public  class SurvaySettingService : ISurvaySettingService , IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISurvayTargetService _survayTargetService; 
        private readonly IMapper _mapper;
        private readonly ILog _log;
        private List<string> IncludeList => new List<string>() { "Survay", "SurvayTargets" };
        public SurvaySettingService(IUnitOfWork unitOfWork, IMapper mapper, ISurvayTargetService survayTargetService)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(OptionService));
            _mapper = mapper;
            _survayTargetService = survayTargetService;
        }

        public IEnumerable<SurvaySettingDto> GetActiveSurvaySettings()
        {

            try
            {
                var entity = _unitOfWork.GetRepository<SurvaySetting>().GetMany(x =>x.IsActive);
                var dto = _mapper.Map<List<SurvaySetting>, List<SurvaySettingDto>>(entity.ToList());
                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public SurvaySettingDto GetSurvaySettingById(int id)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<SurvaySetting>().GetByIdWithInclude(x => x.SurvayId == id, IncludeList.ToArray());
                var dto = _mapper.Map<SurvaySetting, SurvaySettingDto>(entity.SingleOrDefault());
                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void InsertSurvaySetting(SurvaySettingDto dto)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<SurvaySetting>();
                var entity = _mapper.Map<SurvaySettingDto, SurvaySetting>(dto);

                repo.Insert(entity);
                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void UpdateSurvaySetting(SurvaySettingDto dto)
        {
            try
            {
                var dbDto = GetSurvaySettingById(dto.SurvayId);
                if (dbDto == null)
                {
                    var newEntity = _mapper.Map<SurvaySettingDto, SurvaySetting>(dto);
                    _unitOfWork.GetRepository<SurvaySetting>().Insert(newEntity);
                    Save();
                    return;
                }
                dbDto.Target = dto.Target;
                dbDto.IsActive = dto.IsActive;
                dbDto.UpdatedBy = dto.UpdatedBy;
                dbDto.UpdatedDate = DateTime.Now;

                var entity = _mapper.Map<SurvaySettingDto, SurvaySetting>(dbDto);
                _unitOfWork.GetRepository<SurvaySetting>().Update(entity);

                var survayTargets = dto.SurvayTargets; 
                if (survayTargets != null && survayTargets.Any())
                {
                    _survayTargetService.UpdateSurvayTargets(survayTargets , dto.SurvayId);
                }
                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void DeleteSurvaySetting(int id)
        {
            try
            {
                _unitOfWork.GetRepository<SurvaySetting>().Delete(id);
                Save();
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

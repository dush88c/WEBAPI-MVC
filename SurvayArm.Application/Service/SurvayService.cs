using AutoMapper;
using log4net;
using MoreLinq;
using SurvayArm.Application.Dto;
using SurvayArm.Application.IService;
using SurvayArm.Data.Model;
using SurvayArm.Data.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using SurvayArm.Application.Enum;

namespace SurvayArm.Application.Service
{
    public class SurvayService : ISurvayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; 
        private readonly ILog _log;
        private readonly ISurvayTypeService _survayTypeService;
        private List<string> IncludeList => new List<string>() {"SurvaySetting", "SurvaySetting.SurvayTargets", "SurvayTypes", "SurvayTypes.Fields", "SurvayTypes.Fields.FieldOption", "SurvayTypes.Fields.FieldDependants", "SurvayTypes.Fields.FieldOption.Options" };

        public SurvayService(IUnitOfWork unitOfWork , IMapper mapper , ISurvayTypeService survayTypeService)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(SurvayService));
            _mapper = mapper;
            _survayTypeService = survayTypeService;
        }

        public IEnumerable<SurvayDto> GetSurvays() 
        {
            try
            {
                var entity =
                    _unitOfWork.GetRepository<Survay>().GetAll().OrderByDescending(x => x.UpdatedDate);
                
                var dto = _mapper.Map<List<Survay>, List<SurvayDto>>(entity.ToList());
                return dto;
            }
            catch (Exception e)
            {
                 _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public IEnumerable<SurvayDto> GetActiveSurvay()
        {
            try
            {
                var entity = _unitOfWork.GetRepository<Survay>().GetMany(x=>x.IsActive).OrderByDescending(x => x.UpdatedDate);
                var dto = _mapper.Map<List<Survay>, List<SurvayDto>>(entity.ToList());
                return dto;
            }
            catch (Exception e)
            {
                 _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }
        
        public IEnumerable<SurvayDto> GetActiveSurvay(int skip, int pageSize, string sortBy, string filetrBy)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<Survay>().GetWithInclude(c => c.IsActive, sortBy, filetrBy, skip, pageSize, IncludeList.ToArray());
                var dto = _mapper.Map<List<Survay>, List<SurvayDto>>(entity.ToList());
                return dto;
            }
            catch (Exception e)
            {
                 _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public IEnumerable<SurvayDto> GetAllWithPagination(int skip, int pageSize, string sortBy, string filetrBy)
        {
            try
            {               
                var entity = _unitOfWork.GetRepository<Survay>().GetAllPaginatedWithInclude(sortBy, filetrBy, skip, pageSize);
                var dto = _mapper.Map<List<Survay>, List<SurvayDto>>(entity.ToList());
                return dto;
            }
            catch (Exception e)
            {
                 _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }


        public SurvayDto GetSurvayById(int id)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<Survay>().GetByIdWithInclude(x => x.Id == id, IncludeList.ToArray());
                var dto = _mapper.Map<Survay, SurvayDto>(entity.SingleOrDefault());
                return dto;
            }
            catch (Exception e)
            {
                 _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }


        public IEnumerable<SurvayDto> GetSurvaysByCode(int survayCode)
        {
            try
            {                
                var entity = _unitOfWork.GetRepository<Survay>().GetManyWithInclude(x=>x.Id == survayCode , IncludeList.ToArray());
                var dto = _mapper.Map<List<Survay>, List<SurvayDto>>(entity.ToList());
                return dto;
            }
            catch (Exception e)
            {
                 _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public SurvayDto GetSurvaysByCodeAndLanguage(int survayCode, int language) 
        {
            try
            {
                //var entity = _unitOfWork.GetRepository<Survay>().GetQuerybleObject(IncludeList.ToArray());
                //var dto = _mapper.Map<Survay,SurvayDto>(entity.SingleOrDefault(x=>x.Code == survayCode && x.LanguageType == language && x.IsActive));
                //return dto;
                return null;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }



        public IEnumerable<SurvayDto> GetActiveSurvayDisctincByCode()
        {
            try
            {
                var entity = _unitOfWork.GetRepository<Survay>().GetManyQueryable(x=>x.IsActive);
                var dto = _mapper.Map<List<Survay>, List<SurvayDto>>(entity.ToList());
                return dto;

            }
            catch (Exception e)
            {
                 _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void InsertSurvay(SurvayDto dto)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<Survay>();
                var entity = _mapper.Map<SurvayDto, Survay>(dto);
                
                repo.Insert(entity);
                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void InsertSurvayWithDifferentLanguage(SurvayDto dto)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<Survay>();
                var entity = _mapper.Map<SurvayDto, Survay>(dto);
                repo.Insert(entity);
                Save();

            }
            catch (Exception e)
            {
                 _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }


        public void UpdateSurvay(SurvayDto dto)
        {
            try
            {
                var dbDto = GetSurvayById(dto.Id);
                if (dbDto != null)
                {
                    dbDto.Name = dto.Name;
                    dbDto.NoOfQuestion = dto.NoOfQuestion;
                    dbDto.IsActive = dto.IsActive;
                    dbDto.UpdatedBy = dto.UpdatedBy;
                    dbDto.UpdatedDate = DateTime.Now;

                    var entity = _mapper.Map<SurvayDto, Survay>(dbDto);
                    _unitOfWork.GetRepository<Survay>().Update(entity);

                    if (dto.SurvayTypes.Any())
                    {
                        var type = dto.SurvayTypes.First();
                        _survayTypeService.UpdateSurvay(type);
                    }
                    
                    Save();

                }

                _log.Error($"Error : Survay is not existing in DB , survay details :  {dto.Id} {dto.Name}");
            }
            catch (Exception e)
            {
                 _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }


        public void DeleteSurvay(int id)
        {
            try
            {
                _unitOfWork.GetRepository<Survay>().Delete(id);
                Save();
            }
            catch (Exception e)
            {
                 _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }


        public void ManageActivation(SurvayDto dto)
        {
            try
            {
                var dbDto = GetSurvayById(dto.Id);
                if (dbDto != null)
                {                   
                    dbDto.IsActive = dto.IsActive;
                    dbDto.UpdatedBy = dto.UpdatedBy;
                    dbDto.UpdatedDate = DateTime.Now;

                    var entity = _mapper.Map<SurvayDto, Survay>(dbDto);
                    _unitOfWork.GetRepository<Survay>().Update(entity);                   

                    Save();

                }

                _log.Error($"Error : Survay is not existing in DB , survay details :  {dto.Id} {dto.Name}");
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public bool Exists(int id)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _unitOfWork.Save();
        }

        public int GetCount(string sortBy, string filetrBy)

        {
            try
            {
                return _unitOfWork.GetRepository<Survay>().GetCount(sortBy, filetrBy).Count();

            }
            catch (Exception e)
            {
                 _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }


        #region Implementing IDiosposable...

        #region private dispose variable declaration...

        private bool disposed;

        #endregion

        /// <summary>
        ///     Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    
                }
            }
            disposed = true;
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

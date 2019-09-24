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
   public class SurvayTypeService : ISurvayTypeService , IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _log;
        private readonly IFieldService _fieldService; 
        private List<string> IncludeList => new List<string>() { "Survay","Survay.SurvaySetting", "Fields", "Fields.FieldOption","Fields.FieldDependants" ,"Fields.FieldDependants1",
                                        "Fields.FieldOption.Options" , "Fields.FieldOption.MatrixHeaders" ,"Fields.FieldOption.MatrixRows" };

        public SurvayTypeService(IUnitOfWork unitOfWork, IMapper mapper, IFieldService service)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(SurvayService));
            _mapper = mapper;
            _fieldService = service;
        }
        public IEnumerable<SurvayTypeDto> GetSurvayTypes()
        {
            try
            {
                var entity =
                    _unitOfWork.GetRepository<SurvayType>().GetAll().OrderBy(x=>x.Id);

                var dto = _mapper.Map<List<SurvayType>, List<SurvayTypeDto>>(entity.ToList());
                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public IEnumerable<SurvayTypeDto> GetActiveSurvayTypes()
        {
            try
            {
                var entity = _unitOfWork.GetRepository<SurvayType>().GetMany(x => x.IsActive).OrderBy(x=>x.Id);
                var dto = _mapper.Map<List<SurvayType>, List<SurvayTypeDto>>(entity.ToList());
                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public SurvayTypeDto GetSurvayTypeById(int id)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<SurvayType>().GetByIdWithInclude(x => x.Id == id, IncludeList.ToArray());
                
                var dto = _mapper.Map<SurvayType, SurvayTypeDto>(entity.SingleOrDefault());

                var sortedFields = dto.Fields.OrderBy(x => x.OrderNo).ToList();
                dto.Fields.Clear();
                dto.Fields.AddRange(sortedFields);

                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void InsertSurvayType(SurvayTypeDto dto)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<SurvayType>();
                var entity = _mapper.Map<SurvayTypeDto, SurvayType>(dto);

                repo.Insert(entity);
                Save();
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
                _unitOfWork.GetRepository<SurvayType>().Delete(id);
                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void UpdateSurvay(SurvayTypeDto dto)
        {
            try
            {
                var dbDto = GetSurvayTypeById(dto.Id);
                if (dbDto != null)
                {
                    dbDto.Description = dto.Description;
                    dbDto.IsActive = dto.IsActive;
                    dbDto.UpdatedBy = dto.UpdatedBy;
                    dbDto.UpdatedDate = DateTime.Now;

                    var entity = _mapper.Map<SurvayTypeDto, SurvayType>(dbDto);
                    _unitOfWork.GetRepository<SurvayType>().Update(entity);
                    var fieldDtos = dto.Fields;
                    if (fieldDtos != null && fieldDtos.Any())
                    {
                        _fieldService.UpdateField(fieldDtos, dto.Id);
                    }

                    Save();

                }

                _log.Error($"Error : Survay is not existing in DB , survay details :  {dbDto.Id} {dbDto.Survay.Name}");
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

        private void Save()
        {
            _unitOfWork.Save();
        }

        public IEnumerable<SurvayTypeDto> GetSurvayTypesBySurvayId(int survayId)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<SurvayType>().GetManyWithInclude(x=>x.SurvayId == survayId ,IncludeList.ToArray());               
                var dtos = _mapper.Map<List<SurvayType>, List<SurvayTypeDto>>(entity.ToList());
                return dtos;
            } 
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public SurvayTypeDto GetSurvaysByCodeAndLanguage(int survayCode, int language)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<SurvayType>()
                    .GetSingle(x => x.SurvayId == survayCode && x.LanguageType == language, IncludeList.ToArray());
                var model  = _mapper.Map<SurvayType, SurvayTypeDto>(entity);
               if(model != null)
                {
                    var sortedFields = model.Fields.OrderBy(x => x.OrderNo).ToList();
                    model.Fields.Clear();
                    model.Fields.AddRange(sortedFields);
                    return model;
                }
                return model;
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

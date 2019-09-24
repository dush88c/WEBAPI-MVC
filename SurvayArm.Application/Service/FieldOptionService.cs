using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using log4net;
using SurvayArm.Application.Dto;
using SurvayArm.Application.IService;
using SurvayArm.Data.Model;
using SurvayArm.Data.UOW;

namespace SurvayArm.Application.Service
{
    public class FieldOptionService : IFieldOptionService ,IDisposable  
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _log; 
        private readonly IOptionService _optionService;
        private readonly IMatrixHeaderService _matrixHeaderService ;
        private readonly IMatrixRowService _matrixRowService;
        private List<string> IncludeList => new List<string>() { "Options", "MatrixHeaders", "MatrixRows" };

        public FieldOptionService(IUnitOfWork unitOfWork, IMapper mapper, IOptionService service , 
            IMatrixHeaderService matrixHeaderService , IMatrixRowService matrixRowService)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(FieldOptionService));
            _mapper = mapper;
            _optionService = service;
            _matrixHeaderService = matrixHeaderService;
            _matrixRowService = matrixRowService;
        }

        public FieldOptionDto GetFieldOptionByFieldId(int fieldId)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<FieldOption>().GetFirst(x=>x.FieldId == fieldId);
                var dto = _mapper.Map<FieldOption, FieldOptionDto>(entity);
                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }
        
        public void UpdateFieldOption(FieldOptionDto dto)
        {
            try
            {
                var fieldDto = GetFieldOptionByFieldId(dto.FieldId);

                if (fieldDto != null)
                {
                    fieldDto.Include_other_option = dto.Include_other_option;
                    fieldDto.Integer_only = dto.Integer_only;
                    fieldDto.Max = dto.Max;
                    fieldDto.Min = dto.Min;
                    fieldDto.Min_max_length_units = dto.Min_max_length_units;
                    fieldDto.Minlength = dto.Minlength;
                    fieldDto.Size = dto.Size;

                    var entity = _mapper.Map<FieldOptionDto, FieldOption>(fieldDto);
                    _unitOfWork.GetRepository<FieldOption>().Update(entity);

                    if (dto.Options != null && dto.Options.Any())
                    {
                        _optionService.UpdateOption(dto.Options ,dto.FieldId);
                    }
                    if (dto.MatrixHeaders != null && dto.MatrixHeaders.Any())
                    {
                        _matrixHeaderService.UpdateOption(dto.MatrixHeaders, dto.FieldId);
                    }
                    if (dto.MatrixRows != null && dto.MatrixRows.Any())
                    {
                        _matrixRowService.UpdateOption(dto.MatrixRows, dto.FieldId);
                    }

                    Save();
                }
                
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void Delete(int id)
        {
            try
            {
                _unitOfWork.GetRepository<FieldOption>().Delete(d => d.FieldId == id);
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

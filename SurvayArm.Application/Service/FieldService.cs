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
    public class FieldService : IFieldService ,IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _log;
        private readonly IFieldOptionService _fieldOptionService;
        private readonly IMatrixHeaderService _matrixHeaderService;
        private readonly IMatrixRowService _matrixRowService;
        private readonly IOptionService _optionsService;

        private List<string> IncludeList => new List<string>() { "FieldOption", "FieldOption.Options",
                                                                "FieldOption.MatrixHeaders", "FieldOption.MatrixRows" };
        public FieldService(IUnitOfWork unitOfWork, IMapper mapper , IFieldOptionService service ,
            IMatrixHeaderService matrixHeaderService , IMatrixRowService matrixRowService , IOptionService optionsService)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(FieldService));
            _mapper = mapper;
            _fieldOptionService = service;
            _matrixHeaderService = matrixHeaderService;
            _matrixRowService = matrixRowService;
            _optionsService = optionsService;
        }

        public IEnumerable<FieldDto> GetFieldsBySurvayId(int survayId)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<Field>().GetManyWithInclude(x => x.SurvayId == survayId );
                var dto = _mapper.Map<IEnumerable<Field>, IEnumerable<FieldDto>>(entity);
                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }
        
        public void UpdateField(IList<FieldDto> dto ,int survayId)
        {
            try
            {
                var fieldDto = GetFieldsBySurvayId(survayId);

                foreach (var field  in fieldDto)
                {
                    var f = dto.SingleOrDefault(x => x.Id == field.Id);
                    if (f != null)
                    {
                        field.Field_Type = f.Field_Type;
                        field.Label = f.Label;
                        field.Required = f.Required;
                        field.IsImageUpload = f.IsImageUpload;
                        field.IsVoiceUpload = f.IsVoiceUpload;
                        field.VideoUrl = f.VideoUrl;
                        field.OrderNo = f.OrderNo;
                        var entity  = _mapper.Map<FieldDto ,Field>(field);
                        _unitOfWork.GetRepository<Field>().Update(entity);

                        if (f.FieldOption != null)
                        {
                            _fieldOptionService.UpdateFieldOption(f.FieldOption);
                        }                       
                    }
                    else
                    {
                        _matrixHeaderService.DeleteHeadersByFieldOptionId(field.Id);
                        _matrixRowService.DeleteRowsByFieldOptionId(field.Id);
                        _optionsService.DeleteOPtionsByFieldOptionId(field.Id);
                        _fieldOptionService.Delete(field.Id);
                        _unitOfWork.GetRepository<Field>().Delete(field.Id);
                    }
                }

                foreach (var f in dto.Where(x=>x.Id == 0))
                {
                    f.SurvayId =  survayId;
                    var entity = _mapper.Map<FieldDto, Field>(f);
                    _unitOfWork.GetRepository<Field>().Insert(entity);

                }
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

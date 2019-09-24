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
    public class MatrixHeaderService : IMatrixHeaderService, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public MatrixHeaderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(MatrixHeaderService));
            _mapper = mapper;
        }

        public IList<MatrixHeaderDto> GetOptionByFieldOptionId(int fieldOptionId)
        {
            try
            {
                var entity = _unitOfWork.GetRepository<MatrixHeader>().GetMany(x => x.FieldOptionId == fieldOptionId);
                var dto = _mapper.Map<List<MatrixHeader>, List<MatrixHeaderDto>>(entity.ToList());
                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void UpdateOption(IList<MatrixHeaderDto> dto, int fieldOptionId)
        {
            try
            {
                var optionDtos = GetOptionByFieldOptionId(fieldOptionId);
                var repo = _unitOfWork.GetRepository<MatrixHeader>();
                foreach (var option in optionDtos)
                {
                    var f = dto.FirstOrDefault(x => x.Id == option.Id);
                    if (f != null)
                    {
                        option.Label = f.Label;
                        var entity = _mapper.Map<MatrixHeaderDto, MatrixHeader>(option);
                        repo.Update(entity);
                    }
                    else
                    {
                        repo.Delete(option.Id);
                    }
                }

                var newOptions = dto.Where(x => x.Id == 0);
                if(newOptions != null && newOptions.Any())
                {
                    var newEntities = newOptions.Select(x => new MatrixHeader()
                    {
                        FieldOptionId = fieldOptionId,
                        Label = x.Label
                    });

                    repo.AddorUpdate(newEntities);
                }

                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public bool DeleteHeadersByFieldOptionId(int fieldOptionId)
        {
            try
            {
                _unitOfWork.GetRepository<MatrixHeader>().Delete(x => x.FieldOptionId == fieldOptionId);
                Save();
                return true;
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

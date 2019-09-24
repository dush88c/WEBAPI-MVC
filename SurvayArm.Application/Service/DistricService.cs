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
   public class DistricService : IDistricService ,IDisposable
    {
        public readonly IUnitOfWork UnitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public DistricService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(SurvayService));
            _mapper = mapper;
           
        }

        public List<DistrictDto> GetActiveAll()
        {
            try
            {
                var entities = UnitOfWork.GetRepository<District>().GetMany(x=>x.IsActive);
                if (entities != null)
                {
                    var dtos = _mapper.Map<List<District>, List<DistrictDto>>(entities.ToList());
                    return dtos;
                }
                _log.Info($"INFO :  No Districs found");
                return null;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public List<DistrictDto> GetProvinceId(int provinceId)
        {
            try
            {
                var entities = UnitOfWork.GetRepository<District>().GetMany(x => x.ProvinceId == provinceId);
                if (entities != null)
                {
                    var dtos = _mapper.Map<List<District>, List<DistrictDto>>(entities.ToList());
                    return dtos;
                }
                _log.Info($"INFO :  No Districs found for province : {provinceId}");
                return null;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
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

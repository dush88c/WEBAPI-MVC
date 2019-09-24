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
   public class ProvinceService : IProvinceService ,IDisposable 
    {
        public readonly IUnitOfWork UnitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public ProvinceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(SurvayService));
            _mapper = mapper;
           
        }

        public List<ProvinceDto> GetActiveAll()
        {
            try
            {
                var entities = UnitOfWork.GetRepository<Province>().GetMany(x=>x.IsActive);
                if (entities != null)
                {
                    var dtos = _mapper.Map<List<Province>, List<ProvinceDto>>(entities.ToList());
                    return dtos;
                }
                _log.Info($"INFO :  No Provinces found");
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

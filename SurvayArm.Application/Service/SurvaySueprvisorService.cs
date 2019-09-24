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
    public class SurvaySueprvisorService : ISurvaySupervisorService, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        private List<string> IncludeList => new List<string>() { "AspNetUser"};

        public SurvaySueprvisorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(SurvaySueprvisorService));
            _mapper = mapper;

        }

        public List<SurvaySupervisorDto> GetSupervisorsBySurvayId(int survayId)
        {
            try
            {

                var survaySupervisors = _unitOfWork.GetRepository<SurvaySupervisor>()
                    .GetByIdWithInclude(x => x.SurvayId == survayId, IncludeList.ToArray());

                var dtos = _mapper.Map<List<SurvaySupervisor>, List<SurvaySupervisorDto>>(survaySupervisors.ToList());

                return dtos;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public void UpdateSurvaySupervisors(List<SurvaySupervisorDto> supervisors , int survayId)
        {

            try
            {
                _unitOfWork.GetRepository<SurvaySupervisor>().Delete(x=>x.SurvayId == survayId);

                var entities = _mapper.Map<List<SurvaySupervisorDto>, List<SurvaySupervisor>>(supervisors);
                _unitOfWork.GetRepository<SurvaySupervisor>().Insert(entities);
                Save();
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

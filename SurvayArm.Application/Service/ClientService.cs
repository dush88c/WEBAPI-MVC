using SurvayArm.Application.IService;
using System;
using System.Collections.Generic;
using SurvayArm.Application.Dto;
using SurvayArm.Data.UOW;
using AutoMapper;
using log4net;
using SurvayArm.Data.Model;
using System.Linq;

namespace SurvayArm.Application.Service
{
    public class ClientService : IClientService, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        public ClientService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(ClientService));
            _mapper = mapper;
        }

        public List<ClientDto> GetAll()
        {
            try
            {
                var entities = _unitOfWork.GetRepository<Client>().GetAll();
                var dtos = _mapper.Map<List<Client>, List<ClientDto>>(entities.ToList());
                return dtos;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }

        public int Insert(ClientDto dto)
        {
            try
            {
                var entity = _mapper.Map<ClientDto, Client>(dto);
                _unitOfWork.GetRepository<Client>().Insert(entity);
                Save();
                return entity.Id;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
            }
        }
        
        public List<ClientDto> Find(ClientDto criteria)
        {
            try
            {               
                if (criteria.Longitude.HasValue && criteria.Latitude.HasValue)
                {
                  
                    var constValue = 57.2957795130823D;
                    var constValue2 = 3958.75586574D;
                    
                    var latitude = Convert.ToDouble(criteria.Latitude);
                    var longitude = Convert.ToDouble(criteria.Longitude);
                    var nearestClients  = (from l in _unitOfWork.GetRepository<Client>().GetAll()
                               let temp = Math.Sin(Convert.ToDouble(l.Latitude) / constValue) * Math.Sin(Convert.ToDouble(latitude) / constValue) +
                                                                Math.Cos(Convert.ToDouble(l.Latitude) / constValue) *
                                                                Math.Cos(Convert.ToDouble(latitude) / constValue) *
                                                                Math.Cos((Convert.ToDouble(longitude) / constValue) - (Convert.ToDouble(l.Longitude) / constValue))
                               let calMiles = (constValue2 * Math.Acos(temp > 1 ? 1 : (temp < -1 ? -1 : temp)))
                               where (l.Latitude > 0 && l.Longitude > 0)
                               orderby calMiles
                               select new Client
                               {
                                   Id = l.Id,
                                   FirstName = l.FirstName,
                                   LastName = l.LastName,
                                   MobileNumber = l.MobileNumber,
                                   PhoneNumber = l.PhoneNumber,
                                   Email = l.Email,
                                   Address = l.Address,
                                   Latitude = l.Latitude,
                                   Longitude = l.Longitude,
                               });
                    

                    return _mapper.Map<List<Client>, List<ClientDto>>(nearestClients.ToList());

                }

              var  otherResults  =  _unitOfWork.GetRepository<Client>().GetManyQueryable(x => 
                                                        x.FirstName.Contains(criteria.FirstName) ||
                                                        x.LastName.Contains(criteria.LastName) ||
                                                        x.PhoneNumber.Contains(criteria.PhoneNumber) ||
                                                        x.MobileNumber.Contains(criteria.MobileNumber)).ToList();

                

                var dtos = _mapper.Map<List<Client>, List<ClientDto>>(otherResults);
                return dtos; 
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

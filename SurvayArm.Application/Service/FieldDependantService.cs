using SurvayArm.Application.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using SurvayArm.Application.Dto;
using SurvayArm.Data.UOW;
using AutoMapper;
using log4net;
using SurvayArm.Data.Model;

namespace SurvayArm.Application.Service
{
    public class FieldDependantService : IFieldDependantService, IDisposable
    {
        public readonly IUnitOfWork UnitOfWork;
        public readonly ISurvayService _SurvayService;
        private readonly ISurvayTypeService _survayTypeService;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        private List<string> IncludeList => new List<string>()
        {
            "Field", "Field.Survay"
        };

        public FieldDependantService(IUnitOfWork unitOfWork, IMapper mapper , ISurvayService survayService ,ISurvayTypeService survayTypeService)
        {
             UnitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(SurvayService));
            _mapper = mapper;
            _SurvayService = survayService;
            _survayTypeService = survayTypeService; 
        }

        public IEnumerable<FieldDependantDto> GetFieldDependantsWithInclude()
        {
            try
            {
                var entity =
                    UnitOfWork.GetRepository<FieldDependant>().GetAllWithInclude(IncludeList.ToArray()).OrderByDescending(x => x.UpdatedDate);

                var dto = _mapper.Map<List<FieldDependant>, List<FieldDependantDto>>(entity.ToList());
                return dto;
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw new Exception(e.Message);
               
            }
        }

        public void InsertFieldDependant(IEnumerable<FieldDependantDto> dtos, int survayId)
        {
            try
            {
                var repo = UnitOfWork.GetRepository<FieldDependant>();
                var entities  = _mapper.Map<List<FieldDependantDto>, List<FieldDependant>>(dtos.ToList());

                var survay  = _survayTypeService.GetSurvayTypeById(survayId);
                var dependants = new List<int>();
                foreach (var d in survay.Fields)
                {
                    if (d.FieldDependants == null) continue;
                    dependants.AddRange(d?.FieldDependants.Select(x => x.Id));
                    if (d.FieldDependants1 == null) continue;
                    dependants.AddRange(d?.FieldDependants1.Select(x => x.Id));
                }
                
                //deleting non-existing in data base
                foreach (var e in dependants)
                {
                    if(!entities.Any(x=>x.Id == e)){
                        repo.Delete(e);
                    }
                }

                repo.AddorUpdate(entities);                
                
                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw;

            }
        }

        public void Save()
        {
            try
            {
                UnitOfWork.Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw;
            }
        }

        public void DeleteFieldDependancyBySurvayId(int survayId)
        {
            try
            {
                var repo = UnitOfWork.GetRepository<FieldDependant>();
                var survay = _survayTypeService.GetSurvayTypeById(survayId);
                var dependants = new List<int>();
                foreach (var d in survay.Fields)
                {
                    if (d.FieldDependants == null) continue;
                    dependants.AddRange(d?.FieldDependants.Select(x => x.Id));
                }
                foreach (var e in dependants)
                {
                        repo.Delete(e);
                }

                Save();
            }
            catch (Exception e)
            {
                _log.Error($"Error :  {e}");
                throw;
            }
        }

        public void UpdateFieldDependant(IEnumerable<FieldDependantDto> dtos)
        {
            try
            {
                var repo = UnitOfWork.GetRepository<FieldDependant>();
                var entities = _mapper.Map<List<FieldDependantDto>, List<FieldDependant>>(dtos.ToList());
                foreach (var entity in entities)
                {
                    repo.Update(entity);
                }

                Save();

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

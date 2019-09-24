using SurvayArm.Data.Model;
using SurvayArm.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvayArm.Data.UOW
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly SurvayArmDbContext _context; 
      
        public UnitOfWork()
        {
            _context = _context ?? new SurvayArmDbContext() ;
            _context.Configuration.LazyLoadingEnabled = false;
            _context.Configuration.ProxyCreationEnabled = false;
            _context.Configuration.ValidateOnSaveEnabled = false;
        }
        
        public void Save()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {

            var repository = new Repository<TEntity>(_context);

            return repository;
        }


        #region Implementing IDiosposable...

        #region private dispose variable declaration...
        private bool disposed = false;
        #endregion

        /// <summary>
        /// Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();

                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        #endregion
    }
}

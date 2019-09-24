using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.Migrations;
using SurvayArm.Utility;
using System.Linq.Dynamic;
using SurvayArm.Data.Model;

namespace SurvayArm.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class
    {
        private readonly SurvayArmDbContext _context;
        public DbSet<TEntity> DbSet { get; set; }

        public Repository(SurvayArmDbContext context) 
        {
            _context = context ?? new SurvayArmDbContext();
            DbSet = _context.Set<TEntity>();
        }


        /// <summary>
        ///     generic Insert method for the entities
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(TEntity entity)
        {
            try
            {
                DbSet.Add(entity);
                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     generic bulk Insert method for the entities
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(IEnumerable<TEntity> entity)
        {
            try
            {
                //using (var ctx = new BhoomitechDbContext())
                //{
                //    using (var ts = new TransactionScope())
                //    {
                //        // some stuff in _context

                //        ctx.BulkInsert(entity);

                //        ctx.SaveChanges();
                //        ts.Complete();
                //    }
                //}
                foreach (var updatableentity in entity)
                {
                    DbSet.AddOrUpdate(updatableentity);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     generic get method , fetches data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> Get()
        {
            try
            {
                IQueryable<TEntity> query = DbSet;
                var result = query.ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     generic get method , fetches data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public TEntity Get(Func<TEntity, bool> where)
        {
            try
            {
                return DbSet.Where(where).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     generic method to fetch all the records from db
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                var result = DbSet.ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public IQueryable<TEntity> GetCount(string sortBy, string filetrBy)
        {
            try
            {
                var dynamic = DbSet.AsQueryable();

                dynamic = !string.IsNullOrEmpty(filetrBy) ? dynamic.CallContainOf(filetrBy) : dynamic;

                dynamic = !string.IsNullOrEmpty(sortBy) ? dynamic?.OrderBy(sortBy) : dynamic?.OrderBy("UpdatedDate DESC");

                return dynamic;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     Generic get method on the basis of id for Entities.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public TEntity GetById(object id)
        {
            try
            {
                return DbSet.Find(id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public TEntity GetFirst(Func<TEntity, bool> predicate)
        {
            try
            {
                return DbSet.First(predicate);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     Check whether object exists or not by given criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool CheckWithSpecificValue(Func<TEntity, bool> predicate)
        {
            try
            {
                return DbSet.Any(predicate);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///  Get by id with include
        /// </summary>
        /// <param name="id"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetByIdWithInclude(Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            try
            {
                var dynamic = DbSet.AsQueryable();

                dynamic = include?.Aggregate(dynamic, (current, s) => current.Include(s));

                dynamic = dynamic?.Where(predicate);

                return dynamic;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     generic method to get many record on the basis of a condition.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return DbSet.Where(predicate).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///  Get by id with include
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetManyWithInclude(Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            try
            {
                var dynamic = DbSet.AsQueryable();

                dynamic = include?.Aggregate(dynamic, (current, s) => current.Include(s));

                dynamic = dynamic?.Where(predicate);

                return dynamic;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     generic method to get many record on the basis of a condition but query able.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetManyQueryable(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return DbSet.Where(predicate).AsQueryable();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        ///     Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <param name="include"></param>
        /// <returns>A single record that matches the specified criteria</returns>
        public TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            try
            {
                var dynamic = DbSet.AsQueryable();

                dynamic = include?.Aggregate(dynamic, (current, s) => current.Include(s));

                return dynamic?.SingleOrDefault(predicate);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     Generic method to check if entity exists
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public bool Exists(object primaryKey)
        {
            try
            {
                return DbSet.Find(primaryKey) != null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     Generic update method for the entities
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public void Update(TEntity entityToUpdate)
        {
            try
            {
                _context.Set<TEntity>().AddOrUpdate(entityToUpdate);
                //DbSet.Attach(entityToUpdate);
                //_dbContext.Entry(entityToUpdate).State = EntityState.Modified;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     Generic update method for the entities
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public void Update(IEnumerable<TEntity> entityToUpdate)
        {
            try
            {
                foreach (var updatableentity in entityToUpdate)
                {
                    DbSet.AddOrUpdate(updatableentity);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void AddorUpdate(IEnumerable<TEntity> list)
        {
            try
            {
                foreach (var updatableentity in list)
                {
                    DbSet.AddOrUpdate(updatableentity);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        ///     generic delete method , deletes data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public void Delete(TEntity entityToDelete)
        {
            try
            {
                if (_context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    DbSet.Attach(entityToDelete);
                }
                DbSet.Remove(entityToDelete);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Generic delete range method , deletes data with all relation object.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public void DeleteRange(IEnumerable<TEntity> entityToDelete)
        {
            try
            {
                DbSet.RemoveRange(entityToDelete);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        ///     generic delete method , deletes data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public void Delete(Func<TEntity, bool> where)
        {
            try
            {
                var objects = DbSet.Where(where).AsQueryable();
                foreach (var obj in objects)
                    DbSet.Remove(obj);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///     Generic Delete method for the entities
        /// </summary>
        /// <param name="id"></param>
        public void Delete(object id)
        {
            try
            {
                var entityToDelete = DbSet.Find(id);
                Delete(entityToDelete);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        ///     Include multiple entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="filetrBy"></param>
        /// <param name="pageNo"></param>
        /// <param name="sortBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetWithInclude(Expression<Func<TEntity, bool>> predicate,
                    string sortBy, string filetrBy, int skip, int pageSize = 12, params string[] include)
        {
            try
            {
                var dynamic = DbSet.AsQueryable();

                dynamic = include?.Aggregate(dynamic, (current, s) => current.Include(s));

                dynamic = !string.IsNullOrEmpty(filetrBy) ? dynamic.CallContainOf(filetrBy) : dynamic;

                dynamic = !string.IsNullOrEmpty(sortBy) ? dynamic?.OrderBy(sortBy) : dynamic?.OrderBy("UpdatedDate DESC");

                dynamic = dynamic?.Skip(skip).Take(pageSize);

                if (predicate != null) dynamic = dynamic?.Where(predicate);

                return dynamic;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public IQueryable<TEntity> GetAllWithInclude(params string[] include)
        {
            try
            {
                var dynamic = DbSet.AsQueryable();

                dynamic = include?.Aggregate(dynamic, (current, s) => current.Include(s));

                return dynamic;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public IQueryable<TEntity> GetAllPaginatedWithInclude(string sortBy, string filetrBy, int skip, int pageSize = 12, params string[] include) 
        {
            try
            {
                var dynamic = DbSet.AsQueryable();

                dynamic = include?.Aggregate(dynamic, (current, s) => current.Include(s));

                dynamic = !string.IsNullOrEmpty(filetrBy) ? dynamic.CallContainOf(filetrBy) : dynamic; 

                dynamic = !string.IsNullOrEmpty(sortBy) ? dynamic?.OrderBy(sortBy) : dynamic?.OrderBy("UpdatedDate DESC");

                dynamic = dynamic?.Skip(skip).Take(pageSize);                

                return dynamic;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public IQueryable<TEntity> GetQuerybleObject(params string[] include)
        {
            var dynamic = DbSet.AsQueryable();

            dynamic = include?.Aggregate(dynamic, (current, s) => current.Include(s));
            return dynamic;
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

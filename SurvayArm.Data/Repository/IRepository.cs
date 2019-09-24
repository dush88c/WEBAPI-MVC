using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SurvayArm.Data.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {

        /// <summary>
        /// generic Get method for Entities
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> Get();


        /// <summary>
        /// Generic get method on the basis of id for Entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById(object id);

        /// <summary>
        /// generic Insert method for the entities
        /// </summary>
        /// <param name="entity"></param>
        void Insert(TEntity entity);

        /// <summary>
        /// generic  bulk Insert method for the entities
        /// </summary>
        /// <param name="entity"></param>
        void Insert(IEnumerable<TEntity> entity);


        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="id"></param>
        void Delete(object id);


        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="entityToDelete"></param>
        void Delete(TEntity entityToDelete);


        /// <summary>
        /// Generic Delete method for the entities with their child objects
        /// </summary>
        /// <param name="entityToDelete"></param>
        void DeleteRange(IEnumerable<TEntity> entityToDelete);

        /// <summary>
        /// Generic update method for the entities
        /// </summary>
        /// <param name="entityToUpdate"></param>
        void Update(TEntity entityToUpdate);

        /// <summary>
        /// Generic bulk update method for the entities
        /// </summary>
        /// <param name="entityToUpdate"></param>
        void Update(IEnumerable<TEntity> entityToUpdate);


        /// <summary>
        /// Generic bulk add or update method for the entities
        /// </summary>
        /// <param name="entityToUpdate"></param>
        void AddorUpdate(IEnumerable<TEntity> list);


        /// <summary>
        /// generic method to get many record on the basis of a condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> predicate);


        /// <summary>
        /// generic method to get many record on the basis of a condition but query able.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetManyQueryable(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///  generic method to get many record on the basis of a condition but query able with includes
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetManyWithInclude(Expression<Func<TEntity, bool>> predicate, params string[] include);


        /// <summary>
        /// generic get method , fetches data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        TEntity Get(Func<TEntity, Boolean> where);


        /// <summary>
        /// generic delete method , deletes data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        void Delete(Func<TEntity, Boolean> where);


        /// <summary>
        /// generic method to fetch all the records from db
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// generic method to fetch records count from db
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetCount(string sortBy, string filetrBy);

        /// <summary>
        /// Inclue multiple
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="filetrBy"></param>
        /// <param name="pageNo"></param>
        /// <param name="sortBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetWithInclude(Expression<Func<TEntity, bool>> predicate, string sortBy, string filetrBy, int pageNo, int pageSize = 12, params string[] include);

        /// <summary>
        /// Get all with includes
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetAllWithInclude(params string[] include);

        /// <summary>
        /// Get all with paginated and includes
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="filetrBy"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetAllPaginatedWithInclude(string sortBy, string filetrBy, int pageNo, int pageSize = 12, params string[] include);


        /// <summary>
        /// Generic method to check if entity exists
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        bool Exists(object primaryKey);


        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <param name="include"></param>
        /// <returns>A single record that matches the specified criteria</returns>
        TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, params string[] include);


        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        TEntity GetFirst(Func<TEntity, bool> predicate);

        /// <summary>
        /// Check  whether object exists or not  by given string value
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool CheckWithSpecificValue(Func<TEntity, bool> predicate);

        /// <summary>
        /// Get by id with includes
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetByIdWithInclude(Expression<Func<TEntity, bool>> predicate, params string[] include);

        /// <summary>
        /// Get IQueryble object with include if supplied
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetQuerybleObject(params string[] include);   
    }
}

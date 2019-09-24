using SurvayArm.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvayArm.Data.UOW
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Save Context changes
        /// </summary>
        void Save();

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}

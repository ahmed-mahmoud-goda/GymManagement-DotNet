using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Entities;

namespace GymManagementDAL.Data.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool isTrackable = false);
        Task<IEnumerable<TEntity>> GetAllAsync(Specification<TEntity> specifications);
        Task<TEntity?> GetByIdAsync(int id);
        Task<TEntity?> GetBySpecificationAsync(Specification<TEntity> specifications);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        Task<int> CountAsync(Specification<TEntity>? specifications = null);
    }
}

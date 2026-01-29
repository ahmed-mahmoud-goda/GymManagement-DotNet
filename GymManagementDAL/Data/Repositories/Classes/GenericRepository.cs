using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymManagementDAL.Data.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly GymDbContext _context;

        public GenericRepository(GymDbContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> ApplyQuery(Specification<TEntity> specifications)
            => SpecificationEvaluator.GetQuery(_context.Set<TEntity>(), specifications);

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool isTrackable = false)
        {
            if (isTrackable)
                return await _context.Set<TEntity>().ToListAsync();

            return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(Specification<TEntity> specifications)
            => await ApplyQuery(specifications).ToListAsync();

        public async Task<TEntity?> GetByIdAsync(int id)
            => await _context.Set<TEntity>().FindAsync(id);
        public async Task<TEntity?> GetBySpecificationAsync(Specification<TEntity> specifications)
            => await ApplyQuery(specifications).FirstOrDefaultAsync();

        public async Task AddAsync(TEntity entity) => await _context.Set<TEntity>().AddAsync(entity);
        public void Update(TEntity entity) => _context.Set<TEntity>().Update(entity);
        public void Delete(TEntity entity) => _context.Set<TEntity>().Remove(entity);
        public void DeleteRange(IEnumerable<TEntity> entities) => _context.Set<TEntity>().RemoveRange(entities);

        public async Task<int> CountAsync(Specification<TEntity>? specifications = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (specifications != null)
            {
                query = SpecificationEvaluator.GetQuery(query, specifications);
            }

            return await query.CountAsync();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementDAL.Data.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _context;
        private readonly Dictionary<string, object> repositories = [];

        public UnitOfWork(GymDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            var entityName = typeof(TEntity).Name;

            if(repositories.TryGetValue(entityName, out object? value))
                return (IGenericRepository<TEntity>) value;

            var repository = new GenericRepository<TEntity>(_context);

            repositories.Add(entityName, repository);

            return repository;
        }

        public int SaveChanges() => _context.SaveChanges();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagementDAL.Data.Repositories.Classes
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>(IQueryable<T> baseQuery, Specification<T> specifications) where T : class
        {
            var query = baseQuery;

            if (specifications.Criteria != null)
                query = query.Where(specifications.Criteria);

            query = specifications.Includes.Aggregate(query, (currentQuery, include) => currentQuery.Include(include));

            if (specifications.OrderBy != null)
                query = query.OrderBy(specifications.OrderBy);

            else if (specifications.OrderByDescending != null)
                query = query.OrderByDescending(specifications.OrderByDescending);

            return query;
        }
    }
}

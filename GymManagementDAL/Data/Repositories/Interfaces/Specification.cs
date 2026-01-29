using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Data.Repositories.Interfaces
{
    public abstract class Specification<T> where T : class
    {
        protected Specification()
        {

        }
        protected Specification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<T, bool>> Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public Expression<Func<T, object>> OrderBy { get; private set; } = null!;
        public Expression<Func<T, object>> OrderByDescending { get; private set; } = null!;

        protected void AddInclude(Expression<Func<T, object>> include)
            => Includes.Add(include);

        protected void setOrderBy(Expression<Func<T, object>> expression)
            => OrderBy = expression;

        protected void setOrderByDescending(Expression<Func<T, object>> expression)
            => OrderByDescending = expression;

    }
}

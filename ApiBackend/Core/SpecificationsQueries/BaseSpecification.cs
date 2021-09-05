using Core.Interfaces.AppService.RepositoriesAndPatterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.SpecificationsQueries
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public BaseSpecification()
        {
        }

        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>(); // to add ThenInclude 

        public Expression<Func<T, object>> OrderBy { get; private set; }

        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        public Expression<Func<T, object>> ThenOrderBy { get; private set; }

        public Expression<Func<T, object>> ThenOrderByDescending { get; private set; }

        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPagingEnabled { get; set; }

        public bool AsNoTracking { get; private set; }
        protected void AddAsNoTracking(bool tracking = false)
        {
            AsNoTracking = tracking;
        }

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected void AddOrderBy(Expression<Func<T, Object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending(Expression<Func<T, Object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }

        protected void AddThenOrderBy(Expression<Func<T, Object>> thenOrderByExpression)
        {
            ThenOrderBy = thenOrderByExpression;
        }
        protected void AddThenOrderByDescending(Expression<Func<T, Object>> thenOrderByDescExpression)
        {
            ThenOrderByDescending = thenOrderByDescExpression;
        }

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
    }
}

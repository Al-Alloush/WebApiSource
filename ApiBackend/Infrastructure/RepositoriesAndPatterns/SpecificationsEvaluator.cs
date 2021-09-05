using Core.Interfaces.AppService.RepositoriesAndPatterns;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.RepositoriesAndPatterns
{
    public class SpecificationsEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            var query = inputQuery;
            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            if (spec.OrderBy != null)
            {
                if (spec.ThenOrderBy != null)
                    query.OrderBy(spec.OrderBy).ThenBy(spec.ThenOrderBy);
                else if (spec.ThenOrderByDescending != null)
                    query.OrderBy(spec.OrderBy).ThenByDescending(spec.ThenOrderByDescending);
                else
                    query = query.OrderBy(spec.OrderBy);
            }

            if (spec.OrderByDescending != null)
            {
                if (spec.ThenOrderBy != null)
                    query.OrderByDescending(spec.OrderBy).ThenBy(spec.ThenOrderBy);
                else if (spec.ThenOrderByDescending != null)
                    query.OrderByDescending(spec.OrderBy).ThenByDescending(spec.ThenOrderByDescending);
                else
                    query = query.OrderByDescending(spec.OrderByDescending);
            }


            if (spec.IsPagingEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            if (spec.AsNoTracking == true)
                query = query.AsNoTracking();

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            // used for add then include
            if (spec.IncludeStrings != null)
                query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}

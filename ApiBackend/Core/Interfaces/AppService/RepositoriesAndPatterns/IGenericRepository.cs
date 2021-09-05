using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.AppService.RepositoriesAndPatterns
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

        Task<T> LastEntityAsync(ISpecification<T> spec);

        Task<T> EntityAsync(ISpecification<T> spec);

        Task<int> CountAsync(ISpecification<T> spec);

        Task<bool> AddAsync(T model);

        bool Update(T model);

        bool Remove(T model);

        /// <summary>
        /// Update database with all changes
        /// </summary>
        /// <returns> true if success, else flase</returns>
        /// <exception cref="Exception(string?)"></exception>
        /// 
        Task<bool> SaveChangesAsync();


    }
}

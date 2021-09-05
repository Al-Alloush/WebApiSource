using Core.Interfaces.AppService.RepositoriesAndPatterns;
using Infrastructure.DataApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.RepositoriesAndPatterns
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly string _errorNote = $"***************\n*******\n****\nError in IGenericRepository<{typeof(T)}>: ";
        private readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }




        public async Task<T> EntityAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<T> LastEntityAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).LastOrDefaultAsync();
        }


        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }


        public async Task<bool> AddAsync(T model)
        {
            try
            {
                EntityEntry<T> result = await _context.Set<T>().AddAsync(model);
                if (result.State.ToString() == "Added")
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(_errorNote + ex.Message);
                return false;
            }
        }

        public bool Update(T model)
        {
            try
            {
                _context.Set<T>().Attach(model);
                _context.Entry(model).State = EntityState.Modified;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(_errorNote + ex.Message);
                return false;
            }
        }

        public bool Remove(T model)
        {
            try
            {
                EntityEntry<T> result = _context.Remove(model);
                if (result.State.ToString() == "Deleted")
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(_errorNote + ex.Message);
                return false;
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }


    }
}

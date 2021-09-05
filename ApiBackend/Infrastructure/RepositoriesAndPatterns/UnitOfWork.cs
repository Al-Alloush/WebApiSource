using Core.Interfaces.AppService.RepositoriesAndPatterns;
using Infrastructure.DataApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.RepositoriesAndPatterns
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // when create a new instance of AppDbContext and any repositories (one or many repositories)
        // that use all repositories inside this unitOfWwork are going to be stored inside this hash table.
        private Hashtable _hashRepositories;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_hashRepositories == null) _hashRepositories = new Hashtable();

            // get the name of the entity
            var type = typeof(T).Name;

            // check if Hash table contain the an entity with this specific entity name
            if (!_hashRepositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);

                // create instance of current Entity(class).
                // creating an instance of _context, when we create a new repository, it's going to be passing in the context to our UnitOfWork as parameter
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
                // add a new Repositoryx instance in Hash table
                _hashRepositories.Add(type, repositoryInstance);
            }
            // return the repository after query inside _hashRepositories
            return (IGenericRepository<T>)_hashRepositories[type];
        }
    }
}

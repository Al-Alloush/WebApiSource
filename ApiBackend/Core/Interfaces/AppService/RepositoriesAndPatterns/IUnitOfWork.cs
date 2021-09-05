using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.AppService.RepositoriesAndPatterns
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;

        // return the number of changes to database
        // Entityframework is going to track all of the changes to the entities like: Add, remove, .. inside this UnitOfWork
        // when run this methode save all changes to database and return a number of changes.
        Task<int> Complete();
    }
}

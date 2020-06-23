using System;

namespace Wiz.Biblioteca.Domain.Interfaces.Repository
{
    public interface IEntityBaseRepository<TEntity> : IDisposable where TEntity : class
    {
        void Add(TEntity obj);
        void Update(TEntity obj);
        void Remove(TEntity obj);
    }
}

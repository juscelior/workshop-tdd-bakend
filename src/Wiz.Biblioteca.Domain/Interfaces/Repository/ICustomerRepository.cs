using System.Collections.Generic;
using System.Threading.Tasks;
using Wiz.Biblioteca.Domain.Models;
using Wiz.Biblioteca.Domain.Models.Dapper;

namespace Wiz.Biblioteca.Domain.Interfaces.Repository
{
    public interface ICustomerRepository : IEntityBaseRepository<Customer>, IDapperReadRepository<Customer>
    {
        Task<IEnumerable<CustomerAddress>> GetAllAsync();
        Task<CustomerAddress> GetAddressByIdAsync(int id);
        Task<CustomerAddress> GetByNameAsync(string name);
    }
}

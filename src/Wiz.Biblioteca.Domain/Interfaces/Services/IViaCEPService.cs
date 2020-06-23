using System.Threading.Tasks;
using Wiz.Biblioteca.Domain.Models.Services;

namespace Wiz.Biblioteca.Domain.Interfaces.Services
{
    public interface IViaCEPService
    {
        Task<ViaCEP> GetByCEPAsync(string cep);
    }
}

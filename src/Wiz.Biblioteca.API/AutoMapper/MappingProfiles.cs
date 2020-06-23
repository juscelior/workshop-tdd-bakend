using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using Wiz.Biblioteca.API.ViewModels.Customer;
using Wiz.Biblioteca.Domain.Models;
using Wiz.Biblioteca.Domain.Models.Dapper;

namespace Wiz.Biblioteca.API.AutoMapper
{
    [ExcludeFromCodeCoverage]
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            #region Customer

            CreateMap<CustomerAddress, CustomerAddressViewModel>();
            CreateMap<Customer, CustomerViewModel>().ReverseMap();

            #endregion
        }
    }
}

using Bogus;
using Wiz.Biblioteca.Domain.Models.Services;

namespace Wiz.Biblioteca.Unit.Tests.Mocks
{
    public static class ViaCEPMock
    {
        public static Faker<ViaCEP> ViaCEPModelFaker =>
            new Faker<ViaCEP>()
            .CustomInstantiator(x => new ViaCEP
            (
                cep: x.Address.ZipCode(),
                street: x.Address.StreetName(),
                streetFull: x.Address.StreetAddress(),
                uf: x.Address.CountryCode(Bogus.DataSets.Iso3166Format.Alpha2)
            ));
    }
}

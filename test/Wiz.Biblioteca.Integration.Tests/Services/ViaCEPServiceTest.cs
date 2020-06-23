using Moq;
using Moq.Protected;
using RA;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Wiz.Biblioteca.Domain.Models.Services;
using Wiz.Biblioteca.Infra.Services;
using Wiz.Biblioteca.Integration.Tests.Mocks;
using Xunit;

namespace Wiz.Biblioteca.Integration.Tests.Services
{
    public class ViaCEPServiceTest
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

        public ViaCEPServiceTest()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        }

        [Fact]
        public async Task GetByCEPAsync_ReturnViaCEPModelTestAsync()
        {
            var httpClientMock = "https://viacep.com.br/ws/";
            var cep = "17052520";

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(ViaCEPMock.ViaCEPModelFaker.Generate()))
                });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri(httpClientMock)
            };

            var viaCEPservice = new ViaCEPService(httpClient);
            var viaCEPMethod = await viaCEPservice.GetByCEPAsync(cep);

            var serviceResult = Assert.IsType<ViaCEP>(viaCEPMethod);

            Assert.NotNull(serviceResult);
            Assert.NotNull(serviceResult.CEP);
        }

        [Fact]
        public async Task GetByCEPAsync_ReturnViaCEPModelTestAsync_RestAssured()
        {
            var httpClientMock = "https://viacep.com.br/ws/";
            var cep = "17052520";

           new RestAssured()
            .Given()
                //Optional, set the name of this suite
                .Name("VIA Cep Teste")
                //Optional, set the header parameters.  
                //Defaults will be set to application/json if none is given
                .Header("Content-Type", "text/html; charset=utf-8")
                .Header("Accept-Encoding", "gzip,deflate")
            .When()
                //url
                .Get($"{httpClientMock}{cep}/json/")
            .Then()
                .TestStatus("test status", x => x == 200)
                //Give the name of the test and a lambda expression to test with
                //The lambda expression keys off of 'x' which represents the json blob as a dynamic.
                .TestBody("test cep", x => x.CEP != null)
                //Throw an AssertException if the test case is false.
                .Assert("17052-520");
        }
    }
}

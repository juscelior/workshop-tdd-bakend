﻿using Moq;
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

        [Theory]
        [InlineData("17052520")]
        [InlineData("17052-520")]
        public void GetByCEPAsync_ReturnViaCEPModelTestAsync_RestAssured(string cep)
        {
            var httpClientMock = "https://viacep.com.br/ws/";

            new RestAssured()
             .Given()
                 .Name("VIA Cep Teste")
                 .Header("Content-Type", "text/html; charset=utf-8")
                 .Header("Accept-Encoding", "gzip,deflate")
             .When()
                 .Get($"{httpClientMock}{cep}/json/")
             .Then()
                 //.Debug()
                 .TestStatus("test status", x => x == 200)
                 .TestBody("test cep", x => x.cep != null)
                 .TestElaspedTime("test response time", x => x < 1000)
                 .WriteAssertions()
                 .AssertAll();
        }

        [Theory]
        [InlineData("17052520")]
        [InlineData("17052-520")]
        public void GetByCEPAsync_Test_Schema(string cep)
        {
            var httpClientMock = "https://viacep.com.br/ws/";

            new RestAssured()
             .Given()
                 .Name("VIA Cep Teste")
                 .Header("Content-Type", "text/html; charset=utf-8")
                 .Header("Accept-Encoding", "gzip,deflate")
             .When()
                 .Get($"{httpClientMock}{cep}/json/")
             .Then()
                 //.Debug()
                 .Schema(@"{
                    ""$schema"": ""http://json-schema.org/draft-07/schema"",
                    ""$id"": ""http://example.com/example.json"",
                    ""type"": ""object"",
                    ""title"": ""The root schema"",
                    ""description"": ""The root schema comprises the entire JSON document."",
                    ""default"": {},
                    ""examples"": [
                        {
                            ""cep"": ""17052-520"",
                            ""logradouro"": ""Rua Moacyr Teixeira"",
                            ""complemento"": ""até Quadra 8"",
                            ""bairro"": ""Vila Nova Paulista"",
                            ""localidade"": ""Bauru"",
                            ""uf"": ""SP"",
                            ""unidade"": """",
                            ""ibge"": ""3506003"",
                            ""gia"": ""2094""
                        }
                    ],
                    ""required"": [
                        ""cep"",
                        ""logradouro"",
                        ""complemento"",
                        ""bairro"",
                        ""localidade"",
                        ""uf"",
                        ""unidade"",
                        ""ibge"",
                        ""gia""
                    ],
                    ""additionalProperties"": true,
                    ""properties"": {
                        ""cep"": {
                            ""$id"": ""#/properties/cep"",
                            ""type"": ""string"",
                            ""title"": ""The cep schema"",
                            ""description"": ""An explanation about the purpose of this instance."",
                            ""default"": """",
                            ""examples"": [
                                ""17052-520""
                            ]
                        },
                        ""logradouro"": {
                            ""$id"": ""#/properties/logradouro"",
                            ""type"": ""string"",
                            ""title"": ""The logradouro schema"",
                            ""description"": ""An explanation about the purpose of this instance."",
                            ""default"": """",
                            ""examples"": [
                                ""Rua Moacyr Teixeira""
                            ]
                        },
                        ""complemento"": {
                            ""$id"": ""#/properties/complemento"",
                            ""type"": ""string"",
                            ""title"": ""The complemento schema"",
                            ""description"": ""An explanation about the purpose of this instance."",
                            ""default"": """",
                            ""examples"": [
                                ""até Quadra 8""
                            ]
                        },
                        ""bairro"": {
                            ""$id"": ""#/properties/bairro"",
                            ""type"": ""string"",
                            ""title"": ""The bairro schema"",
                            ""description"": ""An explanation about the purpose of this instance."",
                            ""default"": """",
                            ""examples"": [
                                ""Vila Nova Paulista""
                            ]
                        },
                        ""localidade"": {
                            ""$id"": ""#/properties/localidade"",
                            ""type"": ""string"",
                            ""title"": ""The localidade schema"",
                            ""description"": ""An explanation about the purpose of this instance."",
                            ""default"": """",
                            ""examples"": [
                                ""Bauru""
                            ]
                        },
                        ""uf"": {
                            ""$id"": ""#/properties/uf"",
                            ""type"": ""string"",
                            ""title"": ""The uf schema"",
                            ""description"": ""An explanation about the purpose of this instance."",
                            ""default"": """",
                            ""examples"": [
                                ""SP""
                            ]
                        },
                        ""unidade"": {
                            ""$id"": ""#/properties/unidade"",
                            ""type"": ""string"",
                            ""title"": ""The unidade schema"",
                            ""description"": ""An explanation about the purpose of this instance."",
                            ""default"": """",
                            ""examples"": [
                                """"
                            ]
                        },
                        ""ibge"": {
                            ""$id"": ""#/properties/ibge"",
                            ""type"": ""string"",
                            ""title"": ""The ibge schema"",
                            ""description"": ""An explanation about the purpose of this instance."",
                            ""default"": """",
                            ""examples"": [
                                ""3506003""
                            ]
                        },
                        ""gia"": {
                            ""$id"": ""#/properties/gia"",
                            ""type"": ""string"",
                            ""title"": ""The gia schema"",
                            ""description"": ""An explanation about the purpose of this instance."",
                            ""default"": """",
                            ""examples"": [
                                ""2094""
                            ]
                        }
                    }
                }")
                 .WriteAssertions()
                 .AssertSchema();
        }
    }
}

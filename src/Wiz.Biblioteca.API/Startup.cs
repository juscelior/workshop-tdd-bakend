﻿using AutoMapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Net.Http.Headers;
using NSwag;
using NSwag.SwaggerGeneration.Processors.Security;
using Polly;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Wiz.Biblioteca.API.Extensions;
using Wiz.Biblioteca.API.Filters;
using Wiz.Biblioteca.API.Middlewares;
using Wiz.Biblioteca.API.Services;
using Wiz.Biblioteca.API.Services.Interfaces;
using Wiz.Biblioteca.API.Settings;
using Wiz.Biblioteca.API.Swagger;
using Wiz.Biblioteca.Domain.Interfaces.Identity;
using Wiz.Biblioteca.Domain.Interfaces.Notifications;
using Wiz.Biblioteca.Domain.Interfaces.Repository;
using Wiz.Biblioteca.Domain.Interfaces.Services;
using Wiz.Biblioteca.Domain.Interfaces.UoW;
using Wiz.Biblioteca.Domain.Notifications;
using Wiz.Biblioteca.Infra.Context;
using Wiz.Biblioteca.Infra.Identity;
using Wiz.Biblioteca.Infra.Repository;
using Wiz.Biblioteca.Infra.Services;
using Wiz.Biblioteca.Infra.UoW;

[assembly: ApiConventionType(typeof(MyApiConventions))]
namespace Wiz.Biblioteca.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc(options =>
            {
                options.Filters.Add<DomainNotificationFilter>();
                options.EnableEndpointRouting = false;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = Configuration["WizID:Authority"];
                options.Audience = Configuration["WizID:Audience"];
                options.RequireHttpsMetadata = false;
                options.Events = new JwtBearerEvents
                {
                    //Remover warning caso há alguma validação do token assíncrona (async/await)
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                    OnTokenValidated = async ctx =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                    {
                        //Exemplo para recuperar informações do token JWT e utilizar no serviço: IIdentityService
                        var jwtClaimScope = ctx.Principal.Claims.FirstOrDefault(x => x.Type == "scope")?.Value;

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.System, jwtClaimScope),
                            new Claim(ClaimTypes.Authentication, ((JwtSecurityToken)ctx.SecurityToken).RawData)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims);
                        ctx.Principal.AddIdentity(claimsIdentity);
                        ctx.Success();
                    }
                };
            });

            services.Configure<GzipCompressionProviderOptions>(x => x.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(x =>
            {
                x.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddHttpClient<IViaCEPService, ViaCEPService>((s, c) =>
            {
                c.BaseAddress = new Uri(Configuration["API:ViaCEP"]);
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.OrResult(response =>
                    (int)response.StatusCode == (int)HttpStatusCode.InternalServerError)
              .WaitAndRetryAsync(3, retry =>
                   TimeSpan.FromSeconds(Math.Pow(2, retry)) +
                   TimeSpan.FromMilliseconds(new Random().Next(0, 100))))
              .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.CircuitBreakerAsync(
                   handledEventsAllowedBeforeBreaking: 3,
                   durationOfBreak: TimeSpan.FromSeconds(30)
            ));

            if (PlatformServices.Default.Application.ApplicationName != "testhost")
            {
                var healthCheck = services.AddHealthChecksUI(setupSettings: setup =>
                {
                    //Manter a url como localhost, é extremamente importantante para sua aplicação poder funcionar no AKS
                    setup.AddHealthCheckEndpoint("liveness", "/health");
                    setup.AddHealthCheckEndpoint("readness", "/ready");
                    //Para cada sistema de terceiro ou API da Wiz
                    //setup.AddHealthCheckEndpoint("{sistema}", "URL liveness");

                    setup.AddWebhookNotification("Teams", Configuration["Webhook:Teams"],
                        payload: File.ReadAllText(Path.Combine(".", "MessageCard", "ServiceDown.json")),
                        restorePayload: File.ReadAllText(Path.Combine(".", "MessageCard", "ServiceRestore.json")),
                        customMessageFunc: report =>
                            {
                                var failing = report.Entries.Where(e => e.Value.Status == UIHealthStatus.Unhealthy);
                                return $"{AppDomain.CurrentDomain.FriendlyName}: {failing.Count()} healthchecks are failing";
                            }
                        );
                }).AddHealthChecks();

                //500 mb
                healthCheck.AddProcessAllocatedMemoryHealthCheck(500 * 1024 * 1024, "Process Memory", tags: new[] { "self" });
                //500 mb
                healthCheck.AddPrivateMemoryHealthCheck(1500 * 1024 * 1024, "Private memory", tags: new[] { "self" });
                //healthCheck.AddVirtualMemorySizeHealthCheck(int.MaxValue, "Virtual Memory", tags: new[] { "self" });

                healthCheck.AddSqlServer(Configuration["ConnectionStrings:CustomerDB"], tags: new[] { "services" });

                //dotnet add <Project> package AspNetCore.HealthChecks.Redis
                //healthCheck.AddRedis(Configuration["Data:ConnectionStrings:Redis"], tags: new[] {"services"});

                //dotnet add <Project> package AspNetCore.HealthChecks.OpenIdConnectServer
                healthCheck.AddIdentityServer(new Uri(Configuration["WizID:Authority"]), "SSO Wiz", tags: new[] { "services" });

                //if (WebHostEnvironment.IsProduction())
                //{
                    //dotnet add <Project> package AspNetCore.HealthChecks.AzureKeyVault
                    //healthChecks.AddAzureKeyVault(options =>
                    //{
                    //    options.UseKeyVaultUrl($"{Configuration["Azure:KeyVaultUrl"]}");
                    //}, name: "azure-key-vault",tags: new[] {"services"});
                //}

                healthCheck.AddApplicationInsightsPublisher();
            }

            if (!WebHostEnvironment.IsProduction())
            {
                services.AddSwaggerDocument(document =>
                {
                    document.DocumentName = "v1";
                    document.Version = "v1";
                    document.Title = "Biblioteca API";
                    document.Description = "API de Biblioteca";
                    document.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                    document.AddSecurity("JWT", Enumerable.Empty<string>(), new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = HeaderNames.Authorization,
                        Description = "Token de autenticação via SSO",
                        In = SwaggerSecurityApiKeyLocation.Header
                    });
                });
            }

            services.AddAutoMapper(typeof(Startup));
            services.AddHttpContextAccessor();
            services.AddApplicationInsightsTelemetry();

            RegisterServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ApplicationInsightsSettings> options)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseResponseCompression();

            if (PlatformServices.Default.Application.ApplicationName != "testhost")
            {
                app.UseHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = r => r.Tags.Contains("self"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                app.UseHealthChecks("/ready", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("services"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            }

            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUi3();
            }

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseLogMiddleware();

            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new ErrorHandlerMiddleware(options, env).Invoke
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecksUI(opt =>
                {
                    opt.UIPath = "/health-ui";
                });

                endpoints.MapControllers();
            });
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.Configure<ApplicationInsightsSettings>(Configuration.GetSection("ApplicationInsights"));

            #region Service

            services.AddScoped<ICustomerService, CustomerService>();

            #endregion

            #region Domain

            services.AddScoped<IDomainNotification, DomainNotification>();

            #endregion

            #region Infra

            services.AddDbContext<EntityContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CustomerDB")));
            services.AddScoped<DapperContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IIdentityService, IdentityService>();

            #endregion
        }
    }
}

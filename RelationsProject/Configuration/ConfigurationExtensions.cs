using RelationsProject.Abstracts;
using RelationsProject.Entities;
using RelationsProject.Exceptions;
using RelationsProject.Repositories;
using RelationsProject.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using RelationsProject.Repositories.Context;
using RelationsProject.Exceptions.BookExceptions;

namespace RelationsProject.ConfigurationExtensions
{
    public static class ConfigurationExtensions
    {
        public static void ValidateIdInRange(this int id) // Bu metot, ID'nin belirli bir aralıkta olup olmadığını kontrol eder.
        {
            if (!(id > 0 && id <= 1000))
            {
                throw new BookBadRequestException(new Book { Id = id, Title = "Invalid ID", Price = 0 }); // Eğer ID 0'dan küçük veya 1000'den büyükse, hata fırlatır.
            }
        }

        public static void UseCustomExceptionHandler(this WebApplication app) // Bu metot, özel hata işleyicisini kullanmak için uygulamaya ekler.
        {
            app.UseExceptionHandler((appError) => // UseExceptionHandler, global hata yakalama middleware'idir. // Tüm hataları yakalar ve işleme alır.
            {
                appError.Run(async (context) => // Run, middleware'in çalıştırılacağı yerdir. // Hata oluştuğunda bu kod bloğu çalışır.
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError; // Hata durum kodunu 500 Internal Server Error olarak ayarlar.
                    context.Response.ContentType = "application/json"; // Hata mesajının içeriğini JSON olarak ayarlar.

                    var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>(); // IExceptionHandlerPathFeature, hata ile ilgili bilgileri tutar. // Hata ile ilgili bilgileri alır.

                    if (contextFeature is not null)
                    {
                        context.Response.StatusCode = contextFeature.Error switch // Hata türüne göre durum kodunu ayarlar.
                        {
                            NotFoundException => StatusCodes.Status404NotFound, // NotFoundException durumunda 404 Not Found döndürüyoruz.
                            BadRequestException => StatusCodes.Status400BadRequest, // BadRequestException durumunda 400 Bad Request döndürüyoruz.
                            ArgumentOutOfRangeException => StatusCodes.Status400BadRequest, // Örnek olarak, ArgumentOutOfRangeException durumunda da 400 Bad Request döndürüyoruz.
                            KeyNotFoundException => StatusCodes.Status404NotFound, // KeyNotFoundException durumunda 404 Not Found döndürüyoruz.
                            ArgumentException => StatusCodes.Status400BadRequest, // ArgumentException durumunda 400 Bad Request döndürüyoruz.
                            ValidationException => StatusCodes.Status422UnprocessableEntity, // ValidationException durumunda 422 Unprocessable Entity döndürüyoruz.
                            _ => StatusCodes.Status500InternalServerError, // Diğer tüm durumlarda 500 Internal Server Error döndürüyoruz.
                        };

                        //context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                        // await context.Response.WriteAsync("An error has been occured beybi !"); // Custom hata mesajı, tüm hatalarda bu mesajı döner.
                        // await context.Response.WriteAsync(contextFeature.Error.Message); // Hatanın türüne bağlı olarak değişken hata mesajları alınır.

                        // Hata detaylarını JSON formatında döndürür.
                        await context.Response.WriteAsync((new ErrorDetails // ErrorDetails sınıfı, hata detaylarını tutar.
                        {
                            Message = contextFeature.Error.Message,
                            ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            StatusCode = context.Response.StatusCode
                        }).ToString()); // ToString metodu, ErrorDetails sınıfını JSON formatında serileştirir. // Bu sayede hata detaylarını JSON formatında döndürürüz.
                    }
                });
            });
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services) // Bu metot, CORS (Cross-Origin Resource Sharing) yapılandırmasını ekler.
        {
            // Register CORS services
            services.AddCors(options =>
            {
                options.AddPolicy("All",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://localhost:7048/").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            });

            return services; // IServiceCollection arayüzünü döndürür, böylece metot zincirleme (fluent) olarak kullanılabilir.
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services) // Bu metot, Swagger yapılandırmasını ekler.
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "DotNetCoreMinimalAPI",
                    Version = "v1",
                    Description = "A simple example ASP.NET Core Minimal Web API",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Ozan Yaprak",
                        Email = "oznyprk@gmail.com",
                        Url = new Uri("https://github.com/OzanYaprak")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/license/mit/")
                    },
                    TermsOfService = new Uri("https://www.google.com.tr")
                });
            });

            return services; // IServiceCollection arayüzünü döndürür, böylece metot zincirleme (fluent) olarak kullanılabilir.
        }

        public static IServiceCollection UseSqlServerContext(this IServiceCollection services, IConfiguration configuration) // Bu metot, SQL Server veritabanı bağlantısını yapılandırır.
        {
            // Register DbContext with SQL Server
            {
                services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
                return services;
            }
        }

        public static IServiceCollection ServicesIocRegisters(this IServiceCollection services) // Bu metot, Dependency Injection (DI) ile hizmetleri kaydeder.
        {
            services.AddScoped<IBookService, BookServiceV3>(); // IBookService arayüzünü BookServiceV3 sınıfına bağlar. // Scoped olarak ekler, yani her HTTP isteği için yeni bir örnek oluşturulur.
            return services; // IServiceCollection arayüzünü döndürür, böylece metot zincirleme (fluent) olarak kullanılabilir.
        }

        public static IServiceCollection RepositoryIocRegisters(this IServiceCollection services) // Bu metot, Dependency Injection (DI) ile hizmetleri kaydeder.
        {
            services.AddScoped<BookRepository>(); // BookRepository sınıfını DI konteynerine ekler. // Scoped olarak ekler, yani her HTTP isteği için yeni bir örnek oluşturulur.
            return services; // IServiceCollection arayüzünü döndürür, böylece metot zincirleme (fluent) olarak kullanılabilir.
        }

        public static IServiceCollection CustomIocRegisters(this IServiceCollection services) // Bu metot, Dependency Injection (DI) ile hizmetleri kaydeder.
        {
            services.AddAutoMapper(typeof(Program)); // AutoMapper'ı ekler. // Program sınıfının bulunduğu assembly'den AutoMapper profillerini tarar.
            return services; // IServiceCollection arayüzünü döndürür, böylece metot zincirleme (fluent) olarak kullanılabilir.
        }
    }
}
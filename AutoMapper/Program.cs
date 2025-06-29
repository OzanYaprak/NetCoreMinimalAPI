
using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AutoMapper.Entities;
using AutoMapper.Exceptions;
using AutoMapper.Repositories;
using AutoMapper.Services;
using AutoMapper.Abstracts;
using AutoMapperProject.DTOs;

namespace AutoMapper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(x =>
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

            // ESKÝ builder.Services.AddSingleton<IBookService, BookService>(); // Dependency Injection ile BookServiceV2'i IBookService arayüzüne baðlar. // Singleton olarak ekler, yani uygulama ömrü boyunca tek bir örnek kullanýlýr.
            builder.Services.AddScoped<IBookService, BookServiceV3>(); // Dependency Injection ile BookServiceV3'i IBookService arayüzüne baðlar. // Scoped olarak ekler, yani her HTTP isteði için yeni bir örnek oluþturulur.
            builder.Services.AddScoped<BookRepository>(); // Dependency Injection ile BookRepository'i ekler. // Scoped olarak ekler, yani her HTTP isteði için yeni bir örnek oluþturulur.

            builder.Services.AddAutoMapper(typeof(Program)); // AutoMapper'ý ekler. // Program sýnýfýnýn bulunduðu assembly'den AutoMapper profillerini tarar.

            builder.Services.AddDbContext<RepositoryContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"))); // Dependency Injection ile RepositoryContext'i ekler. // UseSqlServer, SQL Server veritabaný baðlantýsýný kullanýr. // builder.Configuration.GetConnectionString("sqlConnection"), appsettings.json dosyasýndaki sqlConnection baðlantý dizesini alýr.

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseExceptionHandler((appError) => // UseExceptionHandler, global hata yakalama middleware'idir. // Tüm hatalarý yakalar ve iþleme alýr.
            {
                appError.Run(async (context) => // Run, middleware'in çalýþtýrýlacaðý yerdir. // Hata oluþtuðunda bu kod bloðu çalýþýr.
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError; // Hata durum kodunu 500 Internal Server Error olarak ayarlar.
                    context.Response.ContentType = "application/json"; // Hata mesajýnýn içeriðini JSON olarak ayarlar.

                    var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>(); // IExceptionHandlerPathFeature, hata ile ilgili bilgileri tutar. // Hata ile ilgili bilgileri alýr.

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
                            _ => StatusCodes.Status500InternalServerError, // Diðer tüm durumlarda 500 Internal Server Error döndürüyoruz.
                        };

                        //context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                        // await context.Response.WriteAsync("An error has been occured beybi !"); // Custom hata mesajý, tüm hatalarda bu mesajý döner.
                        // await context.Response.WriteAsync(contextFeature.Error.Message); // Hatanýn türüne baðlý olarak deðiþken hata mesajlarý alýnýr.

                        // Hata detaylarýný JSON formatýnda döndürür.
                        await context.Response.WriteAsync((new ErrorDetails // ErrorDetails sýnýfý, hata detaylarýný tutar.
                        {
                            Message = contextFeature.Error.Message,
                            ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            StatusCode = context.Response.StatusCode
                        }).ToString()); // ToString metodu, ErrorDetails sýnýfýný JSON formatýnda serileþtirir. // Bu sayede hata detaylarýný JSON formatýnda döndürürüz.
                    }
                });
            });

            // GET
            app.MapGet("/api/error", () =>
            {
                throw new Exception("An error has been occured.");
            }).Produces<ErrorDetails>(StatusCodes.Status500InternalServerError) // Produces, Swagger'da bu endpoint'in hata kodunu ve hata mesajýný gösterir.
            .ExcludeFromDescription(); // ExcludeFromDescription, Swagger'da bu endpoint'i gizler. // Bu endpoint'i Swagger'da görmek istemiyorsanýz kullanabilirsiniz.

            // GET
            app.MapGet("/api/books", (IBookService bookService) =>
            {
                return bookService.Count > 0 ? Results.Ok(bookService.GetBooks()) : throw new BookNotFoundException(0); // Eðer kitap listesi boþ deðilse, kitap listesini döndürür. // Eðer kitap listesi boþsa, hata fýrlatýr.
            })
                .Produces<List<Book>>(StatusCodes.Status200OK) // Produces, Swagger'da bu endpoint'in baþarýlý durum kodunu ve baþarýlý durum mesajýný gösterir.
                .Produces(StatusCodes.Status204NoContent) // Eðer kitap listesi boþsa, 204 No Content durum kodunu döndürür.
                .WithTags("CRUD", "GETs"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduðunu gösterir. // Bu endpoint'i "CRUD" ve "GETs" gruplarýna ekler.

            // GET BY ID
            app.MapGet("/api/books/{id}", (int id, IBookService bookService) =>
            {
                if (!(id > 0 && id <= 1000))
                {
                    throw new BookBadRequestException(new Book { Id = id, Title = "Invalid ID", Price = 0 }); // Eðer ID 0'dan küçük veya 1000'den büyükse, hata fýrlatýr.
                }

                var book = bookService.GetBooks().Where(x => x.Id.Equals(id)).FirstOrDefault(); // Kitap listesinden ID'ye göre kitap arar.

                return book is not null ? Results.Ok(book) : throw new BookNotFoundException(id); // Eðer kitap bulunursa, kitap bilgilerini döndürür. // Eðer kitap bulunamazsa, hata fýrlatýr.

                //return Results.Ok(Book.ListWithId(id)); // Book sýnýfýndaki ListWithId metodunu kullanarak ID'ye göre kitap döndürür.
            })
                .Produces<Book>(StatusCodes.Status200OK) // Produces, Swagger'da bu endpoint'in baþarýlý durum kodunu ve baþarýlý durum mesajýný gösterir.
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound) // Eðer kitap bulunamazsa, 404 Not Found durum kodunu döndürür.
                .WithTags("GETs"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduðunu gösterir. // Bu endpoint'i "GETs" grubuna ekler.

            // POST
            app.MapPost("/api/books", (BookDtoForInsertion InsertBook, IBookService bookService) =>
            {
                var book = bookService.CreateBook(InsertBook);
                return Results.Created($"/api/books/{book.Id}", InsertBook); // 201
            })
                .Produces<Book>(StatusCodes.Status201Created) // Produces, Swagger'da bu endpoint'in baþarýlý durum kodunu ve baþarýlý durum mesajýný gösterir.
                .Produces<List<ValidationResult>>(StatusCodes.Status422UnprocessableEntity) // Eðer kitap doðrulama hatasý varsa, 422 Unprocessable Entity durum kodunu döndürür.
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduðunu gösterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // PUT
            app.MapPut("/api/books/{id}", (int id, BookDtoForUpdate updateBook, IBookService bookService) =>
            {
                Book book = bookService.UpdateBook(id, updateBook);
                return Results.Ok(book); // 200
            })
                .Produces<Book>(StatusCodes.Status200OK)
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .Produces<ErrorDetails>(StatusCodes.Status422UnprocessableEntity)
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduðunu gösterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // DELETE
            app.MapDelete("/api/books/{id}", (int id, IBookService bookService) =>
            {
                if (!(id > 0 && id <= 1000))
                {
                    throw new BookBadRequestException(new Book { Id = id, Title = "Invalid ID", Price = 0 }); // Eðer ID 0'dan küçük veya 1000'den büyükse, hata fýrlatýr.
                }

                bookService.DeleteBook(id); // Kitabý kitap listesinden siler.

                return Results.NoContent(); // 204
            })
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduðunu gösterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // GET SEARCH
            app.MapGet("/api/books/search", (string? title, IBookService bookService) =>
            {
                var books = string.IsNullOrEmpty(title) ? bookService.GetBooks() : bookService.GetBooks().Where(x => x.Title is not null && x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList(); // Eðer title boþ ise, tüm kitaplarý döndürür. // Eðer title dolu ise, kitap listesinden title'a göre arama yapar. // StringComparison.OrdinalIgnoreCase, büyük/küçük harf duyarsýz arama yapar.

                return books.Any() ? Results.Ok(books) : Results.NotFound(); // Eðer kitap listesi boþ deðilse, kitap listesini döndürür. // Eðer kitap listesi boþsa, 404 Not Found durum kodunu döndürür.
            })
                .Produces<List<Book>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .WithTags("GETs");



            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

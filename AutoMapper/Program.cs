
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

            // ESK� builder.Services.AddSingleton<IBookService, BookService>(); // Dependency Injection ile BookServiceV2'i IBookService aray�z�ne ba�lar. // Singleton olarak ekler, yani uygulama �mr� boyunca tek bir �rnek kullan�l�r.
            builder.Services.AddScoped<IBookService, BookServiceV3>(); // Dependency Injection ile BookServiceV3'i IBookService aray�z�ne ba�lar. // Scoped olarak ekler, yani her HTTP iste�i i�in yeni bir �rnek olu�turulur.
            builder.Services.AddScoped<BookRepository>(); // Dependency Injection ile BookRepository'i ekler. // Scoped olarak ekler, yani her HTTP iste�i i�in yeni bir �rnek olu�turulur.

            builder.Services.AddAutoMapper(typeof(Program)); // AutoMapper'� ekler. // Program s�n�f�n�n bulundu�u assembly'den AutoMapper profillerini tarar.

            builder.Services.AddDbContext<RepositoryContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"))); // Dependency Injection ile RepositoryContext'i ekler. // UseSqlServer, SQL Server veritaban� ba�lant�s�n� kullan�r. // builder.Configuration.GetConnectionString("sqlConnection"), appsettings.json dosyas�ndaki sqlConnection ba�lant� dizesini al�r.

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseExceptionHandler((appError) => // UseExceptionHandler, global hata yakalama middleware'idir. // T�m hatalar� yakalar ve i�leme al�r.
            {
                appError.Run(async (context) => // Run, middleware'in �al��t�r�laca�� yerdir. // Hata olu�tu�unda bu kod blo�u �al���r.
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError; // Hata durum kodunu 500 Internal Server Error olarak ayarlar.
                    context.Response.ContentType = "application/json"; // Hata mesaj�n�n i�eri�ini JSON olarak ayarlar.

                    var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>(); // IExceptionHandlerPathFeature, hata ile ilgili bilgileri tutar. // Hata ile ilgili bilgileri al�r.

                    if (contextFeature is not null)
                    {
                        context.Response.StatusCode = contextFeature.Error switch // Hata t�r�ne g�re durum kodunu ayarlar.
                        {
                            NotFoundException => StatusCodes.Status404NotFound, // NotFoundException durumunda 404 Not Found d�nd�r�yoruz.
                            BadRequestException => StatusCodes.Status400BadRequest, // BadRequestException durumunda 400 Bad Request d�nd�r�yoruz.
                            ArgumentOutOfRangeException => StatusCodes.Status400BadRequest, // �rnek olarak, ArgumentOutOfRangeException durumunda da 400 Bad Request d�nd�r�yoruz.
                            KeyNotFoundException => StatusCodes.Status404NotFound, // KeyNotFoundException durumunda 404 Not Found d�nd�r�yoruz.
                            ArgumentException => StatusCodes.Status400BadRequest, // ArgumentException durumunda 400 Bad Request d�nd�r�yoruz.
                            ValidationException => StatusCodes.Status422UnprocessableEntity, // ValidationException durumunda 422 Unprocessable Entity d�nd�r�yoruz.
                            _ => StatusCodes.Status500InternalServerError, // Di�er t�m durumlarda 500 Internal Server Error d�nd�r�yoruz.
                        };

                        //context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                        // await context.Response.WriteAsync("An error has been occured beybi !"); // Custom hata mesaj�, t�m hatalarda bu mesaj� d�ner.
                        // await context.Response.WriteAsync(contextFeature.Error.Message); // Hatan�n t�r�ne ba�l� olarak de�i�ken hata mesajlar� al�n�r.

                        // Hata detaylar�n� JSON format�nda d�nd�r�r.
                        await context.Response.WriteAsync((new ErrorDetails // ErrorDetails s�n�f�, hata detaylar�n� tutar.
                        {
                            Message = contextFeature.Error.Message,
                            ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            StatusCode = context.Response.StatusCode
                        }).ToString()); // ToString metodu, ErrorDetails s�n�f�n� JSON format�nda serile�tirir. // Bu sayede hata detaylar�n� JSON format�nda d�nd�r�r�z.
                    }
                });
            });

            // GET
            app.MapGet("/api/error", () =>
            {
                throw new Exception("An error has been occured.");
            }).Produces<ErrorDetails>(StatusCodes.Status500InternalServerError) // Produces, Swagger'da bu endpoint'in hata kodunu ve hata mesaj�n� g�sterir.
            .ExcludeFromDescription(); // ExcludeFromDescription, Swagger'da bu endpoint'i gizler. // Bu endpoint'i Swagger'da g�rmek istemiyorsan�z kullanabilirsiniz.

            // GET
            app.MapGet("/api/books", (IBookService bookService) =>
            {
                return bookService.Count > 0 ? Results.Ok(bookService.GetBooks()) : throw new BookNotFoundException(0); // E�er kitap listesi bo� de�ilse, kitap listesini d�nd�r�r. // E�er kitap listesi bo�sa, hata f�rlat�r.
            })
                .Produces<List<Book>>(StatusCodes.Status200OK) // Produces, Swagger'da bu endpoint'in ba�ar�l� durum kodunu ve ba�ar�l� durum mesaj�n� g�sterir.
                .Produces(StatusCodes.Status204NoContent) // E�er kitap listesi bo�sa, 204 No Content durum kodunu d�nd�r�r.
                .WithTags("CRUD", "GETs"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait oldu�unu g�sterir. // Bu endpoint'i "CRUD" ve "GETs" gruplar�na ekler.

            // GET BY ID
            app.MapGet("/api/books/{id}", (int id, IBookService bookService) =>
            {
                if (!(id > 0 && id <= 1000))
                {
                    throw new BookBadRequestException(new Book { Id = id, Title = "Invalid ID", Price = 0 }); // E�er ID 0'dan k���k veya 1000'den b�y�kse, hata f�rlat�r.
                }

                var book = bookService.GetBooks().Where(x => x.Id.Equals(id)).FirstOrDefault(); // Kitap listesinden ID'ye g�re kitap arar.

                return book is not null ? Results.Ok(book) : throw new BookNotFoundException(id); // E�er kitap bulunursa, kitap bilgilerini d�nd�r�r. // E�er kitap bulunamazsa, hata f�rlat�r.

                //return Results.Ok(Book.ListWithId(id)); // Book s�n�f�ndaki ListWithId metodunu kullanarak ID'ye g�re kitap d�nd�r�r.
            })
                .Produces<Book>(StatusCodes.Status200OK) // Produces, Swagger'da bu endpoint'in ba�ar�l� durum kodunu ve ba�ar�l� durum mesaj�n� g�sterir.
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound) // E�er kitap bulunamazsa, 404 Not Found durum kodunu d�nd�r�r.
                .WithTags("GETs"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait oldu�unu g�sterir. // Bu endpoint'i "GETs" grubuna ekler.

            // POST
            app.MapPost("/api/books", (BookDtoForInsertion InsertBook, IBookService bookService) =>
            {
                var book = bookService.CreateBook(InsertBook);
                return Results.Created($"/api/books/{book.Id}", InsertBook); // 201
            })
                .Produces<Book>(StatusCodes.Status201Created) // Produces, Swagger'da bu endpoint'in ba�ar�l� durum kodunu ve ba�ar�l� durum mesaj�n� g�sterir.
                .Produces<List<ValidationResult>>(StatusCodes.Status422UnprocessableEntity) // E�er kitap do�rulama hatas� varsa, 422 Unprocessable Entity durum kodunu d�nd�r�r.
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait oldu�unu g�sterir. // Bu endpoint'i "CRUD" grubuna ekler.

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
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait oldu�unu g�sterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // DELETE
            app.MapDelete("/api/books/{id}", (int id, IBookService bookService) =>
            {
                if (!(id > 0 && id <= 1000))
                {
                    throw new BookBadRequestException(new Book { Id = id, Title = "Invalid ID", Price = 0 }); // E�er ID 0'dan k���k veya 1000'den b�y�kse, hata f�rlat�r.
                }

                bookService.DeleteBook(id); // Kitab� kitap listesinden siler.

                return Results.NoContent(); // 204
            })
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait oldu�unu g�sterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // GET SEARCH
            app.MapGet("/api/books/search", (string? title, IBookService bookService) =>
            {
                var books = string.IsNullOrEmpty(title) ? bookService.GetBooks() : bookService.GetBooks().Where(x => x.Title is not null && x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList(); // E�er title bo� ise, t�m kitaplar� d�nd�r�r. // E�er title dolu ise, kitap listesinden title'a g�re arama yapar. // StringComparison.OrdinalIgnoreCase, b�y�k/k���k harf duyars�z arama yapar.

                return books.Any() ? Results.Ok(books) : Results.NotFound(); // E�er kitap listesi bo� de�ilse, kitap listesini d�nd�r�r. // E�er kitap listesi bo�sa, 404 Not Found durum kodunu d�nd�r�r.
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

using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace DependencyInjection
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

            builder.Services.AddSingleton<BookService>(); // BookService, kitap verilerini tutar ve i�lemlerini yapar. // Singleton olarak eklenir, b�ylece uygulama boyunca tek bir �rne�i kullan�l�r.

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Global Error Handler Middleware
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
            app.MapGet("/api/books", (BookService bookService) =>
            {
                return bookService.GetBooks.Count > 0 ? bookService.GetBooks : throw new BookNotFoundException(0); // E�er kitap listesi bo� de�ilse, kitap listesini d�nd�r�r. // E�er kitap listesi bo�sa, hata f�rlat�r.
            })
                .Produces<List<Book>>(StatusCodes.Status200OK) // Produces, Swagger'da bu endpoint'in ba�ar�l� durum kodunu ve ba�ar�l� durum mesaj�n� g�sterir.
                .Produces(StatusCodes.Status204NoContent) // E�er kitap listesi bo�sa, 204 No Content durum kodunu d�nd�r�r.
                .WithTags("CRUD", "GETs"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait oldu�unu g�sterir. // Bu endpoint'i "CRUD" ve "GETs" gruplar�na ekler.

            // GET BY ID
            app.MapGet("/api/books/{id}", (int id, BookService bookService) =>
            {
                if (!(id > 0 && id <= 1000))
                {
                    throw new BookBadRequestException(new Book { Id = id, Title = "Invalid ID", Price = 0 }); // E�er ID 0'dan k���k veya 1000'den b�y�kse, hata f�rlat�r.
                }

                var book = bookService.GetBooks.Where(x => x.Id.Equals(id)).FirstOrDefault(); // Kitap listesinden ID'ye g�re kitap arar.

                return book is not null ? Results.Ok(book) : throw new BookNotFoundException(id); // E�er kitap bulunursa, kitap bilgilerini d�nd�r�r. // E�er kitap bulunamazsa, hata f�rlat�r.

                //return Results.Ok(Book.ListWithId(id)); // Book s�n�f�ndaki ListWithId metodunu kullanarak ID'ye g�re kitap d�nd�r�r.
            })
                .Produces<Book>(StatusCodes.Status200OK) // Produces, Swagger'da bu endpoint'in ba�ar�l� durum kodunu ve ba�ar�l� durum mesaj�n� g�sterir.
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound) // E�er kitap bulunamazsa, 404 Not Found durum kodunu d�nd�r�r.
                .WithTags("GETs"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait oldu�unu g�sterir. // Bu endpoint'i "GETs" grubuna ekler.

            // POST
            app.MapPost("/api/books", (Book InsertBook, BookService bookService) =>
            {
                var validationResults = new List<ValidationResult>(); // ValidationResult, do�rulama sonu�lar�n� tutar.
                var context = new ValidationContext(InsertBook); // ValidationContext, do�rulama ba�lam�n� tutar.
                bool isValid = Validator.TryValidateObject(InsertBook, context, validationResults, true); // Validator, do�rulama i�lemini yapar. // TryValidateObject, do�rulama i�lemini yapar ve sonu�lar� validationResults listesine ekler. // true parametresi, t�m �zelliklerin do�rulanmas�n� sa�lar.

                if (!isValid)
                {
                    return Results.UnprocessableEntity(validationResults); // 422
                }

                // Temel alan kontrolleri
                if (string.IsNullOrWhiteSpace(InsertBook.Title) || InsertBook.Price <= 0)
                {
                    throw new BookBadRequestException(InsertBook);
                }
                if (InsertBook.Title.Contains("string"))
                {
                    throw new BookBadRequestException(InsertBook);
                }

                bookService.CreateBook(InsertBook);

                return Results.Created($"/api/books/{InsertBook.Id}", InsertBook); // 201
            })
                .Produces<Book>(StatusCodes.Status201Created) // Produces, Swagger'da bu endpoint'in ba�ar�l� durum kodunu ve ba�ar�l� durum mesaj�n� g�sterir.
                .Produces<List<ValidationResult>>(StatusCodes.Status422UnprocessableEntity) // E�er kitap do�rulama hatas� varsa, 422 Unprocessable Entity durum kodunu d�nd�r�r.
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait oldu�unu g�sterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // PUT
            app.MapPut("/api/books/{id}", (int id, Book updateBook, BookService bookService) =>
            {
                if (!(id > 0 && id <= 1000))
                {
                    throw new BookBadRequestException(new Book { Id = id, Title = "Invalid ID", Price = 0 }); // E�er ID 0'dan k���k veya 1000'den b�y�kse, hata f�rlat�r.
                }

                var validationResults = new List<ValidationResult>(); // ValidationResult, do�rulama sonu�lar�n� tutar.
                var context = new ValidationContext(updateBook); // ValidationContext, do�rulama ba�lam�n� tutar.
                bool isValid = Validator.TryValidateObject(updateBook, context, validationResults, true); // Validator, do�rulama i�lemini yapar. // TryValidateObject, do�rulama i�lemini yapar ve sonu�lar� validationResults listesine ekler. // true parametresi, t�m �zelliklerin do�rulanmas�n� sa�lar.

                if (!isValid)
                {
                    return Results.UnprocessableEntity(validationResults.First().ErrorMessage); // 422
                }

                Book book = bookService.UpdateBook(id, updateBook);

                return Results.Ok(book); // 200
            })
                .Produces<Book>(StatusCodes.Status200OK)
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
                .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
                .Produces<ErrorDetails>(StatusCodes.Status422UnprocessableEntity)
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait oldu�unu g�sterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // DELETE
            app.MapDelete("/api/books/{id}", (int id, BookService bookService) =>
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
            app.MapGet("/api/books/search", (string? title, BookService bookService) =>
            {
                var books = string.IsNullOrEmpty(title) ? bookService.GetBooks : bookService.GetBooks.Where(x => x.Title is not null && x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList(); // E�er title bo� ise, t�m kitaplar� d�nd�r�r. // E�er title dolu ise, kitap listesinden title'a g�re arama yapar. // StringComparison.OrdinalIgnoreCase, b�y�k/k���k harf duyars�z arama yapar.

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

    public abstract class NotFoundException : Exception
    {
        protected NotFoundException(string message) : base(message)
        {
        }
    }

    public sealed class BookBadRequestException : BadRequestException
    {
        public BookBadRequestException(Book book) : base($"Bad Request")
        {
        }
    }

    public sealed class BookNotFoundException : NotFoundException
    {
        public BookNotFoundException(int id) : base($"The book with {id} could not be found!")
        {
        }
    }

    public abstract class BadRequestException : Exception
    {
        protected BadRequestException(string message) : base(message)
        {
        }
    }

    public class ErrorDetails
    {
        public string? ErrorDate { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }

        public override string ToString() // ToString yaz�ld���nda ErrorDetails s�n�f�n� Serialize edecektir.
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class Book
    {
        [Required]
        public int Id { get; set; }

        [Range(10, 100)]
        public Decimal Price { get; set; }

        [MinLength(2, ErrorMessage = "Min. lenght must be 2")]
        [MaxLength(250, ErrorMessage = "Max. lenght must be 250")]
        public String? Title { get; set; }
    }

    public class BookService
    {
        private readonly List<Book> _books;

        public BookService()
        {
            // �rnek kitap verileri
            _books = new List<Book>()
            {
                new Book(){ Id=1, Title="Su� ve Ceza", Price=400 },
                new Book(){ Id=2, Title="Beyaz Zambaklar �lkesinde", Price=300 },
                new Book(){ Id=3, Title="Hayvanlar �iftli�i", Price=230 },
            };
        }

        public List<Book> GetBooks => _books;

        public Book GetBookById(int id)
        {
            return _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id);
        }

        public void CreateBook(Book book)
        {
            book.Id = _books.Max(x => x.Id) + 1; // Yeni kitap i�in ID'yi otomatik olarak art�r�r. // Kitap listesindeki en y�ksek ID'ye 1 ekler.
            _books.Add(book); // Yeni kitab� kitap listesine ekler.
        }

        public Book UpdateBook(int id, Book book)
        {
            var existingBook = _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id); // ID'ye g�re kitap arar, e�er bulunamazsa hata f�rlat�r.
            existingBook.Title = book.Title;
            existingBook.Price = book.Price;

            return existingBook; // G�ncellenmi� kitab� d�nd�r�r.
        }

        public void DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id);
            _books.Remove(book); // Kitab� kitap listesinden siler.
        }
    }
}
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

            builder.Services.AddSingleton<BookService>(); // BookService, kitap verilerini tutar ve iþlemlerini yapar. // Singleton olarak eklenir, böylece uygulama boyunca tek bir örneði kullanýlýr.

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Global Error Handler Middleware
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
            app.MapGet("/api/books", (BookService bookService) =>
            {
                return bookService.GetBooks.Count > 0 ? bookService.GetBooks : throw new BookNotFoundException(0); // Eðer kitap listesi boþ deðilse, kitap listesini döndürür. // Eðer kitap listesi boþsa, hata fýrlatýr.
            })
                .Produces<List<Book>>(StatusCodes.Status200OK) // Produces, Swagger'da bu endpoint'in baþarýlý durum kodunu ve baþarýlý durum mesajýný gösterir.
                .Produces(StatusCodes.Status204NoContent) // Eðer kitap listesi boþsa, 204 No Content durum kodunu döndürür.
                .WithTags("CRUD", "GETs"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduðunu gösterir. // Bu endpoint'i "CRUD" ve "GETs" gruplarýna ekler.

            // GET BY ID
            app.MapGet("/api/books/{id}", (int id, BookService bookService) =>
            {
                if (!(id > 0 && id <= 1000))
                {
                    throw new BookBadRequestException(new Book { Id = id, Title = "Invalid ID", Price = 0 }); // Eðer ID 0'dan küçük veya 1000'den büyükse, hata fýrlatýr.
                }

                var book = bookService.GetBooks.Where(x => x.Id.Equals(id)).FirstOrDefault(); // Kitap listesinden ID'ye göre kitap arar.

                return book is not null ? Results.Ok(book) : throw new BookNotFoundException(id); // Eðer kitap bulunursa, kitap bilgilerini döndürür. // Eðer kitap bulunamazsa, hata fýrlatýr.

                //return Results.Ok(Book.ListWithId(id)); // Book sýnýfýndaki ListWithId metodunu kullanarak ID'ye göre kitap döndürür.
            })
                .Produces<Book>(StatusCodes.Status200OK) // Produces, Swagger'da bu endpoint'in baþarýlý durum kodunu ve baþarýlý durum mesajýný gösterir.
                .Produces<ErrorDetails>(StatusCodes.Status404NotFound) // Eðer kitap bulunamazsa, 404 Not Found durum kodunu döndürür.
                .WithTags("GETs"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduðunu gösterir. // Bu endpoint'i "GETs" grubuna ekler.

            // POST
            app.MapPost("/api/books", (Book InsertBook, BookService bookService) =>
            {
                var validationResults = new List<ValidationResult>(); // ValidationResult, doðrulama sonuçlarýný tutar.
                var context = new ValidationContext(InsertBook); // ValidationContext, doðrulama baðlamýný tutar.
                bool isValid = Validator.TryValidateObject(InsertBook, context, validationResults, true); // Validator, doðrulama iþlemini yapar. // TryValidateObject, doðrulama iþlemini yapar ve sonuçlarý validationResults listesine ekler. // true parametresi, tüm özelliklerin doðrulanmasýný saðlar.

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
                .Produces<Book>(StatusCodes.Status201Created) // Produces, Swagger'da bu endpoint'in baþarýlý durum kodunu ve baþarýlý durum mesajýný gösterir.
                .Produces<List<ValidationResult>>(StatusCodes.Status422UnprocessableEntity) // Eðer kitap doðrulama hatasý varsa, 422 Unprocessable Entity durum kodunu döndürür.
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduðunu gösterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // PUT
            app.MapPut("/api/books/{id}", (int id, Book updateBook, BookService bookService) =>
            {
                if (!(id > 0 && id <= 1000))
                {
                    throw new BookBadRequestException(new Book { Id = id, Title = "Invalid ID", Price = 0 }); // Eðer ID 0'dan küçük veya 1000'den büyükse, hata fýrlatýr.
                }

                var validationResults = new List<ValidationResult>(); // ValidationResult, doðrulama sonuçlarýný tutar.
                var context = new ValidationContext(updateBook); // ValidationContext, doðrulama baðlamýný tutar.
                bool isValid = Validator.TryValidateObject(updateBook, context, validationResults, true); // Validator, doðrulama iþlemini yapar. // TryValidateObject, doðrulama iþlemini yapar ve sonuçlarý validationResults listesine ekler. // true parametresi, tüm özelliklerin doðrulanmasýný saðlar.

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
                .WithTags("CRUD"); // WithTags, Swagger'da bu endpoint'in hangi gruba ait olduðunu gösterir. // Bu endpoint'i "CRUD" grubuna ekler.

            // DELETE
            app.MapDelete("/api/books/{id}", (int id, BookService bookService) =>
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
            app.MapGet("/api/books/search", (string? title, BookService bookService) =>
            {
                var books = string.IsNullOrEmpty(title) ? bookService.GetBooks : bookService.GetBooks.Where(x => x.Title is not null && x.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList(); // Eðer title boþ ise, tüm kitaplarý döndürür. // Eðer title dolu ise, kitap listesinden title'a göre arama yapar. // StringComparison.OrdinalIgnoreCase, büyük/küçük harf duyarsýz arama yapar.

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

        public override string ToString() // ToString yazýldýðýnda ErrorDetails sýnýfýný Serialize edecektir.
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
            // Örnek kitap verileri
            _books = new List<Book>()
            {
                new Book(){ Id=1, Title="Suç ve Ceza", Price=400 },
                new Book(){ Id=2, Title="Beyaz Zambaklar Ülkesinde", Price=300 },
                new Book(){ Id=3, Title="Hayvanlar Çiftliði", Price=230 },
            };
        }

        public List<Book> GetBooks => _books;

        public Book GetBookById(int id)
        {
            return _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id);
        }

        public void CreateBook(Book book)
        {
            book.Id = _books.Max(x => x.Id) + 1; // Yeni kitap için ID'yi otomatik olarak artýrýr. // Kitap listesindeki en yüksek ID'ye 1 ekler.
            _books.Add(book); // Yeni kitabý kitap listesine ekler.
        }

        public Book UpdateBook(int id, Book book)
        {
            var existingBook = _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id); // ID'ye göre kitap arar, eðer bulunamazsa hata fýrlatýr.
            existingBook.Title = book.Title;
            existingBook.Price = book.Price;

            return existingBook; // Güncellenmiþ kitabý döndürür.
        }

        public void DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(x => x.Id == id) ?? throw new BookNotFoundException(id);
            _books.Remove(book); // Kitabý kitap listesinden siler.
        }
    }
}
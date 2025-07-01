using ConfigurationAndExtensions.Abstracts;
using ConfigurationAndExtensions.Configuration;
using ConfigurationAndExtensions.DTOs;
using ConfigurationAndExtensions.Entities;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args); // Web uygulamasý için yapýlandýrma oluþturur. // Bu metot, uygulama için gerekli yapýlandýrmalarý yapar. // Örneðin, appsettings.json dosyasýný okur ve gerekli hizmetleri ekler.

builder.Services.AddControllers(); // Controller'larý ekler. // Bu metot, API controller'larýný ekler ve HTTP isteklerini yönlendirmek için gerekli middleware'i yapýlandýrýr.
builder.Services.AddEndpointsApiExplorer(); // Endpoint'leri keþfetmek için gerekli hizmetleri ekler. // Bu metot, API endpoint'lerini keþfetmek için gerekli hizmetleri ekler.

#region Configuration And Extensions

builder.Services.AddCustomCors(); // CORS yapýlandýrmasýný ekler. // Bu metot, CORS politikalarýný yapýlandýrýr. // Tüm kaynaklara izin verir.
builder.Services.AddCustomSwagger(); // Swagger yapýlandýrmasýný ekler. // Bu metot, Swagger'ý yapýlandýrýr ve kullanýma hazýr hale getirir.
builder.Services.ServicesIocRegisters(); // IOC (Inversion of Control) kayýtlarýný ekler. // Bu metot, uygulama için gerekli servisleri IOC konteynerine kaydeder. // Örneðin, BookServiceV3 sýnýfýný IBookService arayüzüne kaydeder.
builder.Services.RepositoryIocRegisters(); // Repository kayýtlarýný ekler. // Bu metot, uygulama için gerekli repository'leri IOC konteynerine kaydeder. // Örneðin, BookRepository sýnýfýný IBookRepository arayüzüne kaydeder.
builder.Services.CustomIocRegisters(); // Özel IOC kayýtlarýný ekler. // Bu metot, uygulama için özel servisleri IOC konteynerine kaydeder. // Örneðin, AutoMapper'ý yapýlandýrýr ve kullanýma hazýr hale getirir.
builder.Services.UseSqlServerContext(builder.Configuration); // SQL Server veritabaný baðlantýsýný yapýlandýrýr. // Bu metot, appsettings.json dosyasýndaki ConnectionStrings bölümünden veritabaný baðlantý dizesini alýr ve DbContext'i yapýlandýrýr.

#endregion Configuration And Extensions

var app = builder.Build(); // Uygulamayý oluþturur. // Bu metot, uygulama için gerekli middleware'leri ve hizmetleri yapýlandýrýr. // Örneðin, CORS, Swagger, hata iþleme middleware'lerini ekler.
if (app.Environment.IsDevelopment()) // Uygulama geliþtirme ortamýnda ise Swagger'ý kullanýr. // Bu, uygulamanýn geliþtirme aþamasýnda Swagger arayüzünü gösterir.
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
    var book = bookService.GetBookById(id); // Kitap listesinden ID'ye göre kitap arar.
    return Results.Ok(book); // Eðer kitap bulunursa, kitap bilgilerini döndürür. // Eðer kitap bulunamazsa, hata fýrlatýr.

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

app.UseCors("All"); // Use CORS policy // Use "AllowSpecificOrigin" to restrict to a specific origin
app.UseCustomExceptionHandler(); // Özel hata iþleyicisini kullanýr. // Bu metot, global hata yakalama middleware'ini ekler. // Hatalarý JSON formatýnda döndürür. // Bu sayede hata mesajlarýný daha okunabilir hale getirir.
app.UseHttpsRedirection(); // HTTPS yönlendirmesini kullanýr. // HTTP isteklerini HTTPS isteklerine yönlendirir.
app.UseAuthorization(); // Yetkilendirmeyi kullanýr. // Bu middleware, yetkilendirme iþlemlerini yapar. // Kullanýcýlarýn yetkilerini kontrol eder.
app.MapControllers(); // Controller'larý haritalar. // Bu middleware, controller'larý HTTP isteklerine yönlendirir. // Controller'lar, HTTP isteklerini iþler ve yanýt döner.

app.Run(); // Uygulamayý baþlatýr. // Uygulama, HTTP isteklerini dinler ve yönlendirir.
using ConfigurationAndExtensions.Abstracts;
using ConfigurationAndExtensions.Configuration;
using ConfigurationAndExtensions.DTOs;
using ConfigurationAndExtensions.Entities;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args); // Web uygulamas� i�in yap�land�rma olu�turur. // Bu metot, uygulama i�in gerekli yap�land�rmalar� yapar. // �rne�in, appsettings.json dosyas�n� okur ve gerekli hizmetleri ekler.

builder.Services.AddControllers(); // Controller'lar� ekler. // Bu metot, API controller'lar�n� ekler ve HTTP isteklerini y�nlendirmek i�in gerekli middleware'i yap�land�r�r.
builder.Services.AddEndpointsApiExplorer(); // Endpoint'leri ke�fetmek i�in gerekli hizmetleri ekler. // Bu metot, API endpoint'lerini ke�fetmek i�in gerekli hizmetleri ekler.

#region Configuration And Extensions

builder.Services.AddCustomCors(); // CORS yap�land�rmas�n� ekler. // Bu metot, CORS politikalar�n� yap�land�r�r. // T�m kaynaklara izin verir.
builder.Services.AddCustomSwagger(); // Swagger yap�land�rmas�n� ekler. // Bu metot, Swagger'� yap�land�r�r ve kullan�ma haz�r hale getirir.
builder.Services.ServicesIocRegisters(); // IOC (Inversion of Control) kay�tlar�n� ekler. // Bu metot, uygulama i�in gerekli servisleri IOC konteynerine kaydeder. // �rne�in, BookServiceV3 s�n�f�n� IBookService aray�z�ne kaydeder.
builder.Services.RepositoryIocRegisters(); // Repository kay�tlar�n� ekler. // Bu metot, uygulama i�in gerekli repository'leri IOC konteynerine kaydeder. // �rne�in, BookRepository s�n�f�n� IBookRepository aray�z�ne kaydeder.
builder.Services.CustomIocRegisters(); // �zel IOC kay�tlar�n� ekler. // Bu metot, uygulama i�in �zel servisleri IOC konteynerine kaydeder. // �rne�in, AutoMapper'� yap�land�r�r ve kullan�ma haz�r hale getirir.
builder.Services.UseSqlServerContext(builder.Configuration); // SQL Server veritaban� ba�lant�s�n� yap�land�r�r. // Bu metot, appsettings.json dosyas�ndaki ConnectionStrings b�l�m�nden veritaban� ba�lant� dizesini al�r ve DbContext'i yap�land�r�r.

#endregion Configuration And Extensions

var app = builder.Build(); // Uygulamay� olu�turur. // Bu metot, uygulama i�in gerekli middleware'leri ve hizmetleri yap�land�r�r. // �rne�in, CORS, Swagger, hata i�leme middleware'lerini ekler.
if (app.Environment.IsDevelopment()) // Uygulama geli�tirme ortam�nda ise Swagger'� kullan�r. // Bu, uygulaman�n geli�tirme a�amas�nda Swagger aray�z�n� g�sterir.
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
    var book = bookService.GetBookById(id); // Kitap listesinden ID'ye g�re kitap arar.
    return Results.Ok(book); // E�er kitap bulunursa, kitap bilgilerini d�nd�r�r. // E�er kitap bulunamazsa, hata f�rlat�r.

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

app.UseCors("All"); // Use CORS policy // Use "AllowSpecificOrigin" to restrict to a specific origin
app.UseCustomExceptionHandler(); // �zel hata i�leyicisini kullan�r. // Bu metot, global hata yakalama middleware'ini ekler. // Hatalar� JSON format�nda d�nd�r�r. // Bu sayede hata mesajlar�n� daha okunabilir hale getirir.
app.UseHttpsRedirection(); // HTTPS y�nlendirmesini kullan�r. // HTTP isteklerini HTTPS isteklerine y�nlendirir.
app.UseAuthorization(); // Yetkilendirmeyi kullan�r. // Bu middleware, yetkilendirme i�lemlerini yapar. // Kullan�c�lar�n yetkilerini kontrol eder.
app.MapControllers(); // Controller'lar� haritalar. // Bu middleware, controller'lar� HTTP isteklerine y�nlendirir. // Controller'lar, HTTP isteklerini i�ler ve yan�t d�ner.

app.Run(); // Uygulamay� ba�lat�r. // Uygulama, HTTP isteklerini dinler ve y�nlendirir.
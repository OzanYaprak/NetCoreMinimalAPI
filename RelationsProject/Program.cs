using RelationsProject.Abstracts;
using RelationsProject.APIs;
using RelationsProject.Configuration;
using RelationsProject.DTOs;
using RelationsProject.Entities;
using RelationsProject.Exceptions.BookExceptions;
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

// GET ERROR
app.MapGet("/api/error", () =>
{
    throw new Exception("An error has been occured.");
}).Produces<ErrorDetails>(StatusCodes.Status500InternalServerError) // Produces, Swagger'da bu endpoint'in hata kodunu ve hata mesajýný gösterir.
.ExcludeFromDescription(); // ExcludeFromDescription, Swagger'da bu endpoint'i gizler. // Bu endpoint'i Swagger'da görmek istemiyorsanýz kullanabilirsiniz.

#region Book API Endpoint Extensions

app.GetAllBooks(); // Bu metotlar, kitaplarý listelemek için kullanýlýr. // Kitap listesini döndürür. // Eðer kitap listesi boþ ise, hata fýrlatýr.
app.GetBookById(); // Bu metotlar, kitaplarý listelemek ve ID'ye göre kitap aramak için kullanýlýr. // Bu metotlar, API endpoint'lerini haritalar ve gerekli middleware'leri ekler.
app.PostBook(); // Bu metotlar, yeni kitap eklemek için kullanýlýr. // Kitap ekler ve baþarýlý durum kodunu döndürür. // Eðer kitap doðrulama hatasý varsa, hata mesajýný döndürür.
app.PutBook(); // Bu metotlar, kitap güncellemek için kullanýlýr. // Kitap günceller ve baþarýlý durum kodunu döndürür. // Eðer kitap bulunamazsa, hata fýrlatýr. // Eðer kitap doðrulama hatasý varsa, hata mesajýný döndürür.
app.DeleteBook(); // Bu metotlar, kitap silmek için kullanýlýr. // Kitap siler ve baþarýlý durum kodunu döndürür. // Eðer kitap bulunamazsa, hata fýrlatýr.
app.SearchBooks(); // Bu metotlar, kitaplarý aramak için kullanýlýr. // Kitaplarý arar ve baþarýlý durum kodunu döndürür. // Eðer kitap bulunamazsa, hata fýrlatýr.

#endregion Book API Endpoint Extensions


app.UseCors("All"); // Use CORS policy // Use "AllowSpecificOrigin" to restrict to a specific origin
app.UseCustomExceptionHandler(); // Özel hata iþleyicisini kullanýr. // Bu metot, global hata yakalama middleware'ini ekler. // Hatalarý JSON formatýnda döndürür. // Bu sayede hata mesajlarýný daha okunabilir hale getirir.
app.UseHttpsRedirection(); // HTTPS yönlendirmesini kullanýr. // HTTP isteklerini HTTPS isteklerine yönlendirir.
app.UseAuthorization(); // Yetkilendirmeyi kullanýr. // Bu middleware, yetkilendirme iþlemlerini yapar. // Kullanýcýlarýn yetkilerini kontrol eder.
app.MapControllers(); // Controller'larý haritalar. // Bu middleware, controller'larý HTTP isteklerine yönlendirir. // Controller'lar, HTTP isteklerini iþler ve yanýt döner.

app.Run(); // Uygulamayý baþlatýr. // Uygulama, HTTP isteklerini dinler ve yönlendirir.
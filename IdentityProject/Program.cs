using IdentityProject.APIs;
using IdentityProject.ConfigurationExtensions;
using IdentityProject.Entities;

var builder = WebApplication.CreateBuilder(args); // Web uygulamasý için yapýlandýrma oluþturur. // Bu metot, uygulama için gerekli yapýlandýrmalarý yapar. // Örneðin, appsettings.json dosyasýný okur ve gerekli hizmetleri ekler.

builder.Services.AddControllers(); // Controller'larý ekler. // Bu metot, API controller'larýný ekler ve HTTP isteklerini yönlendirmek için gerekli middleware'i yapýlandýrýr.
builder.Services.AddEndpointsApiExplorer(); // Endpoint'leri keþfetmek için gerekli hizmetleri ekler. // Bu metot, API endpoint'lerini keþfetmek için gerekli hizmetleri ekler.

#region Custom Configuration And Extensions

builder.Services.AddCustomCors(); // CORS yapýlandýrmasýný ekler. // Bu metot, CORS politikalarýný yapýlandýrýr. // Tüm kaynaklara izin verir.
builder.Services.AddCustomSwagger(); // Swagger yapýlandýrmasýný ekler. // Bu metot, Swagger'ý yapýlandýrýr ve kullanýma hazýr hale getirir.
builder.Services.ServicesIocRegisters(); // IOC (Inversion of Control) kayýtlarýný ekler. // Bu metot, uygulama için gerekli servisleri IOC konteynerine kaydeder. // Örneðin, BookServiceV3 sýnýfýný IBookService arayüzüne kaydeder.
builder.Services.RepositoryIocRegisters(); // Repository kayýtlarýný ekler. // Bu metot, uygulama için gerekli repository'leri IOC konteynerine kaydeder. // Örneðin, BookRepository sýnýfýný IBookRepository arayüzüne kaydeder.
builder.Services.CustomIocRegisters(); // Özel IOC kayýtlarýný ekler. // Bu metot, uygulama için özel servisleri IOC konteynerine kaydeder. // Örneðin, AutoMapper'ý yapýlandýrýr ve kullanýma hazýr hale getirir.
builder.Services.UseSqlServerContext(builder.Configuration); // SQL Server veritabaný baðlantýsýný yapýlandýrýr. // Bu metot, appsettings.json dosyasýndaki ConnectionStrings bölümünden veritabaný baðlantý dizesini alýr ve DbContext'i yapýlandýrýr.

#endregion Custom Configuration And Extensions

var app = builder.Build(); // Uygulamayý oluþturur. // Bu metot, uygulama için gerekli middleware'leri ve hizmetleri yapýlandýrýr. // Örneðin, CORS, Swagger, hata iþleme middleware'lerini ekler.
if (app.Environment.IsDevelopment()) // Uygulama geliþtirme ortamýnda ise Swagger'ý kullanýr. // Bu, uygulamanýn geliþtirme aþamasýnda Swagger arayüzünü gösterir.
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// GET ERROR // TEST METHOD
app.MapGet("/api/error", () =>
{
    throw new Exception("An error has been occured.");
}).Produces<ErrorDetails>(StatusCodes.Status500InternalServerError) // Produces, Swagger'da bu endpoint'in hata kodunu ve hata mesajýný gösterir.
.ExcludeFromDescription(); // ExcludeFromDescription, Swagger'da bu endpoint'i gizler. // Bu endpoint'i Swagger'da görmek istemiyorsanýz kullanabilirsiniz.

#region Book API Endpoint Extensions

app.BookAPIs(); // Kitap API endpoint'lerini ekler. // Bu metot, kitaplarla ilgili API endpoint'lerini ekler. // Örneðin, kitaplarý listeleme, kitap ekleme, kitap güncelleme ve kitap silme iþlemlerini yapar.

#endregion Book API Endpoint Extensions

#region Category API Endpoint Extensions

app.CategoryAPIs();

#endregion Category API Endpoint Extensions

app.UseCors("All"); // Use CORS policy // Use "AllowSpecificOrigin" to restrict to a specific origin
app.UseCustomExceptionHandler(); // Özel hata iþleyicisini kullanýr. // Bu metot, global hata yakalama middleware'ini ekler. // Hatalarý JSON formatýnda döndürür. // Bu sayede hata mesajlarýný daha okunabilir hale getirir.
app.UseHttpsRedirection(); // HTTPS yönlendirmesini kullanýr. // HTTP isteklerini HTTPS isteklerine yönlendirir.
app.UseAuthorization(); // Yetkilendirmeyi kullanýr. // Bu middleware, yetkilendirme iþlemlerini yapar. // Kullanýcýlarýn yetkilerini kontrol eder.
app.MapControllers(); // Controller'larý haritalar. // Bu middleware, controller'larý HTTP isteklerine yönlendirir. // Controller'lar, HTTP isteklerini iþler ve yanýt döner.

app.Run(); // Uygulamayý baþlatýr. // Uygulama, HTTP isteklerini dinler ve yönlendirir.
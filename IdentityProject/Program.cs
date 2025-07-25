using IdentityProject.Abstracts;
using IdentityProject.APIs;
using IdentityProject.ConfigurationExtensions;
using IdentityProject.DTOs.IdentityDTOs;
using IdentityProject.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args); // Web uygulamas� i�in yap�land�rma olu�turur. // Bu metot, uygulama i�in gerekli yap�land�rmalar� yapar. // �rne�in, appsettings.json dosyas�n� okur ve gerekli hizmetleri ekler.

builder.Services.AddControllers(); // Controller'lar� ekler. // Bu metot, API controller'lar�n� ekler ve HTTP isteklerini y�nlendirmek i�in gerekli middleware'i yap�land�r�r.
builder.Services.AddEndpointsApiExplorer(); // Endpoint'leri ke�fetmek i�in gerekli hizmetleri ekler. // Bu metot, API endpoint'lerini ke�fetmek i�in gerekli hizmetleri ekler.

#region Custom Configuration And Extensions

builder.Services.AddCustomCors(); // CORS yap�land�rmas�n� ekler. // Bu metot, CORS politikalar�n� yap�land�r�r. // T�m kaynaklara izin verir.
builder.Services.AddCustomSwagger(); // Swagger yap�land�rmas�n� ekler. // Bu metot, Swagger'� yap�land�r�r ve kullan�ma haz�r hale getirir.
builder.Services.ServicesIocRegisters(); // IOC (Inversion of Control) kay�tlar�n� ekler. // Bu metot, uygulama i�in gerekli servisleri IOC konteynerine kaydeder. // �rne�in, BookServiceV3 s�n�f�n� IBookService aray�z�ne kaydeder.
builder.Services.RepositoryIocRegisters(); // Repository kay�tlar�n� ekler. // Bu metot, uygulama i�in gerekli repository'leri IOC konteynerine kaydeder. // �rne�in, BookRepository s�n�f�n� IBookRepository aray�z�ne kaydeder.
builder.Services.CustomIocRegisters(); // �zel IOC kay�tlar�n� ekler. // Bu metot, uygulama i�in �zel servisleri IOC konteynerine kaydeder. // �rne�in, AutoMapper'� yap�land�r�r ve kullan�ma haz�r hale getirir.
builder.Services.UseSqlServerContext(builder.Configuration); // SQL Server veritaban� ba�lant�s�n� yap�land�r�r. // Bu metot, appsettings.json dosyas�ndaki ConnectionStrings b�l�m�nden veritaban� ba�lant� dizesini al�r ve DbContext'i yap�land�r�r.

#region Identity Configuration

builder.Services.ConfigureIdentity(); // ASP.NET Core Identity yap�land�rmas�n� ekler. // Bu metot, ASP.NET Core Identity'yi yap�land�r�r. // �rne�in, kullan�c� kimlik do�rulama ve yetkilendirme i�lemlerini yapar.

//builder.Services.AddAuthentication(); // Kimlik do�rulama hizmetini ekler. // Bu metot, kimlik do�rulama i�lemlerini yapar. // �rne�in, JWT token ile kimlik do�rulama i�lemlerini yapar.
builder.Services.ConfigureJWT(builder.Configuration); // JWT yap�land�rmas�n� ekler. // Bu metot, JWT token'lar� i�in gerekli yap�land�rmalar� yapar. // �rne�in, JWT token'�n imzalanmas� i�in gerekli anahtarlar� ve s�releri ayarlar.

builder.Services.AddAuthorization(); // Yetkilendirme hizmetini ekler. // Bu metot, yetkilendirme i�lemlerini yapar. // �rne�in, kullan�c�lar�n yetkilerini kontrol eder.
builder.Services.AddAuthenticationService(); // Kimlik do�rulama servisini ekler. // Bu metot, IAuthService aray�z�n� uygulayan AuthenticationService s�n�f�n� IOC konteynerine kaydeder. // B�ylece, kimlik do�rulama i�lemlerini yapabiliriz.

#endregion Identity Configuration

#endregion Custom Configuration And Extensions

var app = builder.Build(); // Uygulamay� olu�turur. // Bu metot, uygulama i�in gerekli middleware'leri ve hizmetleri yap�land�r�r. // �rne�in, CORS, Swagger, hata i�leme middleware'lerini ekler.
if (app.Environment.IsDevelopment()) // Uygulama geli�tirme ortam�nda ise Swagger'� kullan�r. // Bu, uygulaman�n geli�tirme a�amas�nda Swagger aray�z�n� g�sterir.
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// GET ERROR // TEST METHOD
app.MapGet("/api/error", () =>
{
    throw new Exception("An error has been occured.");
}).Produces<ErrorDetails>(StatusCodes.Status500InternalServerError) // Produces, Swagger'da bu endpoint'in hata kodunu ve hata mesaj�n� g�sterir.
.ExcludeFromDescription(); // ExcludeFromDescription, Swagger'da bu endpoint'i gizler. // Bu endpoint'i Swagger'da g�rmek istemiyorsan�z kullanabilirsiniz.

#region Book API Endpoint Extensions

app.BookAPIs(); // Kitap API endpoint'lerini ekler. // Bu metot, kitaplarla ilgili API endpoint'lerini ekler. // �rne�in, kitaplar� listeleme, kitap ekleme, kitap g�ncelleme ve kitap silme i�lemlerini yapar.

#endregion Book API Endpoint Extensions

#region Category API Endpoint Extensions

app.CategoryAPIs();

#endregion Category API Endpoint Extensions

#region Identity-Authentication API Endpoint Extensions

app.AuthenticationAPIs(); // Kimlik do�rulama API endpoint'lerini ekler. // Bu metot, kullan�c� kayd�, giri� ve ��k�� i�lemlerini yapar. // �rne�in, kullan�c� kayd� i�in RegisterUser endpoint'ini ekler. // Kullan�c� giri�i i�in Login endpoint'ini ekler. // Kullan�c� ��k��� i�in Logout endpoint'ini ekler.

#endregion Identity-Authentication API Endpoint Extensions


app.UseCors("All"); // Use CORS policy // Use "AllowSpecificOrigin" to restrict to a specific origin
app.UseCustomExceptionHandler(); // �zel hata i�leyicisini kullan�r. // Bu metot, global hata yakalama middleware'ini ekler. // Hatalar� JSON format�nda d�nd�r�r. // Bu sayede hata mesajlar�n� daha okunabilir hale getirir.
app.UseHttpsRedirection(); // HTTPS y�nlendirmesini kullan�r. // HTTP isteklerini HTTPS isteklerine y�nlendirir.
app.UseAuthentication(); // Kimlik do�rulamay� kullan�r. // Bu middleware, kimlik do�rulama i�lemlerini yapar. // Kullan�c�lar�n kimlik bilgilerini kontrol eder.
app.UseAuthorization(); // Yetkilendirmeyi kullan�r. // Bu middleware, yetkilendirme i�lemlerini yapar. // Kullan�c�lar�n yetkilerini kontrol eder.
app.MapControllers(); // Controller'lar� haritalar. // Bu middleware, controller'lar� HTTP isteklerine y�nlendirir. // Controller'lar, HTTP isteklerini i�ler ve yan�t d�ner.

app.Run(); // Uygulamay� ba�lat�r. // Uygulama, HTTP isteklerini dinler ve y�nlendirir.


using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;

namespace Giris
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            string baslik = "Sefiller";
            string yazar = "Viktor Hugo";
            int sayfaSayisi = 300;

            // Endpoint Tanýmý
            app.MapGet("/kitap", () =>
            {
                var kitapv2 = new Kitap();
                var kitap = new Kitap(baslik, yazar, sayfaSayisi);
                var kitapListesi = new List<Kitap>
                {
                    new Kitap("Sefiller", "Viktor Hugo", 300),
                    new Kitap("1984", "George Orwell", 328),
                    new Kitap("Hayvan Çiftliði", "George Orwell", 112)
                };

                var sonuc = "";

                foreach (var item in kitapListesi)
                {
                    sonuc += item.BilgileriYaz() + "\n";
                }

                return kitapListesi;
            }).WithName("kitap");

            // Film
            app.MapGet("/Film", () =>
            {
                var film = new Film();

                return film; // Json

                //return film.Ad + " " + film.Yonetmen + " " + film.Sure;
            });

            //Ogrenci
            app.MapGet("/Ogrenci", () =>
            {
                return new Ogrenci();
            });

            //Urun
            app.MapGet("/Urun", () =>
            {
                return new Urun();
            });

            //Arac
            app.MapGet("/Arac", () =>
            {
                string marka = "Toyota";
                string model = "Corolla";
                int yil = 2020;

                var arac = new Arac(marka, model, yil);

                return arac.BilgileriYaz();
            });
            app.MapPost("/Arac", (Arac arac) =>
            {
                return arac.BilgileriYaz();
            });

            //Telefon
            app.MapPost("/Telefon", (Telefon telefon) =>
            {
                return telefon.BilgileriYaz();
            });

            //Sehir
            app.MapGet("/Sehir", () =>
            {
                string ad = "Ýstanbul";
                string ulke = "Türkiye";
                int nufus = 18000000;

                var sehir = new Sehir(ad, ulke, nufus);

                return sehir.BilgileriYaz();
            });
            app.MapPost("/Sehir", (Sehir sehir) =>
            {
                return sehir.BilgileriYaz();
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

public class Response
{
    public Response(string msg)
    {
        Message = msg;
        CreatedTime = Convert.ToDateTime(DateTime.Now.ToShortTimeString());
    }

    public string? Message { get; set; }
    public DateTime CreatedTime { get; set; }
}

public class Kullanici
{
    public Kullanici()
    {
        Ad = "Ozan";
        Yas = 33;
    }

    public string? Ad { get; set; }
    public int Yas { get; set; }

    public string Karsila()
    {
        return $"Merhaba {Ad}, {Yas} yaþýndasýn!";
    }
}

public class Kitap
{
    public Kitap()
    {
        Baslik = "Suç Ve Ceza";
        Yazar = "Dostoyevski";
        SayfaSayisi = 500;
    }

    public Kitap(string baslik, string yazar, int sayfaSayisi)
    {
        Baslik = baslik;
        Yazar = yazar;
        SayfaSayisi = sayfaSayisi;
    }

    public string? Baslik { get; set; }
    public string? Yazar { get; set; }
    public int SayfaSayisi { get; set; }

    public string BilgileriYaz()
    {
        return $"Kitap:{Baslik}, Yazar:{Yazar}, Sayfa Sayýsý:{SayfaSayisi}";
    }
}

public class Film
{
    public Film()
    {
        Ad = "Baþlangýç";
        Yonetmen = "Christopher Nolan";
        Sure = 148;
    }

    public string? Ad { get; set; }
    public string? Yonetmen { get; set; }
    public int Sure { get; set; }

    public string BilgileriYaz()
    {
        return $"Film: {Ad}, Yönetmen: {Yonetmen}, Süre: {Sure}";
    }
}

public class Ogrenci
{
    public Ogrenci()
    {
        Isim = "Zeynep";
        Soyisim = "Kara";
        Numara = 101;
    }

    public string? Isim { get; set; }
    public string? Soyisim { get; set; }
    public int Numara { get; set; }

    public string BilgileriYaz()
    {
        return $"{Isim}, {Soyisim}, {Numara}";
    }
}

public class Urun
{
    public Urun()
    {
        UrunAdi = "Kablosuz Mouse";
        Fiyat = 249.99;
        StokAdedi = 125;
    }

    public string? UrunAdi { get; set; }
    public double Fiyat { get; set; }
    public int StokAdedi { get; set; }

    public string BilgileriYaz()
    {
        return $"Ürün: {UrunAdi}, Fiyat: {Fiyat}, Stok Adedi: {StokAdedi}";
    }
}

public class Arac
{
    public Arac(string marka, string model, int yil)
    {
        Marka = marka;
        Model = model;
        Yil = yil;
    }

    public string? Marka { get; set; }
    public string? Model { get; set; }
    public int Yil { get; set; }

    public string BilgileriYaz()
    {
        return $"Araba: {Marka} {Model}, Yýl: {Yil}";
    }
}

public class Telefon
{
    public Telefon(string marka, string model, double fiyat)
    {
        Marka = marka;
        Model = model;
        Fiyat = fiyat;
    }

    public string Marka { get; set; }
    public string Model { get; set; }
    public double Fiyat { get; set; }

    public string BilgileriYaz()
    {
        return $"Telefon: {Marka} {Model}, Fiyat: {Fiyat}";
    }
}

public class Sehir
{
    public Sehir(string ad, string ulke, int nufus)
    {
        Ad = ad;
        Ulke = ulke;
        Nufus = nufus;
    }

    public string Ad { get; set; }
    public string Ulke { get; set; }
    public int Nufus { get; set; }

    public string BilgileriYaz()
    {
        return $"Þehir: {Ad} {Ulke}, Nüfus: {Nufus} ";
    }
}
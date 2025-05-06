using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using Giris.Entities;

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
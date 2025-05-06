namespace Giris.Entities
{
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
}
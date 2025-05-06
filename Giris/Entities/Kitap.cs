namespace Giris.Entities
{
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
            return $"Kitap:{Baslik}, Yazar:{Yazar}, Sayfa Sayısı:{SayfaSayisi}";
        }
    }
}
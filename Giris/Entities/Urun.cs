namespace Giris.Entities
{
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
}
namespace Giris.Entities
{
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
}
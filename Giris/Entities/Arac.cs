namespace Giris.Entities
{
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
            return $"Araba: {Marka} {Model}, Yıl: {Yil}";
        }
    }
}
namespace Giris.Entities
{
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
            return $"Şehir: {Ad} {Ulke}, Nüfus: {Nufus} ";
        }
    }
}
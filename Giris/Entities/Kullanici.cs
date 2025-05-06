namespace Giris.Entities
{
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
            return $"Merhaba {Ad}, {Yas} yaşındasın!";
        }
    }
}
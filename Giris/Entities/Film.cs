namespace Giris.Entities
{
    public class Film
    {
        public Film()
        {
            Ad = "Başlangıç";
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
}
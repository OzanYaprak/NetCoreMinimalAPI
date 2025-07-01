using System.Text.Json;

namespace RelationsProject.Entities
{
    public class ErrorDetails
    {
        public string? ErrorDate { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }

        public override string ToString() // ToString yaz�ld���nda ErrorDetails s�n�f�n� Serialize edecektir.
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
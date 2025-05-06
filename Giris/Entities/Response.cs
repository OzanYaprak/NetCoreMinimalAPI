namespace Giris.Entities
{
    public class Response
    {
        public Response(string msg)
        {
            Message = msg;
            CreatedTime = Convert.ToDateTime(DateTime.Now.ToShortTimeString());
        }

        public string? Message { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
namespace ConfigurationAndExtensions.DTOs
{
    public record BookDtoForUpdate : BookDTO
    {
        public Int32 Id { get; set; }
    }
}
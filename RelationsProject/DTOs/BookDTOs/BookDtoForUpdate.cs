namespace RelationsProject.DTOs.BookDTOs
{
    public record BookDtoForUpdate : BookDTOBase
    {
        public int Id { get; set; }
    }
}
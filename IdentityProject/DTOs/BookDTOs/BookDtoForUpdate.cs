namespace IdentityProject.DTOs.BookDTOs
{
    public record BookDtoForUpdate : BookDTOBase
    {
        public int Id { get; set; }
    }
}
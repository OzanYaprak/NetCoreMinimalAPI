namespace RelationsProject.DTOs
{
    public record BookDtoForUpdate : BookDTOBase
    {
        public Int32 Id { get; set; }
    }
}
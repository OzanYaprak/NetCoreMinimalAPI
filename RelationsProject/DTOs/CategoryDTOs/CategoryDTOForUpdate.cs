namespace RelationsProject.DTOs.CategoryDTOs
{
    public record CategoryDTOForUpdate : CategoryDTOBase
    {
        public int CategoryId { get; set; }
    }
}
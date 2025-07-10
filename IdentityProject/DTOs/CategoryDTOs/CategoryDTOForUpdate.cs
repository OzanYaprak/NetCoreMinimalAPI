namespace IdentityProject.DTOs.CategoryDTOs
{
    public record CategoryDTOForUpdate : CategoryDTOBase
    {
        public int CategoryId { get; set; }
    }
}
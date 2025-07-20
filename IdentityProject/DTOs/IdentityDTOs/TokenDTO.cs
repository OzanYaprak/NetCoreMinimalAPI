namespace IdentityProject.DTOs.IdentityDTOs
{
    public record TokenDTO
    {
        public String AccessToken { get; init; }
        public String RefreshToken { get; init; }
    }
}
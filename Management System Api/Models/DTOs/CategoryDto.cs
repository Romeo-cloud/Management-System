namespace Management_System_Api.Models.DTOs
{
    // Create
    public record CategoryCreateDto(string Name);

    // Update
    public record CategoryUpdateDto(string Name);

    // Response / Read
    public record CategoryResponseDto(int Id, string Name);
}

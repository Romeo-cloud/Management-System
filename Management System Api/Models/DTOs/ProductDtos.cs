namespace Management_System_Api.Models.DTOs
{
    public record ProductCreateDto(
        string ProductId,
        string Name,
        string? Description,
        decimal Price,
        int Quantity,
        int CategoryId // required when creating
    );

    public record ProductUpdateDto(
        string Name,
        string? Description,
        decimal Price,
        int Quantity,
        bool IsActive,
        int? CategoryId // optional when updating
    );

    public record ProductResponseDto(
        string ProductId,
        string Name,
        string? Description,
        decimal Price,
        int Quantity,
        bool IsActive,
        int CategoryId,
        string CategoryName // so client sees name
    );
}

namespace Management_System_Api.Models.DTOs;

public record SaleCreateDto(string ProductId, int Quantity, string? CustomerName, string? CustomerContact);
public record SaleResponseDto(int Id, string ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal TotalPrice, string SoldByUserId, DateTime SoldAtUtc, int? InvoiceId, string? InvoiceNumber);
using Management_System_Api.Models.DTOs;

namespace Management_System_Api.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResponseDto> GenerateForSaleAsync(int saleId, string businessName, string businessContact);
        Task<byte[]> GetPdfAsync(int invoiceId);
    }
}

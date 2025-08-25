using Management_System_Api.Data;
using Management_System_Api.Models.DTOs;
using Management_System_Api.Services.Interfaces;
using Management_System_Api.Models.Domain;
using Management_System_Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Management_System_Api.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _db;
        public InvoiceService(AppDbContext db) { _db = db; }

        public async Task<InvoiceResponseDto> GenerateForSaleAsync(int saleId, string businessName, string businessContact)
        {
            var sale = await _db.Sales.Include(s => s.Product).FirstOrDefaultAsync(s => s.Id == saleId)
            ?? throw new NotFoundException($"Sale {saleId} not found");
            if (sale.Invoice is not null) throw new BadRequestException("Invoice already generated for this sale");


            var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{sale.Id}";
            var pdfBytes = GenerateInvoicePdf(invoiceNumber, businessName, businessContact, sale);


            var invoice = new Invoice { InvoiceNumber = invoiceNumber, SaleId = sale.Id, PdfBytes = pdfBytes };
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();


            return new InvoiceResponseDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CreatedAt = invoice.CreatedAtUtc,
                Sales = new List<SaleResponseDto>() // can be filled later if needed
            };

        }
        public async Task<byte[]> GetPdfAsync(int invoiceId)
        {
            var invoice = await _db.Invoices.FindAsync(invoiceId) ?? throw new NotFoundException($"Invoice {invoiceId} not found");
            return invoice.PdfBytes;
        }


        private static byte[] GenerateInvoicePdf(string number, string business, string contact, Sale sale)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            using var ms = new MemoryStream();


            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header().Text($"{business}").FontSize(20).SemiBold();
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Contact: {contact}");
                        col.Item().Text($"Invoice #: {number}");
                        col.Item().Text($"Date (UTC): {DateTime.UtcNow:yyyy-MM-dd HH:mm}");
                        col.Item().LineHorizontal(1);


                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c => { c.RelativeColumn(); c.ConstantColumn(60); c.ConstantColumn(80); c.ConstantColumn(90); });
                            t.Header(h => { h.Cell().Text("Product"); h.Cell().Text("Qty"); h.Cell().Text("Unit"); h.Cell().Text("Total"); });
                            t.Cell().Text($"{sale.Product.Name} ({sale.ProductId})");
                            t.Cell().Text(sale.QuantitySold.ToString());
                            t.Cell().Text(sale.UnitPrice.ToString("0.00"));
                            t.Cell().Text(sale.TotalPrice.ToString("0.00"));
                        });


                        col.Item().LineHorizontal(1);
                        col.Item().Text($"Customer: {sale.CustomerName ?? "-"} | {sale.CustomerContact ?? "-"}");
                        col.Item().Text($"Sold by: {sale.SoldByUserId} at {sale.SoldAtUtc:yyyy-MM-dd HH:mm} UTC");
                    });
                    page.Footer().AlignRight().Text("Thank you!");
                });
            }).GeneratePdf(ms);


            return ms.ToArray();
        }
    }
}

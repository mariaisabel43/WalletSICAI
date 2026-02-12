using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WalletSICAI.Models;

namespace WalletSICAI.Services
{


    public class ReportService
    {
        private readonly WalletContext _context;

        public ReportService(WalletContext context)
        {
            _context = context;
        }
        public async Task<List<Recarga>> ObtenerRecargasPorFecha(DateTime fecha)
        {
            return await _context.Recargas
                .Where(r => r.FechaRecarga == DateOnly.FromDateTime(fecha))
                .ToListAsync();
        }

        public async Task<byte[]> GenerarReporteRecargasPorFecha(DateTime fecha)
        {
            var recargas = await ObtenerRecargasPorFecha(fecha);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header()
                        .Text($"Reporte de Recargas - {fecha:dd/MM/yyyy}")
                        .FontSize(20).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(80);   // Fecha
                            columns.RelativeColumn();     // Nombre completo
                            columns.RelativeColumn();     // Modo de pago
                            columns.RelativeColumn();     // Monto
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Fecha").Bold();
                            header.Cell().Text("Solicitante").Bold();
                            header.Cell().Text("Modo Pago").Bold();
                            header.Cell().Text("Monto").Bold();
                        });

                        foreach (var r in recargas)
                        {
                            table.Cell().Text(r.FechaRecarga?.ToString("dd/MM/yyyy"));
                            table.Cell().Text(r.SolicitanteRecargaNombreCompleto);
                            table.Cell().Text(r.ModoPagoRecarga);
                            table.Cell().Text(r.MontoRecarga?.ToString("C"));
                        }
                    });

                    page.Footer()
                        .AlignRight()
                        .Text($"Total: {recargas.Sum(r => r.MontoRecarga ?? 0):C}")
                        .Bold();
                });
            });

            return document.GeneratePdf();
        }

    }
}

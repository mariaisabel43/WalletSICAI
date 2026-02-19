using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WalletSICAI.Models;
using WalletSICAI.viewModels;

namespace WalletSICAI.Services
{


    public class ReportService
    {
        private readonly WalletContext _context;

        public ReportService(WalletContext context)
        {
            _context = context;
        }
        
        // Método para obtener recargas filtradas
        public async Task<List<Recarga>> ObtenerRecargas(DateTime fechaInicio, DateTime? fechaFin, string? solicitante = null) 
       { 
            var query = _context.Recargas.AsQueryable(); 
            // Filtrar por fecha inicio
            query = query.Where(r => r.FechaRecarga >= DateOnly.FromDateTime(fechaInicio)); 
            // Filtrar por fecha fin si existe
            if (fechaFin.HasValue) 
                query = query.Where(r => r.FechaRecarga <= DateOnly.FromDateTime(fechaFin.Value)); 
            // Filtrar por solicitante (nombre o cédula)
            if (!string.IsNullOrEmpty(solicitante)) 
            { 
                query = query.Where(r => r.SolicitanteRecargaCedula == solicitante || 
                r.SolicitanteRecargaNombreCompleto.Contains(solicitante)); 
            } 
            return await query.ToListAsync(); 
        } 
        // Método para generar el PDF
        public async Task<byte[]> GenerarReporteRecargas(DateTime fechaInicio, DateTime? fechaFin, string? solicitante = null) 
        { 
            var recargas = await ObtenerRecargas(fechaInicio, fechaFin, solicitante); 
            var document = Document.Create(container => 
            { 
                container.Page(page => 
                { 
                    page.Margin(30); 
                    page.Header() 
                        .Text(fechaFin.HasValue 
                            ? $"Reporte de Recargas - {fechaInicio:dd/MM/yyyy} a {fechaFin:dd/MM/yyyy}" 
                            : $"Reporte de Recargas - {fechaInicio:dd/MM/yyyy}") 
                        .FontSize(20).Bold().AlignCenter(); 
                    page.Content().Table(table => 
                    { 
                        table.ColumnsDefinition(columns => 
                        { 
                            columns.ConstantColumn(80); // Fecha
                            columns.RelativeColumn(); // Nombre completo
                            columns.RelativeColumn(); // Cédula
                            columns.RelativeColumn(); // Modo de pago
                            columns.RelativeColumn(); // Monto
                        }); 
                        table.Header(header => 
                        { 
                            header.Cell().Text("Fecha").Bold(); 
                            header.Cell().Text("Solicitante").Bold(); 
                            header.Cell().Text("Cédula").Bold();
                            header.Cell().Text("Modo Pago").Bold(); 
                            header.Cell().Text("Monto").Bold(); 
                        }); 
                        foreach (var r in recargas) 
                        { 
                            table.Cell().Text(r.FechaRecarga?.ToString("dd/MM/yyyy")); 
                            table.Cell().Text(r.SolicitanteRecargaNombreCompleto); 
                            table.Cell().Text(r.SolicitanteRecargaCedula); 
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
        // Método para llenar el dropdown de solicitantes
        public List<SolicitanteViewModel> ObtenerSolicitantes() 
        { 
            return _context.Recargas 
                .Select(r => new SolicitanteViewModel 
                {
                    SolicitanteRecargaCedula = r.SolicitanteRecargaCedula,
                    SolicitanteRecargaNombreCompleto = r.SolicitanteRecargaNombreCompleto 
                }) 
                .Distinct() 
                .ToList(); 
        } 
    }
}

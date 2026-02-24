using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
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
            var query = _context.Recargas
                .Include(r => r.Estudiante) // Trae el estudiante relacionado
                .AsQueryable();

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
        //public async Task<byte[]> GenerarReporteRecargas(DateTime fechaInicio, DateTime? fechaFin, string? solicitante = null) 
        //{ 
        //    var recargas = await ObtenerRecargas(fechaInicio, fechaFin, solicitante); 
        //    var document = Document.Create(container => 
        //    { 
        //        container.Page(page => 
        //        { 
        //            page.Margin(30); 
        //            page.Header() 
        //                .Text(fechaFin.HasValue 
        //                    ? $"Reporte de Recargas - {fechaInicio:dd/MM/yyyy} a {fechaFin:dd/MM/yyyy}" 
        //                    : $"Reporte de Recargas - {fechaInicio:dd/MM/yyyy}") 
        //                .FontSize(20).Bold().AlignCenter(); 
        //            page.Content().Table(table => 
        //            { 
        //                table.ColumnsDefinition(columns => 
        //                { 
        //                    columns.ConstantColumn(80); // Fecha
        //                    columns.RelativeColumn(); // Nombre completo Solicitante
        //                    columns.RelativeColumn(); // Cédula Solicitante
        //                    /*columns.RelativeColumn();*/ // Nombre completo estudiante
        //                    columns.RelativeColumn(); // Modo de pago
        //                    columns.RelativeColumn(); // Monto

        //                }); 
        //                table.Header(header => 
        //                { 
        //                    header.Cell().Text("Fecha").Bold(); 
        //                    header.Cell().Text("Solicitante").Bold(); 
        //                    header.Cell().Text("Cédula").Bold();
        //                    //header.Cell().Text("Nombre Est").Bold();
        //                    header.Cell().Text("Modo Pago").Bold(); 
        //                    header.Cell().Text("Monto").Bold(); 
        //                }); 
        //                foreach (var r in recargas) 
        //                { 
        //                    table.Cell().Text(r.FechaRecarga?.ToString("dd/MM/yyyy")); 
        //                    table.Cell().Text(r.SolicitanteRecargaNombreCompleto); 
        //                    table.Cell().Text(r.SolicitanteRecargaCedula);
        //                    //table.Cell().Text(r.NombreCompletoEstudiante);
        //                    table.Cell().Text(r.ModoPagoRecarga); 
        //                    table.Cell().Text(r.MontoRecarga?.ToString("C")); 
        //                } 
        //            }); 
        //                page.Footer() 
        //                    .AlignRight() 
        //                    .Text($"Total: {recargas.Sum(r => r.MontoRecarga ?? 0):C}") 
        //                    .Bold(); 
        //        }); 
        //    });
        //    return document.GeneratePdf(); 

        //}

       

        public async Task<byte[]> GenerarReporteRecargas(
    DateTime fechaInicio,
    DateTime? fechaFin,
    string? solicitante = null)
        {
            var recargas = await ObtenerRecargas(fechaInicio, fechaFin, solicitante);

            var logoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "images", "logo-wallet-sicai.png");
                        
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(35);
                    page.PageColor(Colors.White);

                    /* =========================
                              HEADER 
                    ========================= */
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Spacing(6);

                            column.Item()
                                .Text("Wallet SICAI")
                                .FontSize(14)
                                .Bold()
                                .FontColor(Colors.Grey.Darken2);

                            column.Item()
                                .Text("Reporte – Historial de Recargas")
                                .FontSize(20)
                                .Bold()
                                .FontColor(Colors.Black);

                            column.Item()
                                .Text(fechaFin.HasValue
                                    ? $"Periodo: {fechaInicio:dd/MM/yyyy} al {fechaFin:dd/MM/yyyy}"
                                    : $"Fecha: {fechaInicio:dd/MM/yyyy}")
                                .FontSize(11)
                                .FontColor(Colors.Grey.Darken1);

                            if (!string.IsNullOrWhiteSpace(solicitante))
                            {
                                column.Item()
                                    .Text($"Solicitante: {solicitante}")
                                    .FontSize(11)
                                    .FontColor(Colors.Grey.Darken1);
                            }
                        });

                        if (File.Exists(logoPath))
                        {
                            row.ConstantItem(50)
                                .AlignRight()
                                .AlignMiddle()
                                .Element(container =>
                                {
                                    container.Image(logoPath, ImageScaling.FitArea);
                                });
                        }
                    });

                    /* =========================
                             CONTENIDO
                    ========================= */
                    page.Content().PaddingTop(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(80);   // Fecha
                            columns.RelativeColumn();     // Solicitante
                            columns.RelativeColumn();     // Cédula
                            columns.RelativeColumn();     // Nombre completo estudiante
                            columns.RelativeColumn();     // Modo de pago
                            columns.RelativeColumn();     // Monto
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(7)
                                .AlignCenter().Text("Fecha").Bold();

                            header.Cell().Background(Colors.Grey.Lighten3).Padding(7)
                                .Text("Solicitante").Bold();

                            header.Cell().Background(Colors.Grey.Lighten3).Padding(7)
                                .Text("Cédula").Bold();

                            header.Cell().Background(Colors.Grey.Lighten3).Padding(7)
                                .Text("Estudiante").Bold();


                            header.Cell().Background(Colors.Grey.Lighten3).Padding(7)
                                .AlignCenter().Text("Modo de Pago").Bold();

                            header.Cell().Background(Colors.Grey.Lighten3).Padding(7)
                                .AlignRight().Text("Monto").Bold();
                        });

                        bool filaAlterna = false;

                        foreach (var r in recargas)
                        {
                            var fondo = filaAlterna ? Colors.Grey.Lighten5 : Colors.White;
                            filaAlterna = !filaAlterna;

                            table.Cell().Background(fondo).Padding(6)
                                .AlignCenter()
                                .Text(r.FechaRecarga?.ToString("dd/MM/yyyy"));

                            table.Cell().Background(fondo).Padding(6)
                                .Text(r.SolicitanteRecargaNombreCompleto);

                            table.Cell().Background(fondo).Padding(6)
                                .Text(r.SolicitanteRecargaCedula);

                            table.Cell().Background(fondo).Padding(6)
                                .Text(r.Estudiante?.EstudianteNombreCompleto);

                            table.Cell().Background(fondo).Padding(6)
                                .AlignCenter()
                                .Text(r.ModoPagoRecarga);

                            table.Cell().Background(fondo).Padding(6)
                                .AlignRight()
                                .Text(r.MontoRecarga?.ToString("C"));
                        }
                    });

                    /* =========================
                               FOOTER 
                    ========================= */
                    page.Footer().Row(row =>
                    {
                        row.RelativeItem()
                            .Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);

                        row.ConstantItem(150)
                            .AlignCenter()
                            .Text(text =>
                            {
                                text.DefaultTextStyle(style =>
                                    style.FontSize(9)
                                        .FontColor(Colors.Grey.Darken1));

                                text.Span("Página ");
                                text.CurrentPageNumber();
                                text.Span(" de ");
                                text.TotalPages();
                            });

                        row.ConstantItem(200)
                            .AlignRight()
                            .Text($"Total General: {recargas.Sum(r => r.MontoRecarga ?? 0):C}")
                            .Bold()
                            .FontSize(12);
                    });
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

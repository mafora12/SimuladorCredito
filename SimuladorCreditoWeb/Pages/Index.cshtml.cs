using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimuladorCredito.Models;
using SimuladorCredito.Services;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;


namespace SimuladorCreditoWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SimuladorFacade _simulador;

        public IndexModel(SimuladorFacade simulador)
        {
            _simulador = simulador;
        }
        [BindProperty] public int Opcion { get; set; }
        [BindProperty] public decimal Monto { get; set; }
        [BindProperty] public double Tasa { get; set; }
        [BindProperty] public string? Tipo { get; set; }
        [BindProperty] public string? Clase { get; set; }
        [BindProperty] public int Capitalizaciones { get; set; }
        [BindProperty] public int PagosAnuales { get; set; }
        [BindProperty] public int Plazo { get; set; }
        [BindProperty] public bool UsarExtra { get; set; }
        [BindProperty] public List<int> PeriodosExtra { get; set; } = new();
        [BindProperty] public List<decimal> MontoExtra { get; set; } = new();

        public List<Cuota> Tabla { get; set; } = new();

        public void OnPost()
        {
            try
            {
                var abonos = UsarExtra ? ObtenerAbonos() : new Dictionary<int, decimal>();


                PeriodosExtra = abonos.Keys.ToList();
                MontoExtra = abonos.Values.ToList();

                Tabla = _simulador.GenerarSimulacion(Opcion, Monto, Tasa, Tipo, Clase, Capitalizaciones, PagosAnuales, Plazo, abonos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Tabla = new List<Cuota>();
            }
        }
        private Dictionary<int, decimal> ObtenerAbonos()
        {
            var dic = new Dictionary<int, decimal>();
            for (int i = 0; i < PeriodosExtra.Count; i++)
            {
                int p = PeriodosExtra[i];
                decimal m = i < MontoExtra.Count ? MontoExtra[i] : 0;
                if (p > 0 && m > 0 && !dic.ContainsKey(p))
                    dic[p] = m;
            }
            return dic;
        }


        public IActionResult OnPostExportExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Mariana Florez");
            using var package = new ExcelPackage();
            var abonos = UsarExtra ? ObtenerAbonos() : new Dictionary<int, decimal>();
            var tabla = _simulador.GenerarSimulacion(
                Opcion, Monto, Tasa, Tipo, Clase,
                Capitalizaciones, PagosAnuales, Plazo, abonos);
            var ws = package.Workbook.Worksheets.Add("Simulacion");
            ws.Cells[1, 1].Value = "Periodo";
            ws.Cells[1, 2].Value = "Cuota";
            ws.Cells[1, 3].Value = "Interes";
            ws.Cells[1, 4].Value = "Capital";
            ws.Cells[1, 5].Value = "Extra";
            ws.Cells[1, 6].Value = "Saldo";


            int fila = 2;
            foreach (var c in tabla)
            {
                ws.Cells[fila, 1].Value = c.Numero;
                ws.Cells[fila, 2].Value = c.ValorCuota;
                ws.Cells[fila, 3].Value = c.Interes;
                ws.Cells[fila, 4].Value = c.AbonoCapital;
                ws.Cells[fila, 5].Value = c.AbonoExtraordinario;
                ws.Cells[fila, 6].Value = c.SaldoRestante;
                fila++;
            }

            int ultimaFila = fila - 1;

            var chart = ws.Drawings.AddLineChart("GraficaCredito", eLineChartType.Line);
            chart.Title.Text = "Comportamiento del Crédito";
            chart.SetPosition(fila + 1, 0, 0, 0);
            chart.SetSize(800, 400);

            var serieSaldo = chart.Series.Add(
                ws.Cells[2, 6, ultimaFila, 6],
                ws.Cells[2, 1, ultimaFila, 1]);
            serieSaldo.Header = "Saldo";

            var chart2 = (ExcelLineChart)chart.PlotArea.ChartTypes.Add(eChartType.Line);

            var serieInteres = chart2.Series.Add(
                ws.Cells[2, 3, ultimaFila, 3],
                ws.Cells[2, 1, ultimaFila, 1]);
            serieInteres.Header = "Interés";

            var serieCapital = chart2.Series.Add(
                ws.Cells[2, 4, ultimaFila, 4],
                ws.Cells[2, 1, ultimaFila, 1]);
            serieCapital.Header = "Capital";

            chart2.UseSecondaryAxis = true;
            chart2.XAxis.Deleted = true;

            chart.Legend.Position = eLegendPosition.Bottom;

            var bytes = package.GetAsByteArray();
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "SimulacionCredito.xlsx");
        }
    }
}
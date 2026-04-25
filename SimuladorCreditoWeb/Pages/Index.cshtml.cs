using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimuladorCredito.Models;
using SimuladorCredito.Services;
using OfficeOpenXml;


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

        public List<Cuota> Tabla { get; set; } = new();

        public void OnPost()
        {

            try
            {
                Tabla = _simulador.GenerarSimulacion(Opcion, Monto, Tasa, Tipo, Clase, Capitalizaciones, PagosAnuales, Plazo, UsarExtra);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Tabla = new List<Cuota>();
            }

        }    



public IActionResult OnPostExportExcel()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Mariana Florez");
            using var package = new ExcelPackage();



            double tasaInput = Tasa / 100;

            double ea = ConversorTasas.ConvertirAEfectivaMensual(
                tasaInput, Tipo, Clase, Capitalizaciones);

            double tasaPeriodica = ConversorTasas.CalcularTasaPeriodica(
                ea, PagosAnuales);

            Credito miCredito;

            switch (Opcion)
            {
                case 2:
                    miCredito = new CreditoAbonoConstante(Monto, tasaPeriodica, Plazo);
                    break;
                case 3:
                    miCredito = new CreditoTasaVariableCuotaFija(Monto, tasaPeriodica, Plazo);
                    break;
                default:
                    miCredito = new CreditoCuotaFija(Monto, tasaPeriodica, Plazo);
                    break;
            }

            if (UsarExtra)
            {
                miCredito.AbonosExtraordinarios.Add(3, 1000000);
            }

            var tabla = _simulador.GenerarSimulacion(Opcion, Monto, Tasa, Tipo, Clase, Capitalizaciones, PagosAnuales, Plazo, UsarExtra);

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

            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SimulacionCredito.xlsx");
        }
    }
}
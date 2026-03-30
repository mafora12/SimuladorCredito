using System;
using SimuladorCredito.Models;
using SimuladorCredito.Services;

namespace SimuladorCredito
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== SIMULADOR DE CRÉDITO PROFESIONAL ===\n");

            Console.WriteLine("Seleccione modalidad: 1. Cuota Fija, 2. Abono Constante, 3. Variable-Cuota Fija");
            int opcion = int.Parse(Console.ReadLine() ?? "0");


            Console.Write("Monto del crédito: ");
            decimal monto = decimal.Parse(Console.ReadLine() ?? "0");

            Console.Write("Tasa de interés (%): ");
            double tasaInput = double.Parse(Console.ReadLine() ?? "0") / 100;

            Console.Write("Tipo (Nominal/Efectiva): ");
            string? tipo = Console.ReadLine();

            Console.Write("Clase (Vencida/Anticipada): ");
            string? clase = Console.ReadLine();

            Console.Write("Frecuencia de la tasa (Capitalizaciones al año, ej: Mensual=12): ");
            int cap = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Frecuencia de pago (Pagos al año, ej: Mensual=12): ");
            int pagosAnuales = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Plazo total (Número de períodos): ");
            int plazo = int.Parse(Console.ReadLine() ?? "0");

            
            double ea = ConversorTasas.ConvertirAEfectivaMensual(tasaInput, tipo ?? "", clase ?? "", cap);
            double tasaPeriodica = ConversorTasas.CalcularTasaPeriodica(ea, pagosAnuales);

            Console.WriteLine($"\nTasa Efectiva Anual: {ea:P2}");
            Console.WriteLine($"Tasa Periódica Aplicada: {tasaPeriodica:P4}\n");

            
            Credito miCredito = new CreditoCuotaFija(monto, tasaPeriodica, plazo);
            switch (opcion)
            {
                case 2:
                    miCredito = new CreditoAbonoConstante(monto, tasaPeriodica, plazo);
                    break;
                case 3:
                    miCredito = new CreditoTasaVariableCuotaFija(monto, tasaPeriodica, plazo);
                    break;
                default:
                    miCredito = new CreditoCuotaFija(monto, tasaPeriodica, plazo);
                    break;
            }


            Console.Write("¿Desea agregar un abono extraordinario en el periodo 3? (s/n): ");
            if ((Console.ReadLine() ?? "").ToLower() == "s")
            {
                miCredito.AbonosExtraordinarios.Add(3, 1000000); 
            }

            var tabla = miCredito.GenerarTabla();

        
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("| Per |    Cuota    |   Interés   |   Capital   |    Extra    |    Saldo    |");
            Console.WriteLine("-------------------------------------------------------------------------------");
            foreach (var c in tabla)
            {
                Console.WriteLine($"| {c.Numero,-3} | {c.ValorCuota,11:C} | {c.Interes,11:C} | {c.AbonoCapital,11:C} | {c.AbonoExtraordinario,11:C} | {c.SaldoRestante,11:C} |");
            }
            Console.WriteLine("-------------------------------------------------------------------------------");

            Console.ReadLine();
        }
    }
}
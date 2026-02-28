using System;
using SimuladorCredito.Models;
using SimuladorCredito.Services;

namespace SimuladorCredito
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== SIMULADOR DE CRÉDITO ===\n");

            Console.Write("Monto del crédito: ");
            decimal principal = decimal.Parse(Console.ReadLine());

            Console.Write("Tasa (%): ");
            double tasa = double.Parse(Console.ReadLine()) / 100;

            Console.Write("Tipo (nominal/efectiva): ");
            string tipo = Console.ReadLine();

            Console.Write("Clase (vencida/anticipada): ");
            string clase = Console.ReadLine();

            Console.Write("Capitalizaciones por año: ");
            int capitalizaciones = int.Parse(Console.ReadLine());

            Console.Write("Pagos por año: ");
            int pagos = int.Parse(Console.ReadLine());

            Console.Write("Número de periodos: ");
            int periodos = int.Parse(Console.ReadLine());

            double tasaPeriodo = ConversorTasas.ParsearTasa(
                tasa, tipo, clase, capitalizaciones, pagos);

            Console.WriteLine($"\nTasa por periodo: {tasaPeriodo * 100:F4}%\n");

            // POLIMORFISMO
            Credito credito = new CreditoFrances(principal, tasaPeriodo, periodos);

            var tabla = credito.GenerarTabla();

            foreach (var cuota in tabla)
            {
                Console.WriteLine(
                    $"Periodo {cuota.Numero} | " +
                    $"Cuota: {cuota.CuotaValor:C} | " +
                    $"Interés: {cuota.Interes:C} | " +
                    $"Capital: {cuota.AbonoCapital:C} | " +
                    $"Saldo: {cuota.SaldoRestante:C}");
            }

            Console.ReadLine();
        }
    }
}
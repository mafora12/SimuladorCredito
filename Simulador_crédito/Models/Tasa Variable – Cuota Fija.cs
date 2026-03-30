using System;
using System.Collections.Generic;
using System.Numerics;

namespace SimuladorCredito.Models
{
    public class CreditoTasaVariableCuotaFija : Credito
    {
        public CreditoTasaVariableCuotaFija(decimal monto, double tasa, int plazo)
            : base(monto, tasa, plazo) { }

        public override List<Cuota> GenerarTabla()
        {
            var tabla = new List<Cuota>();
            decimal saldo = Monto;


            decimal cuotaEstablecida = CalcularCuotaFija(Monto, TasaPeriodica, Plazo);

            for (int i = 1; i <= Plazo; i++)
            {
                decimal interes = saldo * (decimal)TasaPeriodica;
                decimal abonoPrincipal = cuotaEstablecida - interes;

                if (abonoPrincipal < 0) abonoPrincipal = 0;

                decimal extra = AbonosExtraordinarios.ContainsKey(i) ? AbonosExtraordinarios[i] : 0;

                saldo -= (abonoPrincipal + extra);
                if (saldo < 0) saldo = 0;

                tabla.Add(new Cuota
                {
                    Numero = i,
                    ValorCuota = Math.Round(cuotaEstablecida, 2),
                    Interes = Math.Round(interes, 2),
                    AbonoCapital = Math.Round(abonoPrincipal, 2),
                    AbonoExtraordinario = Math.Round(extra, 2),
                    SaldoRestante = Math.Round(saldo, 2)
                });

                if (saldo <= 0) break;
            }
            return tabla;
        }

        private decimal CalcularCuotaFija(decimal p, double i, int n)
        {
            if (i == 0) return p / n;
            return p * (decimal)((i * Math.Pow(1 + i, n)) / (Math.Pow(1 + i, n) - 1));
        }
    }
}
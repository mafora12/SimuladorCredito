using System;
using System.Collections.Generic;
using System.Numerics;

namespace SimuladorCredito.Models
{
    public class CreditoCuotaFija : Credito
    {
        public CreditoCuotaFija(decimal monto, double tasa, int plazo) : base(monto, tasa, plazo) { }

        private decimal CalcularCuotaFija(decimal saldoActual, int periodosRestantes)
        {
            if (TasaPeriodica == 0) return saldoActual / periodosRestantes;

            double factor = (TasaPeriodica * Math.Pow(1 + TasaPeriodica, periodosRestantes)) /
                           (Math.Pow(1 + TasaPeriodica, periodosRestantes) - 1);
            return saldoActual * (decimal)factor;
        }

        public override List<Cuota> GenerarTabla()
        {
            var tabla = new List<Cuota>();
            decimal saldo = Monto;

            for (int i = 1; i <= Plazo; i++)
            {

                decimal cuotaBase = CalcularCuotaFija(saldo, Plazo - i + 1);
                decimal interes = saldo * (decimal)TasaPeriodica;
                decimal abonoPrincipal = cuotaBase - interes;

                decimal extra = AbonosExtraordinarios.ContainsKey(i) ? AbonosExtraordinarios[i] : 0;

                saldo -= (abonoPrincipal + extra);
                if (saldo < 0) saldo = 0;

                tabla.Add(new Cuota
                {
                    Numero = i,
                    ValorCuota = Math.Round(cuotaBase, 2),
                    Interes = Math.Round(interes, 2),
                    AbonoCapital = Math.Round(abonoPrincipal, 2),
                    AbonoExtraordinario = Math.Round(extra, 2),
                    SaldoRestante = Math.Round(saldo, 2)
                });

                if (saldo <= 0) break;
            }
            return tabla;
        }
    }
}



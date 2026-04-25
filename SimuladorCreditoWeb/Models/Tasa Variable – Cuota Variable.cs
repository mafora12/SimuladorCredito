using System;
using System.Collections.Generic;
using System.Numerics;

namespace SimuladorCredito.Models
{
    
    public class CreditoAbonoConstante : Credito
    {
        public CreditoAbonoConstante(decimal monto, double tasa, int plazo)
            : base(monto, tasa, plazo) { }

        public override List<Cuota> GenerarTabla()
        {
            var tabla = new List<Cuota>();
            decimal saldo = Monto;
            decimal abonoCapitalFijo = Monto / Plazo;

            for (int i = 1; i <= Plazo; i++)
            {
                
                decimal interes = saldo * (decimal)TasaPeriodica;

               
                decimal abonoPrincipal = abonoCapitalFijo;

                
                decimal valorCuota = abonoPrincipal + interes;

                
                decimal extra = AbonosExtraordinarios.ContainsKey(i) ? AbonosExtraordinarios[i] : 0;

                saldo -= (abonoPrincipal + extra);
                if (saldo < 0) saldo = 0;

                tabla.Add(new Cuota
                {
                    Numero = i,
                    ValorCuota = Math.Round(valorCuota, 2),
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
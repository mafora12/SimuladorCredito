using System;
using System.Collections.Generic;

namespace SimuladorCredito.Models
{
    public class CreditoFrances : Credito
    {
        public CreditoFrances(decimal principal, double tasa, int numeroPeriodos)
            : base(principal, tasa, numeroPeriodos)
        {
        }

        private decimal CalcularCuota()
        {
            if (Tasa == 0)
                return Principal / NumeroPeriodos;

            double factor = (Tasa * Math.Pow(1 + Tasa, NumeroPeriodos)) /
                            (Math.Pow(1 + Tasa, NumeroPeriodos) - 1);

            return Principal * (decimal)factor;
        }

        public override List<Cuota> GenerarTabla()
        {
            var tabla = new List<Cuota>();
            decimal saldo = Principal;
            decimal cuota = CalcularCuota();

            for (int i = 1; i <= NumeroPeriodos; i++)
            {
                decimal interes = saldo * (decimal)Tasa;
                decimal abonoCapital = cuota - interes;
                saldo -= abonoCapital;

                if (saldo < 0)
                    saldo = 0;

                tabla.Add(new Cuota
                {
                    Numero = i,
                    CuotaValor = Math.Round(cuota, 2),
                    Interes = Math.Round(interes, 2),
                    AbonoCapital = Math.Round(abonoCapital, 2),
                    SaldoRestante = Math.Round(saldo, 2)
                });

                if (saldo == 0)
                    break;
            }

            return tabla;
        }
    }
}
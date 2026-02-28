using System;
using System.Collections.Generic;

namespace SimuladorCredito.Models
{
    public abstract class Credito
    {
        public decimal Principal { get; protected set; }
        public double Tasa { get; protected set; }
        public int NumeroPeriodos { get; protected set; }

        protected Credito(decimal principal, double tasa, int numeroPeriodos)
        {
            Principal = principal;
            Tasa = tasa;
            NumeroPeriodos = numeroPeriodos;
        }

        // POLIMORFISMO
        public abstract List<Cuota> GenerarTabla();
    }
}
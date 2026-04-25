using System.Collections.Generic;

namespace SimuladorCredito.Models
{
    public abstract class Credito
    {
        
        public decimal Monto { get; protected set; }
        public double TasaPeriodica { get; protected set; }
        public int Plazo { get; protected set; }
        public Dictionary<int, decimal> AbonosExtraordinarios { get; set; } = new Dictionary<int, decimal>();

        protected Credito(decimal monto, double tasaPeriodica, int plazo)
        {
            Monto = monto;
            TasaPeriodica = tasaPeriodica;
            Plazo = plazo;
        }

        public abstract List<Cuota> GenerarTabla();
    }
}
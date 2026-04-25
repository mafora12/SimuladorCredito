namespace SimuladorCredito.Models
{
    public class Cuota
    {
        public int Numero { get; set; }
        public decimal ValorCuota { get; set; }
        public decimal Interes { get; set; }
        public decimal AbonoCapital { get; set; }
        public decimal AbonoExtraordinario { get; set; }
        public decimal SaldoRestante { get; set; }
    }
}
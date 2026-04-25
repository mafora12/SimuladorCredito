using SimuladorCredito.Models;

namespace SimuladorCredito.Services
{
    public class CreditoFactory
    {


        public Credito CrearCredito(int opcion, decimal monto, double tasaPeriodica, int plazo)
        {
            return opcion switch
            {
                2 => new CreditoAbonoConstante(monto, tasaPeriodica, plazo),
                3 => new CreditoTasaVariableCuotaFija(monto, tasaPeriodica, plazo),
                _ => new CreditoCuotaFija(monto, tasaPeriodica, plazo)
            };
        }
    }
}
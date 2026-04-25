using SimuladorCredito.Models;

namespace SimuladorCredito.Services
{
    public class SimuladorFacade
    {
        private readonly CreditoFactory _factory;


        public SimuladorFacade(CreditoFactory factory)
        {
            _factory = factory;
        }

        public List<Cuota> GenerarSimulacion(int opcion, decimal monto, double tasaPct, string tipo, string clase, int cap, int pagos, int plazo, bool usarExtra)
        {
            double tasaInput = tasaPct / 100;
            double ea = ConversorTasas.ConvertirAEfectivaMensual(tasaInput, tipo ?? "", clase ?? "", cap);
            double tasaPeriodica = ConversorTasas.CalcularTasaPeriodica(ea, pagos);

            Credito miCredito = _factory.CrearCredito(opcion, monto, tasaPeriodica, plazo);

            if (usarExtra)
            {

                if (!miCredito.AbonosExtraordinarios.ContainsKey(3))
                    miCredito.AbonosExtraordinarios.Add(3, 1000000);
            }

            return miCredito.GenerarTabla();
        }
    }
}
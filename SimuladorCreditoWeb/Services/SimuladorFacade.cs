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

        public List<Cuota> GenerarSimulacion(int opcion, decimal monto, double tasaPct, string tipo, string clase, int cap, int pagos, int plazo, Dictionary<int, decimal> abonos)
        {
            double tasaInput = tasaPct / 100;
            double ea = ConversorTasas.ConvertirAEfectivaMensual(tasaInput, tipo ?? "", clase ?? "", cap);
            double tasaPeriodica = ConversorTasas.CalcularTasaPeriodica(ea, pagos);
            Credito miCredito = _factory.CrearCredito(opcion, monto, tasaPeriodica, plazo);

            foreach (var kv in abonos)
                miCredito.AbonosExtraordinarios[kv.Key] = kv.Value;

            return miCredito.GenerarTabla();
        }
    }
}
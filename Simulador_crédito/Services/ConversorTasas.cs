using System;

namespace SimuladorCredito.Services
{
    public static class ConversorTasas
    {
        public static double NominalAEfectivaAnual(double tasaNominal, int capitalizaciones)
        {
            return Math.Pow(1 + tasaNominal / capitalizaciones, capitalizaciones) - 1;
        }

        public static double EfectivaAnualAPeriodo(double tasaEfectivaAnual, int pagosPorAnio)
        {
            return Math.Pow(1 + tasaEfectivaAnual, 1.0 / pagosPorAnio) - 1;
        }

        public static double AnticipadaAVencida(double tasaAnticipada)
        {
            return tasaAnticipada / (1 - tasaAnticipada);
        }

        public static double ParsearTasa(
            double tasa,
            string tipo,
            string clase,
            int capitalizaciones,
            int pagosPorAnio)
        {
            double tasaPeriodo;

            if (tipo.ToLower() == "nominal")
            {
                var efectiva = NominalAEfectivaAnual(tasa, capitalizaciones);
                tasaPeriodo = EfectivaAnualAPeriodo(efectiva, pagosPorAnio);
            }
            else
            {
                tasaPeriodo = EfectivaAnualAPeriodo(tasa, pagosPorAnio);
            }

            if (clase.ToLower() == "anticipada")
                tasaPeriodo = AnticipadaAVencida(tasaPeriodo);

            return tasaPeriodo;
        }
    }
}
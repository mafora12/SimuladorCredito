using System;

namespace SimuladorCredito.Services
{
    public static class ConversorTasas
    {
        public static double ConvertirAEfectivaMensual(double tasa, string tipo, string clase, int capitalizacionesAlAno)
        {
            double efectivaAnual;

            
            if (tipo.ToLower() == "nominal")
            {
                efectivaAnual = Math.Pow(1 + (tasa / capitalizacionesAlAno), capitalizacionesAlAno) - 1;
            }
            else 
            {
                efectivaAnual = tasa;
            }

           
            if (clase.ToLower() == "anticipada")
            {
                efectivaAnual = efectivaAnual / (1 - efectivaAnual);
            }

            return efectivaAnual;
        }

        public static double CalcularTasaPeriodica(double efectivaAnual, int pagosAlAno)
        {
            
            return Math.Pow(1 + efectivaAnual, 1.0 / pagosAlAno) - 1;
        }
    }
}
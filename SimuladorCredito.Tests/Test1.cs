using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimuladorCredito.Models;
using SimuladorCredito.Services;
using System.Collections.Generic;

namespace SimuladorCredito.Tests
{
    [TestClass]
    public class ConversorTasasTests
    {

        [TestMethod]
        public void ConvertirAEfectivaMensual_TasaEfectiva_DevuelveMismoValor()
        {
            double resultado = ConversorTasas.ConvertirAEfectivaMensual(
                tasa: 0.12, tipo: "efectiva", clase: "vencida", capitalizacionesAlAno: 12);

            Assert.AreEqual(0.12, resultado, delta: 0.0001);
        }

        [TestMethod]
        public void ConvertirAEfectivaMensual_TasaNominalMensual_DevuelveEACorrecta()
        {
            double resultado = ConversorTasas.ConvertirAEfectivaMensual(
                tasa: 0.12, tipo: "nominal", clase: "vencida", capitalizacionesAlAno: 12);

            Assert.AreEqual(0.1268, resultado, delta: 0.0001);
        }

        [TestMethod]
        public void ConvertirAEfectivaMensual_TasaNominalTrimestral_DevuelveEACorrecta()
        {
            double resultado = ConversorTasas.ConvertirAEfectivaMensual(
                tasa: 0.12, tipo: "nominal", clase: "vencida", capitalizacionesAlAno: 4);

            Assert.AreEqual(0.1255, resultado, delta: 0.0001);
        }

        [TestMethod]
        public void CalcularTasaPeriodica_PagosMensuales_DevuelveTasaCorrecta()
        {
            double resultado = ConversorTasas.CalcularTasaPeriodica(
                efectivaAnual: 0.12, pagosAlAno: 12);

            Assert.AreEqual(0.009489, resultado, delta: 0.000001);
        }


        [TestMethod]
        public void CalcularTasaPeriodica_TasaCero_DevuelveCero()
        {
            double resultado = ConversorTasas.CalcularTasaPeriodica(
                efectivaAnual: 0.0, pagosAlAno: 12);

            Assert.AreEqual(0.0, resultado, delta: 0.0001);
        }
    }


    [TestClass]
    public class CreditoCuotaFijaTests
    {
        private const decimal Monto = 10_000_000m;
        private const double Tasa = 0.01;
        private const int Plazo = 12;

        [TestMethod]
        public void GenerarTabla_PlazoCorrecto_DevuelveNumeroCuotasEsperado()
        {
            var credito = new CreditoCuotaFija(Monto, Tasa, Plazo);
            List<Cuota> tabla = credito.GenerarTabla();

            Assert.AreEqual(Plazo, tabla.Count);
        }

        [TestMethod]
        public void GenerarTabla_PrimeraCuota_NumeroCuotaEsUno()
        {
            var credito = new CreditoCuotaFija(Monto, Tasa, Plazo);

            Assert.AreEqual(1, credito.GenerarTabla()[0].Numero);
        }

        [TestMethod]
        public void GenerarTabla_UltimaCuota_SaldoRestanteEsCero()
        {
            var credito = new CreditoCuotaFija(Monto, Tasa, Plazo);
            List<Cuota> tabla = credito.GenerarTabla();

            Assert.AreEqual(0m, tabla[^1].SaldoRestante);
        }

        [TestMethod]
        public void GenerarTabla_CadaFila_CuotaIgualInteresmasCapital()
        {
            var credito = new CreditoCuotaFija(Monto, Tasa, Plazo);

            foreach (Cuota c in credito.GenerarTabla())
            {
                decimal suma = c.Interes + c.AbonoCapital;
                Assert.AreEqual((double)c.ValorCuota, (double)suma, delta: 0.02,
                    $"Período {c.Numero}: Cuota ({c.ValorCuota}) ≠ Interés + Capital ({suma})");
            }
        }

        [TestMethod]
        public void GenerarTabla_Intereses_DisminuyenCadaPeriodo()
        {
            var credito = new CreditoCuotaFija(Monto, Tasa, Plazo);
            List<Cuota> tabla = credito.GenerarTabla();

            for (int i = 1; i < tabla.Count; i++)
                Assert.IsTrue(tabla[i].Interes < tabla[i - 1].Interes,
                    $"Período {tabla[i].Numero}: el interés no disminuyó.");
        }

        [TestMethod]
        public void GenerarTabla_AbonoCapital_AumentaCadaPeriodo()
        {
            var credito = new CreditoCuotaFija(Monto, Tasa, Plazo);
            List<Cuota> tabla = credito.GenerarTabla();

            for (int i = 1; i < tabla.Count; i++)
                Assert.IsTrue(tabla[i].AbonoCapital > tabla[i - 1].AbonoCapital,
                    $"Período {tabla[i].Numero}: el abono capital no aumentó.");
        }

        [TestMethod]
        public void GenerarTabla_ConAbonoExtraordinario_SaldoDisminuyeMasRapido()
        {
            var creditoNormal = new CreditoCuotaFija(Monto, Tasa, Plazo);
            List<Cuota> tablaBase = creditoNormal.GenerarTabla();

            var creditoExtra = new CreditoCuotaFija(Monto, Tasa, Plazo);
            creditoExtra.AbonosExtraordinarios.Add(3, 1_000_000m);
            List<Cuota> tablaExtra = creditoExtra.GenerarTabla();

            Assert.IsTrue(tablaExtra[2].SaldoRestante < tablaBase[2].SaldoRestante,
                "Tras el período 3, el saldo con abono extra debe ser menor.");
        }

        [TestMethod]
        public void GenerarTabla_AbonoExtraordinarioMayorAlSaldo_SaldoNuncaEsNegativo()
        {
            var credito = new CreditoCuotaFija(Monto, Tasa, Plazo);
            credito.AbonosExtraordinarios.Add(Plazo, 50_000_000m);

            foreach (Cuota c in credito.GenerarTabla())
                Assert.IsTrue(c.SaldoRestante >= 0m,
                    $"Período {c.Numero}: saldo negativo ({c.SaldoRestante}).");
        }

        [TestMethod]
        public void GenerarTabla_TasaCero_CuotaIgualMontoEntrePlazo()
        {
            var credito = new CreditoCuotaFija(Monto, tasa: 0.0, Plazo);
            decimal esperado = Monto / Plazo;

            foreach (Cuota c in credito.GenerarTabla())
                Assert.AreEqual((double)esperado, (double)c.ValorCuota, delta: 0.01,
                    $"Con tasa 0 la cuota debería ser {esperado}.");
        }

        [TestMethod]
        public void GenerarTabla_PlazoUno_UnicaCuotaSaldaElCredito()
        {
            var credito = new CreditoCuotaFija(Monto, Tasa, plazo: 1);
            List<Cuota> tabla = credito.GenerarTabla();

            Assert.AreEqual(1, tabla.Count);
            Assert.AreEqual(0m, tabla[0].SaldoRestante);
        }
    }


    [TestClass]
    public class CreditoAbonoConstanteTests
    {
        private const decimal Monto = 10_000_000m;
        private const double Tasa = 0.01;
        private const int Plazo = 12;

        [TestMethod]
        public void GenerarTabla_PlazoCorrecto_DevuelveNumeroCuotasEsperado()
        {
            var credito = new CreditoAbonoConstante(Monto, Tasa, Plazo);

            Assert.AreEqual(Plazo, credito.GenerarTabla().Count);
        }

        [TestMethod]
        public void GenerarTabla_AbonoCapital_EsConstanteEnTodosLosPeriodos()
        {
            var credito = new CreditoAbonoConstante(Monto, Tasa, Plazo);
            decimal abonoEsperado = Monto / Plazo;

            foreach (Cuota c in credito.GenerarTabla())
                Assert.AreEqual((double)abonoEsperado, (double)c.AbonoCapital, delta: 0.01,
                    $"Período {c.Numero}: abono capital ({c.AbonoCapital}) ≠ esperado ({abonoEsperado}).");
        }


        [TestMethod]
        public void GenerarTabla_ValorCuota_DisminuyeCadaPeriodo()
        {
            var credito = new CreditoAbonoConstante(Monto, Tasa, Plazo);
            List<Cuota> tabla = credito.GenerarTabla();

            for (int i = 1; i < tabla.Count; i++)
                Assert.IsTrue(tabla[i].ValorCuota < tabla[i - 1].ValorCuota,
                    $"Período {tabla[i].Numero}: la cuota no disminuyó.");
        }

        [TestMethod]
        public void GenerarTabla_UltimaCuota_SaldoRestanteEsCero()
        {
            var credito = new CreditoAbonoConstante(Monto, Tasa, Plazo);
            List<Cuota> tabla = credito.GenerarTabla();

            Assert.AreEqual(0m, tabla[^1].SaldoRestante, 0.01m);
        }

        [TestMethod]
        public void GenerarTabla_TasaCero_InteresEsCeroEnTodasLasCuotas()
        {
            var credito = new CreditoAbonoConstante(Monto, tasa: 0.0, Plazo);

            foreach (Cuota c in credito.GenerarTabla())
            {
                Assert.AreEqual(0m, c.Interes, 0.001m,
                    $"Período {c.Numero}: con tasa 0 el interés debe ser 0.");
                Assert.AreEqual((double)c.AbonoCapital, (double)c.ValorCuota, delta: 0.001,
                    $"Período {c.Numero}: sin interés la cuota debe igualar al abono capital.");
            }
        }
    }


    [TestClass]
    public class CreditoTasaVariableCuotaFijaTests
    {
        private const decimal Monto = 10_000_000m;
        private const double Tasa = 0.01;
        private const int Plazo = 12;

        [TestMethod]
        public void GenerarTabla_PlazoCorrecto_DevuelveNumeroCuotasEsperado()
        {
            var credito = new CreditoTasaVariableCuotaFija(Monto, Tasa, Plazo);

            Assert.AreEqual(Plazo, credito.GenerarTabla().Count);
        }

        [TestMethod]
        public void GenerarTabla_ValorCuota_EsConstanteEnTodosLosPeriodos()
        {
            var credito = new CreditoTasaVariableCuotaFija(Monto, Tasa, Plazo);
            List<Cuota> tabla = credito.GenerarTabla();
            decimal primera = tabla[0].ValorCuota;

            foreach (Cuota c in tabla)
                Assert.AreEqual((double)primera, (double)c.ValorCuota, delta: 0.01,
                    $"Período {c.Numero}: cuota ({c.ValorCuota}) difiere de la inicial ({primera}).");
        }

        [TestMethod]
        public void GenerarTabla_SaldoRestante_NuncaEsNegativo()
        {
            var credito = new CreditoTasaVariableCuotaFija(Monto, Tasa, Plazo);

            foreach (Cuota c in credito.GenerarTabla())
                Assert.IsTrue(c.SaldoRestante >= 0m,
                    $"Período {c.Numero}: saldo negativo ({c.SaldoRestante}).");
        }

        [TestMethod]
        public void GenerarTabla_TasaCero_CuotaIgualMontoEntrePlazo()
        {
            var credito = new CreditoTasaVariableCuotaFija(Monto, tasa: 0.0, Plazo);
            decimal esperado = Monto / Plazo;

            foreach (Cuota c in credito.GenerarTabla())
                Assert.AreEqual((double)esperado, (double)c.ValorCuota, delta: 0.01);
        }
    }


    [TestClass]
    public class CuotaTests
    {
        [TestMethod]
        public void Cuota_AsignarPropiedades_SeLeenCorrectamente()
        {
            var cuota = new Cuota
            {
                Numero = 1,
                ValorCuota = 500_000m,
                Interes = 100_000m,
                AbonoCapital = 400_000m,
                AbonoExtraordinario = 50_000m,
                SaldoRestante = 9_600_000m
            };

            Assert.AreEqual(1, cuota.Numero);
            Assert.AreEqual(500_000m, cuota.ValorCuota);
            Assert.AreEqual(100_000m, cuota.Interes);
            Assert.AreEqual(400_000m, cuota.AbonoCapital);
            Assert.AreEqual(50_000m, cuota.AbonoExtraordinario);
            Assert.AreEqual(9_600_000m, cuota.SaldoRestante);
        }
    }


    [TestClass]
    public class ComparativaModalidadesTests
    {
        private const decimal Monto = 10_000_000m;
        private const double Tasa = 0.01;
        private const int Plazo = 12;


        [TestMethod]
        public void TotalIntereses_CuotaFija_MayorOIgualQueAbonoConstante()
        {
            decimal interesesFija = 0m;
            decimal interesesConstante = 0m;

            foreach (Cuota c in new CreditoCuotaFija(Monto, Tasa, Plazo).GenerarTabla())
                interesesFija += c.Interes;

            foreach (Cuota c in new CreditoAbonoConstante(Monto, Tasa, Plazo).GenerarTabla())
                interesesConstante += c.Interes;

            Assert.IsTrue(interesesFija >= interesesConstante,
                $"Cuota Fija ({interesesFija:C}) debe pagar más o igual intereses " +
                $"que Abono Constante ({interesesConstante:C}).");
        }


        [TestMethod]
        public void TodasLasModalidades_SaldanCompletamenteElCredito()
        {
            var modalidades = new List<Credito>
            {
                new CreditoCuotaFija(Monto, Tasa, Plazo),
                new CreditoAbonoConstante(Monto, Tasa, Plazo),
                new CreditoTasaVariableCuotaFija(Monto, Tasa, Plazo)
            };

            foreach (Credito credito in modalidades)
            {
                List<Cuota> tabla = credito.GenerarTabla();
                Assert.AreEqual(0m, tabla[^1].SaldoRestante, 0.01m,
                    $"{credito.GetType().Name}: saldo final ({tabla[^1].SaldoRestante}) ≠ 0.");
            }
        }


        [TestMethod]
        public void TodasLasModalidades_ValoresDeCuota_SonPositivos()
        {
            var modalidades = new List<Credito>
            {
                new CreditoCuotaFija(Monto, Tasa, Plazo),
                new CreditoAbonoConstante(Monto, Tasa, Plazo),
                new CreditoTasaVariableCuotaFija(Monto, Tasa, Plazo)
            };

            foreach (Credito credito in modalidades)
                foreach (Cuota c in credito.GenerarTabla())
                    Assert.IsTrue(c.ValorCuota > 0m,
                        $"{credito.GetType().Name} – Período {c.Numero}: cuota no positiva ({c.ValorCuota}).");
        }
    }
}
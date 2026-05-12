// ================================================================
//  PRUEBAS UNITARIAS – SIMULADOR DE CRÉDITO WEB
//  Proyecto principal : SimuladorCreditoWeb
//  Framework          : MSTest
// ================================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimuladorCredito.Models;
using SimuladorCredito.Services;
using System.Collections.Generic;

namespace SimuladorCreditoWeb.Tests
{
    // ────────────────────────────────────────────────────────────
    //  1. CONVERSOR DE TASAS
    // ────────────────────────────────────────────────────────────
    [TestClass]
    public class ConversorTasasTests
    {
        /// <summary>
        /// Tasa efectiva del 12 % anual debe devolverse sin cambios.
        /// </summary>
        [TestMethod]
        public void ConvertirAEfectivaMensual_TasaEfectiva_DevuelveMismoValor()
        {
            double resultado = ConversorTasas.ConvertirAEfectivaMensual(
                tasa: 0.12, tipo: "efectiva", clase: "vencida", capitalizacionesAlAno: 12);

            Assert.AreEqual(0.12, resultado, delta: 0.0001);
        }

        /// <summary>
        /// Tasa nominal 12 % capitalizable mensualmente → EA ≈ 12.68 %.
        /// </summary>
        [TestMethod]
        public void ConvertirAEfectivaMensual_TasaNominalMensual_DevuelveEACorrecta()
        {
            double resultado = ConversorTasas.ConvertirAEfectivaMensual(
                tasa: 0.12, tipo: "nominal", clase: "vencida", capitalizacionesAlAno: 12);

            Assert.AreEqual(0.1268, resultado, delta: 0.0001);
        }

        /// <summary>
        /// Tasa nominal 12 % capitalizable trimestralmente → EA ≈ 12.55 %.
        /// </summary>
        [TestMethod]
        public void ConvertirAEfectivaMensual_TasaNominalTrimestral_DevuelveEACorrecta()
        {
            double resultado = ConversorTasas.ConvertirAEfectivaMensual(
                tasa: 0.12, tipo: "nominal", clase: "vencida", capitalizacionesAlAno: 4);

            Assert.AreEqual(0.1255, resultado, delta: 0.0001);
        }

        /// <summary>
        /// El conversor debe ser insensible a mayúsculas en "tipo".
        /// </summary>
        [TestMethod]
        public void ConvertirAEfectivaMensual_TipoEnMayusculas_FuncionaIgual()
        {
            double minuscula = ConversorTasas.ConvertirAEfectivaMensual(0.12, "nominal", "vencida", 12);
            double mayuscula = ConversorTasas.ConvertirAEfectivaMensual(0.12, "NOMINAL", "VENCIDA", 12);

            Assert.AreEqual(minuscula, mayuscula, delta: 0.0001,
                "El tipo en mayúsculas debe producir el mismo resultado.");
        }

        /// <summary>
        /// Tasa anticipada debe ajustarse correctamente.
        /// EA anticipada = EA / (1 - EA) = 0.12 / 0.88 ≈ 0.13636
        /// </summary>
        [TestMethod]
        public void ConvertirAEfectivaMensual_ClaseAnticipada_AjustaCorrectamente()
        {
            double resultado = ConversorTasas.ConvertirAEfectivaMensual(
                tasa: 0.12, tipo: "efectiva", clase: "anticipada", capitalizacionesAlAno: 12);

            Assert.AreEqual(0.13636, resultado, delta: 0.0001);
        }

        /// <summary>
        /// EA = 12 % con pagos mensuales → tasa mensual ≈ 0.9489 %.
        /// </summary>
        [TestMethod]
        public void CalcularTasaPeriodica_PagosMensuales_DevuelveTasaCorrecta()
        {
            double resultado = ConversorTasas.CalcularTasaPeriodica(
                efectivaAnual: 0.12, pagosAlAno: 12);

            Assert.AreEqual(0.009489, resultado, delta: 0.000001);
        }

        /// <summary>
        /// EA = 0 % → tasa periódica = 0.
        /// </summary>
        [TestMethod]
        public void CalcularTasaPeriodica_TasaCero_DevuelveCero()
        {
            double resultado = ConversorTasas.CalcularTasaPeriodica(
                efectivaAnual: 0.0, pagosAlAno: 12);

            Assert.AreEqual(0.0, resultado, delta: 0.0001);
        }
    }


    // ────────────────────────────────────────────────────────────
    //  2. CREDITO FACTORY
    // ────────────────────────────────────────────────────────────
    [TestClass]
    public class CreditoFactoryTests
    {
        private readonly CreditoFactory _factory = new CreditoFactory();
        private const decimal Monto = 10_000_000m;
        private const double Tasa = 0.01;
        private const int Plazo = 12;

        /// <summary>
        /// Opción 1 debe crear CreditoCuotaFija.
        /// </summary>
        [TestMethod]
        public void CrearCredito_Opcion1_DevuelveCreditoCuotaFija()
        {
            Credito credito = _factory.CrearCredito(1, Monto, Tasa, Plazo);

            Assert.IsInstanceOfType(credito, typeof(CreditoCuotaFija));
        }

        /// <summary>
        /// Opción 2 debe crear CreditoAbonoConstante.
        /// </summary>
        [TestMethod]
        public void CrearCredito_Opcion2_DevuelveCreditoAbonoConstante()
        {
            Credito credito = _factory.CrearCredito(2, Monto, Tasa, Plazo);

            Assert.IsInstanceOfType(credito, typeof(CreditoAbonoConstante));
        }

        /// <summary>
        /// Opción 3 debe crear CreditoTasaVariableCuotaFija.
        /// </summary>
        [TestMethod]
        public void CrearCredito_Opcion3_DevuelveCreditoTasaVariableCuotaFija()
        {
            Credito credito = _factory.CrearCredito(3, Monto, Tasa, Plazo);

            Assert.IsInstanceOfType(credito, typeof(CreditoTasaVariableCuotaFija));
        }

        /// <summary>
        /// Opción desconocida debe usar CreditoCuotaFija por defecto (case _).
        /// </summary>
        [TestMethod]
        public void CrearCredito_OpcionDesconocida_DevuelveCuotaFijaPorDefecto()
        {
            Credito credito = _factory.CrearCredito(99, Monto, Tasa, Plazo);

            Assert.IsInstanceOfType(credito, typeof(CreditoCuotaFija));
        }

        /// <summary>
        /// Los parámetros deben asignarse correctamente al crédito creado.
        /// </summary>
        [TestMethod]
        public void CrearCredito_ParametrosCorrectos_SeAsignanBien()
        {
            Credito credito = _factory.CrearCredito(1, Monto, Tasa, Plazo);

            Assert.AreEqual(Monto, credito.Monto);
            Assert.AreEqual(Tasa, credito.TasaPeriodica, delta: 0.0001);
            Assert.AreEqual(Plazo, credito.Plazo);
        }
    }


    // ────────────────────────────────────────────────────────────
    //  3. SIMULADOR FACADE
    //  NOTA: La firma actual es:
    //  GenerarSimulacion(opcion, monto, tasaPct, tipo, clase,
    //                    cap, pagos, plazo, Dictionary<int,decimal> abonos)
    // ────────────────────────────────────────────────────────────
    [TestClass]
    public class SimuladorFacadeTests
    {
        private SimuladorFacade _facade = null!;

        // Diccionario vacío reutilizable para simulaciones sin abonos extra
        private static readonly Dictionary<int, decimal> SinAbonos = new();

        [TestInitialize]
        public void Setup()
        {
            _facade = new SimuladorFacade(new CreditoFactory());
        }

        /// <summary>
        /// La fachada debe devolver una tabla no vacía con parámetros válidos.
        /// </summary>
        [TestMethod]
        public void GenerarSimulacion_ParametrosValidos_DevuelveTablaConCuotas()
        {
            List<Cuota> tabla = _facade.GenerarSimulacion(
                opcion: 1, monto: 10_000_000m, tasaPct: 12,
                tipo: "nominal", clase: "vencida",
                cap: 12, pagos: 12, plazo: 12,
                abonos: SinAbonos);

            Assert.IsTrue(tabla.Count > 0, "La tabla no debe estar vacía.");
        }

        /// <summary>
        /// Con un abono extraordinario en período 3, ese período debe reflejarlo.
        /// </summary>
        [TestMethod]
        public void GenerarSimulacion_ConAbonoExtraordinario_Periodo3TieneExtra()
        {
            var abonos = new Dictionary<int, decimal> { { 3, 1_000_000m } };

            List<Cuota> tabla = _facade.GenerarSimulacion(
                opcion: 1, monto: 10_000_000m, tasaPct: 12,
                tipo: "nominal", clase: "vencida",
                cap: 12, pagos: 12, plazo: 12,
                abonos: abonos);

            Assert.IsTrue(tabla[2].AbonoExtraordinario > 0,
                "El período 3 debe tener un abono extraordinario mayor a 0.");
        }

        /// <summary>
        /// Con múltiples abonos extraordinarios, cada período debe reflejar su monto.
        /// </summary>
        [TestMethod]
        public void GenerarSimulacion_MultiplesAbonosExtraordinarios_SeReflejanCorrectamente()
        {
            var abonos = new Dictionary<int, decimal>
            {
                { 2, 500_000m },
                { 5, 1_000_000m },
                { 8, 750_000m }
            };

            List<Cuota> tabla = _facade.GenerarSimulacion(
                opcion: 1, monto: 10_000_000m, tasaPct: 12,
                tipo: "nominal", clase: "vencida",
                cap: 12, pagos: 12, plazo: 12,
                abonos: abonos);

            Assert.AreEqual(500_000m, tabla[1].AbonoExtraordinario, "Período 2 incorrecto.");
            Assert.AreEqual(1_000_000m, tabla[4].AbonoExtraordinario, "Período 5 incorrecto.");
            Assert.AreEqual(750_000m, tabla[7].AbonoExtraordinario, "Período 8 incorrecto.");
        }

        /// <summary>
        /// Sin abonos extraordinarios, ningún período debe tener extra > 0.
        /// </summary>
        [TestMethod]
        public void GenerarSimulacion_SinAbonosExtraordinarios_NingunPeriodoTieneExtra()
        {
            List<Cuota> tabla = _facade.GenerarSimulacion(
                opcion: 1, monto: 10_000_000m, tasaPct: 12,
                tipo: "nominal", clase: "vencida",
                cap: 12, pagos: 12, plazo: 12,
                abonos: SinAbonos);

            foreach (Cuota c in tabla)
                Assert.AreEqual(0m, c.AbonoExtraordinario,
                    $"Período {c.Numero}: no debería tener abono extraordinario.");
        }

        /// <summary>
        /// El saldo final debe ser 0 para la opción 1 (Cuota Fija).
        /// </summary>
        [TestMethod]
        public void GenerarSimulacion_Opcion1_SaldoFinalEsCero()
        {
            List<Cuota> tabla = _facade.GenerarSimulacion(
                opcion: 1, monto: 10_000_000m, tasaPct: 12,
                tipo: "nominal", clase: "vencida",
                cap: 12, pagos: 12, plazo: 12,
                abonos: SinAbonos);

            Assert.AreEqual(0m, tabla[^1].SaldoRestante, 0.01m);
        }

        /// <summary>
        /// El saldo final debe ser 0 para la opción 2 (Abono Constante).
        /// </summary>
        [TestMethod]
        public void GenerarSimulacion_Opcion2_SaldoFinalEsCero()
        {
            List<Cuota> tabla = _facade.GenerarSimulacion(
                opcion: 2, monto: 10_000_000m, tasaPct: 12,
                tipo: "nominal", clase: "vencida",
                cap: 12, pagos: 12, plazo: 12,
                abonos: SinAbonos);

            Assert.AreEqual(0m, tabla[^1].SaldoRestante, 0.01m);
        }

        /// <summary>
        /// El saldo final debe ser 0 para la opción 3 (Tasa Variable Cuota Fija).
        /// </summary>
        [TestMethod]
        public void GenerarSimulacion_Opcion3_SaldoFinalEsCero()
        {
            List<Cuota> tabla = _facade.GenerarSimulacion(
                opcion: 3, monto: 10_000_000m, tasaPct: 12,
                tipo: "nominal", clase: "vencida",
                cap: 12, pagos: 12, plazo: 12,
                abonos: SinAbonos);

            Assert.AreEqual(0m, tabla[^1].SaldoRestante, 0.01m);
        }

        /// <summary>
        /// Ninguna cuota debe tener valor negativo en ninguna modalidad.
        /// </summary>
        [TestMethod]
        public void GenerarSimulacion_TodasLasOpciones_ValoresSonPositivos()
        {
            for (int opcion = 1; opcion <= 3; opcion++)
            {
                List<Cuota> tabla = _facade.GenerarSimulacion(
                    opcion: opcion, monto: 10_000_000m, tasaPct: 12,
                    tipo: "nominal", clase: "vencida",
                    cap: 12, pagos: 12, plazo: 12,
                    abonos: SinAbonos);

                foreach (Cuota c in tabla)
                    Assert.IsTrue(c.ValorCuota > 0m,
                        $"Opción {opcion} – Período {c.Numero}: cuota no positiva.");
            }
        }

        /// <summary>
        /// Un abono extraordinario mayor al saldo no debe producir saldo negativo.
        /// </summary>
        [TestMethod]
        public void GenerarSimulacion_AbonoMayorAlSaldo_SaldoNuncaEsNegativo()
        {
            var abonos = new Dictionary<int, decimal> { { 3, 50_000_000m } };

            List<Cuota> tabla = _facade.GenerarSimulacion(
                opcion: 1, monto: 10_000_000m, tasaPct: 12,
                tipo: "nominal", clase: "vencida",
                cap: 12, pagos: 12, plazo: 12,
                abonos: abonos);

            foreach (Cuota c in tabla)
                Assert.IsTrue(c.SaldoRestante >= 0m,
                    $"Período {c.Numero}: saldo negativo ({c.SaldoRestante}).");
        }
    }


    // ────────────────────────────────────────────────────────────
    //  4. CRÉDITO CUOTA FIJA (Sistema Francés)
    // ────────────────────────────────────────────────────────────
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

            Assert.AreEqual(Plazo, credito.GenerarTabla().Count);
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

            Assert.AreEqual(0m, credito.GenerarTabla()[^1].SaldoRestante);
        }

        /// <summary>
        /// En cada fila: ValorCuota ≈ Interés + AbonoCapital.
        /// </summary>
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

        /// <summary>
        /// Los intereses deben disminuir período a período.
        /// </summary>
        [TestMethod]
        public void GenerarTabla_Intereses_DisminuyenCadaPeriodo()
        {
            var credito = new CreditoCuotaFija(Monto, Tasa, Plazo);
            List<Cuota> tabla = credito.GenerarTabla();

            for (int i = 1; i < tabla.Count; i++)
                Assert.IsTrue(tabla[i].Interes < tabla[i - 1].Interes,
                    $"Período {tabla[i].Numero}: el interés no disminuyó.");
        }

        /// <summary>
        /// El abono al capital debe aumentar período a período.
        /// </summary>
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
                Assert.AreEqual((double)esperado, (double)c.ValorCuota, delta: 0.01);
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


    // ────────────────────────────────────────────────────────────
    //  5. CRÉDITO ABONO CONSTANTE (Sistema Alemán)
    // ────────────────────────────────────────────────────────────
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

        /// <summary>
        /// El abono al capital debe ser el mismo en todos los períodos.
        /// </summary>
        [TestMethod]
        public void GenerarTabla_AbonoCapital_EsConstanteEnTodosLosPeriodos()
        {
            var credito = new CreditoAbonoConstante(Monto, Tasa, Plazo);
            decimal abonoEsperado = Monto / Plazo;

            foreach (Cuota c in credito.GenerarTabla())
                Assert.AreEqual((double)abonoEsperado, (double)c.AbonoCapital, delta: 0.01,
                    $"Período {c.Numero}: abono capital ({c.AbonoCapital}) ≠ esperado ({abonoEsperado}).");
        }

        /// <summary>
        /// La cuota total debe disminuir cada período.
        /// </summary>
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

            Assert.AreEqual(0m, credito.GenerarTabla()[^1].SaldoRestante, 0.01m);
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


    // ────────────────────────────────────────────────────────────
    //  6. CRÉDITO TASA VARIABLE – CUOTA FIJA
    // ────────────────────────────────────────────────────────────
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

        /// <summary>
        /// La cuota establecida al inicio debe ser igual en todos los períodos.
        /// </summary>
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


    // ────────────────────────────────────────────────────────────
    //  7. MODELO CUOTA – PROPIEDADES
    // ────────────────────────────────────────────────────────────
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


    // ────────────────────────────────────────────────────────────
    //  8. COMPARATIVA ENTRE LAS TRES MODALIDADES
    // ────────────────────────────────────────────────────────────
    [TestClass]
    public class ComparativaModalidadesTests
    {
        private const decimal Monto = 10_000_000m;
        private const double Tasa = 0.01;
        private const int Plazo = 12;

        /// <summary>
        /// El total de intereses en Cuota Fija debe ser mayor o igual
        /// que en Abono Constante para los mismos parámetros.
        /// </summary>
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

        /// <summary>
        /// Las tres modalidades deben saldar completamente el crédito al final.
        /// </summary>
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

        /// <summary>
        /// Ninguna modalidad debe producir una cuota con valor negativo.
        /// </summary>
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


    // ────────────────────────────────────────────────────────────
    //  9. VALORES NEGATIVOS E INVÁLIDOS
    //  Documentan el comportamiento actual del sistema ante
    //  entradas incorrectas que un usuario web podría enviar.
    //  Si en el futuro se agregan validaciones, estas pruebas
    //  deben actualizarse para verificar que se lanza excepción
    //  o se devuelve tabla vacía de forma controlada.
    // ────────────────────────────────────────────────────────────
    [TestClass]
    public class ValoresInvalidosTests
    {
        private static readonly Dictionary<int, decimal> SinAbonos = new();
        private SimuladorFacade _facade = null!;

        [TestInitialize]
        public void Setup()
        {
            _facade = new SimuladorFacade(new CreditoFactory());
        }

        // ── Monto negativo ───────────────────────────────────────

        /// <summary>
        /// Monto negativo en CuotaFija: el saldo nunca debe ser positivo
        /// (el sistema no explota pero genera valores sin sentido financiero).
        /// </summary>
        [TestMethod]
        public void CreditoCuotaFija_MontoNegativo_SaldoSiempreNegativoOMenorACero()
        {
            var credito = new CreditoCuotaFija(-10_000_000m, 0.01, 12);
            List<Cuota> tabla = credito.GenerarTabla();

            // Con monto negativo la tabla puede estar vacía o tener valores negativos.
            // Lo que NO debe pasar es que genere cuotas positivas como si fuera válido.
            foreach (Cuota c in tabla)
                Assert.IsTrue(c.ValorCuota <= 0m,
                    $"Período {c.Numero}: monto negativo generó cuota positiva ({c.ValorCuota}).");
        }

        [TestMethod]
        public void CreditoAbonoConstante_MontoNegativo_SaldoSiempreNegativoOMenorACero()
        {
            var credito = new CreditoAbonoConstante(-10_000_000m, 0.01, 12);
            List<Cuota> tabla = credito.GenerarTabla();

            foreach (Cuota c in tabla)
                Assert.IsTrue(c.ValorCuota <= 0m,
                    $"Período {c.Numero}: monto negativo generó cuota positiva ({c.ValorCuota}).");
        }

        [TestMethod]
        public void CreditoTasaVariable_MontoNegativo_SaldoSiempreNegativoOMenorACero()
        {
            var credito = new CreditoTasaVariableCuotaFija(-10_000_000m, 0.01, 12);
            List<Cuota> tabla = credito.GenerarTabla();

            foreach (Cuota c in tabla)
                Assert.IsTrue(c.ValorCuota <= 0m,
                    $"Período {c.Numero}: monto negativo generó cuota positiva ({c.ValorCuota}).");
        }

        // ── Tasa negativa ────────────────────────────────────────

        /// <summary>
        /// Tasa negativa no debe generar cuotas mayores al monto original.
        /// </summary>
        [TestMethod]
        public void CreditoCuotaFija_TasaNegativa_NoGeneraCuotasMayoresAlMonto()
        {
            const decimal monto = 10_000_000m;
            var credito = new CreditoCuotaFija(monto, tasa: -0.05, 12);
            List<Cuota> tabla = credito.GenerarTabla();

            foreach (Cuota c in tabla)
                Assert.IsTrue(c.ValorCuota <= monto,
                    $"Período {c.Numero}: cuota ({c.ValorCuota}) supera el monto original.");
        }

        [TestMethod]
        public void CreditoAbonoConstante_TasaNegativa_NoGeneraCuotasMayoresAlMonto()
        {
            const decimal monto = 10_000_000m;
            var credito = new CreditoAbonoConstante(monto, tasa: -0.05, 12);
            List<Cuota> tabla = credito.GenerarTabla();

            foreach (Cuota c in tabla)
                Assert.IsTrue(c.ValorCuota <= monto,
                    $"Período {c.Numero}: cuota ({c.ValorCuota}) supera el monto original.");
        }

        // ── Plazo cero o negativo ────────────────────────────────

        /// <summary>
        /// Plazo 0 no debe generar ninguna cuota (bucle no itera).
        /// </summary>
        [TestMethod]
        public void CreditoCuotaFija_PlazoCero_DevuelveTablaVacia()
        {
            var credito = new CreditoCuotaFija(10_000_000m, 0.01, plazo: 0);
            List<Cuota> tabla = credito.GenerarTabla();

            Assert.AreEqual(0, tabla.Count,
                "Con plazo 0 no se deben generar cuotas.");
        }

        /// <summary>
        /// Plazo 0 en AbonoConstante lanza DivideByZeroException porque
        /// el código calcula Monto/Plazo antes del bucle sin validar.
        /// Documenta el bug actual — debería validarse la entrada.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void CreditoAbonoConstante_PlazoCero_LanzaDivideByZero()
        {
            var credito = new CreditoAbonoConstante(10_000_000m, 0.01, plazo: 0);
            credito.GenerarTabla();
        }

        /// <summary>
        /// Plazo 0 en TasaVariableCuotaFija lanza OverflowException porque
        /// Math.Pow genera valores que no caben en decimal.
        /// Documenta el bug actual — debería validarse la entrada.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void CreditoTasaVariable_PlazoCero_LanzaOverflowException()
        {
            var credito = new CreditoTasaVariableCuotaFija(10_000_000m, 0.01, plazo: 0);
            credito.GenerarTabla();
        }

        // ── Facade con valores inválidos ─────────────────────────

        /// <summary>
        /// Monto 0 en la fachada no debe generar cuotas con valor positivo.
        /// </summary>
        [TestMethod]
        public void Facade_MontoCero_TodasLasCuotasSonCero()
        {
            List<Cuota> tabla = _facade.GenerarSimulacion(
                opcion: 1, monto: 0m, tasaPct: 12,
                tipo: "nominal", clase: "vencida",
                cap: 12, pagos: 12, plazo: 12,
                abonos: SinAbonos);

            foreach (Cuota c in tabla)
                Assert.AreEqual(0m, c.ValorCuota, 0.01m,
                    $"Período {c.Numero}: con monto 0 la cuota debería ser 0.");
        }

        /// <summary>
        /// Tasa 0% en la fachada debe generar cuotas iguales a monto/plazo.
        /// </summary>
        [TestMethod]
        public void Facade_TasaCero_CuotaIgualMontoEntrePlazo()
        {
            const decimal monto = 10_000_000m;
            const int plazo = 12;

            List<Cuota> tabla = _facade.GenerarSimulacion(
                opcion: 1, monto: monto, tasaPct: 0,
                tipo: "efectiva", clase: "vencida",
                cap: 12, pagos: 12, plazo: plazo,
                abonos: SinAbonos);

            decimal esperado = monto / plazo;
            foreach (Cuota c in tabla)
                Assert.AreEqual((double)esperado, (double)c.ValorCuota, delta: 0.01,
                    $"Período {c.Numero}: con tasa 0 la cuota debería ser {esperado}.");
        }
    }
}
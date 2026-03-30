#  Simulador de Crédito Integral

Este proyecto consiste en el desarrollo de un **simulador financiero integral**, diseñado para modelar con precisión el comportamiento matemático de préstamos bajo diversas modalidades y condiciones. El sistema permite calcular tablas de amortización completas y comparar alternativas de financiación, replicando la lógica de los simuladores bancarios reales.

---

##  Integrantes
* **Miguel Santana Saldarriaga**
* **Juan Jose Hernandez Tobón**
* **Mariana Flórez Ramírez**

---

## Descripción del Proyecto
El simulador procesa variables financieras complejas para ofrecer una visión detallada del costo del dinero en el tiempo. Permite al usuario analizar el impacto de diferentes tasas y tipos de cuotas antes de tomar una decisión financiera.

### 1. Modalidades de Simulación
El sistema soporta las tres estructuras principales de amortización:
* **Tasa fija – Cuota fija:** (Sistema Francés) La cuota se mantiene constante durante todo el plazo.
* **Tasa variable – Cuota fija:** El valor de la cuota es constante, pero la distribución entre capital e interés varía según la tasa.
* **Tasa variable – Cuota variable:** (Abono constante a capital) La cuota disminuye periódicamente a medida que bajan los intereses.

### 2.  Parámetros de Entrada
El usuario puede personalizar la simulación mediante:
* **Monto del crédito:** Valor total solicitado.
* **Tasa de interés:** Soporte para tasas **Nominales** y **Efectivas**.
* **Periodicidad:** Mensual, trimestral, semestral o anual.
* **Tipo de liquidación:** Anticipada o Vencida.
* **Abonos extraordinarios:** Capacidad de programar pagos adicionales que afecten el saldo pendiente.

### 3. Procesamiento Matemático
El motor financiero del simulador implementa:
* Cálculo de cuotas mediante fórmulas de **Anualidades**.
* Ajuste dinámico de saldos y recálculos ante **pagos extra**.
* Conversión automática entre tasas nominales y efectivas.
* Desglose período a período de: Intereses, Abono a capital y Saldo pendiente.

---

## Características del Sistema

### Generación del Plan de Pagos
* **Tabla de Amortización:** Detalle numerado de cuotas, intereses, capital amortizado y saldo restante.
* **Visualización:** Formato organizado para lectura rápida en consola o interfaz local.
* **Gráficas (En desarrollo):** Representación visual de la disminución del saldo y evolución de intereses.

### Exportación de Resultados
* Generación de archivos **CSV**.
* Resumen descargable en formato **.xlsx** (Excel) para análisis externo.

---

## Herramientas y Tecnologías
* **Lenguaje:** C#
* **Entorno de Desarrollo:** Visual Studio / VS Code.
* **Control de Versiones:** GitHub.
* **Arquitectura:** Programación Orientada a Objetos (POO) con Polimorfismo.

---

## Alcance y Delimitaciones
* **Incluye:** Diseño de interfaz funcional, lógica matemática local y comparación de modalidades.
* **No incluye:** Conexión a bases de datos externas, gestión de perfiles de usuario o integración con APIs bancarias reales.

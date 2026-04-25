#  Simulador de Crédito Integral

Este proyecto consiste en el desarrollo de un **simulador financiero integral**, diseñado para modelar con precisión el comportamiento matemático de préstamos bajo diversas modalidades y condiciones. El sistema permite calcular tablas de amortización completas y comparar alternativas de financiación, replicando la lógica de los simuladores bancarios reales.

---

##  Integrantes
* **Miguel Santana Saldarriaga**
* **Juan Jose Hernandez Tobón**
* **Mariana Flórez Ramírez**

---


El sistema genera:
* **Tabla de amortización** completa.
* **Gráfica dinámica** del comportamiento del crédito.
* **Reportes (Beta)**: El sistema permite la descarga de archivos en formato Excel, aunque actualmente la exportación se encuentra en fase de ajuste para asegurar la precisión total de los datos transferidos.
  ## Consideraciones
* El sistema fue probado bajo condiciones normales de uso y no presenta errores críticos en la simulación web.
* **Nota sobre Excel**: La función de exportación está habilitada para descarga, pero se advierte que los datos en el archivo podrían no coincidir exactamente con la tabla web en ciertos escenarios complejos (funcionalidad en proceso de mejora).
---

##  Funcionalidades Principales

###  Simulación de Crédito
Permite realizar cálculos basados en:
* **Cuota fija** (Sistema Francés).
* **Abono constante** a capital (Sistema Alemán).
* **Tasa variable**.
* **Conversión de tasas**: Nominal a Efectiva, Vencida a Anticipada.
* **Abonos extraordinarios**: Aplicación de pagos adicionales al capital.

###  Visualización y Herramientas
* **Tabla detallada**: Desglose por periodo, cuota, interés, capital y saldo restante.
* **Gráfica interactiva**: Visualización de la evolución del crédito mediante **Chart.js**.
* **Reportes**: Generación de archivos descargables en formato Excel.

---

##  Ejecutable (Deploy)

Puedes acceder al sistema directamente desde el siguiente enlace:

[**Simulador de Crédito en Vivo**](https://simulador-credito-cws8.onrender.com/)

> [!WARNING]
> La aplicación está desplegada en **Render (plan gratuito)**, por lo que puede tardar unos segundos en "despertar" y cargar si ha estado inactiva.

---

##  Cómo ejecutar el proyecto localmente

### Requisitos
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Visual Studio 2022 o VS Code

### Pasos
1.  **Clonar el repositorio:**
    ```bash
    git clone [https://github.com/mafora12/SimuladorCredito.git](https://github.com/mafora12/SimuladorCredito.git)
    ```
2.  **Entrar al directorio:**
    ```bash
    cd SimuladorCredito
    ```
3.  **Restaurar dependencias:**
    ```bash
    dotnet restore
    ```
4.  **Ejecutar la aplicación:**
    ```bash
    dotnet run
    ```
5.  **Abrir en el navegador:**
    `http://localhost:5000` o la URL indicada en la terminal.
    
* Tambien es posible
  
1. **Clona el repositorio**
2. **Abre el proyecto** en Visual Studio
3. **Ejecuta** la aplicación (presiona `F5`)
4. **Se abre** automáticamente en tu navegador

---

##  Arquitectura y Diseño

El proyecto implementa una estructura basada en la separación de responsabilidades y patrones de diseño:

* **Models**: Representación de las entidades de datos (ej: `PaymentDetail`).
* **Services**: Capa donde reside toda la lógica de negocio y cálculos financieros.
* **Factory Pattern**: Utilizado para la creación dinámica de los diferentes tipos de crédito.
* **Facade Pattern**: Simplifica el acceso a la lógica del sistema desde la interfaz de usuario.

**Beneficios:** Escalabilidad, mantenimiento sencillo y bajo acoplamiento.

---

##  Pruebas Unitarias

Se incluye un proyecto de pruebas orientado a validar la integridad de los cálculos:
* ✅ Cálculo exacto de cuotas.
* ✅ Conversión precisa de tasas de interés.
* ✅ Comportamiento y reducción del saldo tras abonos.

---

##  Tecnologías Utilizadas
* **Backend:** ASP.NET Core Razor Pages (.NET 8)
* **Lenguaje:** C#
* **Frontend:** HTML, CSS, JavaScript
* **Gráficas:** Chart.js
* **Excel:** EPPlus

---

##  Recursos de Terceros
| Recurso | Uso | Licencia |
| :--- | :--- | :--- |
| **Chart.js** | Visualización de gráficos | MIT |
| **EPPlus** | Generación de archivos Excel | Polyform Noncommercial 1.0 |

---

##  Consideraciones
* El sistema fue probado bajo condiciones normales de uso.
* No presenta errores críticos en la simulación.
* Cumple con las funcionalidades planteadas inicialmente.
  
## Link para  la explicación del proyecto de curso   
https://docs.google.com/document/d/14o10_Fbp8LtMwTeqWNDWJmMpt_qhT-LE82HSwvJj8vQ/edit?usp=sharing


# Sistema de Gestión de Archivos de Texto Multiplataforma

**Trabajo Práctico Final: Programación e Investigación**
**Universidad Abierta Interamericana (UAI)**

* **Alumnos:** Agustín Lanati, Matías Patti
* **Materia:** Programación I / Estructura de Datos
* **Año:** 2025
* **Lenguaje:** C# (.NET 8)

---

## 1. Requisitos del sistema
Para ejecutar o compilar este proyecto se requiere:
* **SDK:** .NET 8.0 SDK (o superior).
* **IDE Recomendado:** Visual Studio 2022 (v17.8 o superior) con la carga de trabajo "Desarrollo de escritorio de .NET".
* **Sistema Operativo:** Windows 10/11, Linux o macOS (Es multiplataforma).

## 2. Instrucciones de compilación
### Opción A: Desde Visual Studio 2022 (Recomendado)
1.  Abrir el archivo de solución `.sln` o la carpeta del proyecto.
2.  Ir al menú **Compilar** > **Compilar solución** (o presionar `Ctrl + Shift + B`).
3.  Verificar que en la ventana de "Salida" indique "0 correctos, 0 incorrectos".

### Opción B: Desde línea de comandos (CLI)
1.  Abrir una terminal en la carpeta raíz del proyecto.
2.  Ejecutar el comando:
    ```bash
    dotnet build
    ```

## 3. Instrucciones de uso básico

1.  **Ejecución:**
    * Desde Visual Studio: Presionar el botón "Iniciar" (Play verde).
    * Desde consola: `dotnet run --project Lanati_Agustin_Patti_Matias_GestorAlumnos`

2.  **Menú Principal:**
    La aplicación despliega un menú numérico. Ingrese el número de la opción deseada y presione Enter.
    * **Opción 1 (Crear):** Genera archivos nuevos. Debe ingresar nombre, formato (TXT, CSV, JSON, XML) y cargar los alumnos.
    * **Opción 2 (Leer):** Visualiza los datos en una tabla formateada. Puede usar los archivos de prueba ubicados en la carpeta `ejemplos/`.
    * **Opción 3 (Modificar):** Permite Agregar, Editar o Eliminar alumnos de un archivo ya existente.
    * **Opción 5 (Convertir):** Transforma un archivo (ej: `.csv`) a otro formato (ej: `.json`) sin perder datos.
    * **Opción 6 (Reporte):** Genera un reporte agrupado por Apellido (Corte de Control) y lo muestra en pantalla con opción a guardar.

3.  **Archivos de Ejemplo:**
    Se incluyen archivos de prueba en la carpeta `ejemplos/` (como `alumnos_ejemplo.csv`) para probar la lectura inmediata.

## 4. Problemas conocidos

* **Formato estricto:** Si se intenta leer un archivo TXT o CSV que fue modificado manualmente y no respeta los separadores (`|` o `,`) o la cantidad de columnas, el programa podría indicar error de lectura.
* **Permisos de escritura:** Si se intenta crear o guardar archivos en carpetas protegidas del sistema (como `C:\Program Files`), la aplicación podría lanzar una excepción de acceso denegado. Se recomienda ejecutar en carpetas de usuario.

## 5. Extras implementados

Además de los requisitos mínimos, se implementaron las siguientes mejoras:

* **Validaciones Robustas de Duplicados:** El sistema impide cargar alumnos con **Legajo**, **DNI** o **Email** repetidos, verificando tanto contra el archivo existente como contra los datos que se están cargando en memoria en ese momento.
* **Validación de Formato de Email:** Se verifica que el correo contenga `@` y `.` antes de aceptarlo.
* **Backup Automático:** Al modificar un archivo, se genera automáticamente una copia de seguridad con extensión `.bak` antes de guardar los cambios.
* **Detección Automática de Formato:** El sistema detecta el tipo de archivo (JSON, XML, etc.) basándose en la extensión al momento de leer o convertir.
* **Alineación Visual (PadRight):** Las tablas y reportes utilizan métodos de alineación precisos para garantizar que las columnas se vean ordenadas independientemente de la longitud de los nombres.
* **Uso de LINQ:** Implementación de `GroupBy`, `OrderBy` y `Count` para la generación eficiente del reporte con corte de control. 

---


using Lanati_Agustin_Patti_Matias_GestorAlumnos.src.Models;

namespace Lanati_Agustin_Patti_Matias_GestorAlumnos.src
{
    public class GeneradorReportes
    {
        private GestorArchivos _gestor = new GestorArchivos();

        public void GenerarReporte()
        {
            Console.Clear();
            Console.WriteLine("=== REPORTE (CORTE DE CONTROL) ===");
            Console.Write("Archivo fuente: ");
            string path = Console.ReadLine();

            if (!File.Exists(path)) { Console.WriteLine("No existe."); Console.ReadKey(); return; }

            // Uso explícito de LINQ OrderBy y GroupBy
            var alumnos = _gestor.CargarAlumnos(path);
            var grupos = alumnos.OrderBy(a => a.Apellido)
                                .GroupBy(a => a.Apellido);

            StringWriter sw = new StringWriter();

            sw.WriteLine("REPORTE DE ALUMNOS POR APELLIDO".PadRight(50));
            sw.WriteLine($"Fecha: {DateTime.Now}");
            sw.WriteLine(new string('=', 60));

            int totalGeneral = 0;

            foreach (var grupo in grupos)
            {
                sw.WriteLine($"APELLIDO: {grupo.Key.ToUpper()}");
                sw.WriteLine(new string('-', 60));

                foreach (var alu in grupo)
                {
                    // Alineación con PadRight
                    sw.WriteLine("Legajo:".PadRight(20) + alu.Legajo);
                    sw.WriteLine("Nombres:".PadRight(20) + alu.Nombres);
                    sw.WriteLine("Documento:".PadRight(20) + alu.NumeroDocumento);
                    sw.WriteLine("Email:".PadRight(20) + alu.Email);
                    sw.WriteLine("Teléfono:".PadRight(20) + alu.Telefono);
                    sw.WriteLine("");
                }

                int subtotal = grupo.Count();
                sw.WriteLine($"→ Subtotal {grupo.Key.ToUpper()}: {subtotal} alumno(s)");
                sw.WriteLine(new string('-', 60));
                sw.WriteLine("");

                totalGeneral += subtotal;
            }

            sw.WriteLine(new string('=', 60));
            sw.WriteLine("Total Alumnos Registrados: " + totalGeneral.ToString().PadLeft(5));
            sw.WriteLine(new string('=', 60));

            string reporte = sw.ToString();
            Console.WriteLine(reporte);

            // --- VALIDACIÓN DE GUARDADO (S/N) ---
            string opcion = "";
            do
            {
                Console.Write("\n¿Desea guardar el reporte? (S/N): ");
                // Leemos, pasamos a mayúscula y quitamos espacios por las dudas
                opcion = Console.ReadLine()?.ToUpper().Trim();

                if (opcion != "S" && opcion != "N")
                {
                    Console.WriteLine(" Opción inválida. Por favor ingrese 'S' para Sí o 'N' para No.");
                }

            } while (opcion != "S" && opcion != "N");

            // Ejecutamos la acción según la respuesta válida
            if (opcion == "S")
            {
                string nombreReporte = $"Reporte_{DateTime.Now:yyyyMMdd_HHmm}.txt";
                File.WriteAllText(nombreReporte, reporte);
                Console.WriteLine($" Reporte guardado exitosamente como: {nombreReporte}");
            }
            else
            {
                Console.WriteLine(" No se guardó el reporte.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }
    }
}
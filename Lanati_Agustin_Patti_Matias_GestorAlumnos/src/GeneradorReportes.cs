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

            // 1. y 2. Uso explícito de LINQ OrderBy y GroupBy [cite: 219, 220]
            var alumnos = _gestor.CargarAlumnos(path);
            var grupos = alumnos.OrderBy(a => a.Apellido)
                                .GroupBy(a => a.Apellido);

            StringWriter sw = new StringWriter();

            // Título centrado usando PadLeft para simular centrado básico o simple PadRight para relleno
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
                    // 4. Uso de PadRight para alinear las etiquetas "Labels" [cite: 222]
                    // Así los valores (Legajo, Nombre, etc.) comienzan todos en la misma columna visual.
                    sw.WriteLine("Legajo:".PadRight(20) + alu.Legajo);
                    sw.WriteLine("Nombres:".PadRight(20) + alu.Nombres);
                    sw.WriteLine("Documento:".PadRight(20) + alu.NumeroDocumento);
                    sw.WriteLine("Email:".PadRight(20) + alu.Email);
                    sw.WriteLine("Teléfono:".PadRight(20) + alu.Telefono);
                    sw.WriteLine(""); // Espacio entre alumnos
                }

                // 3. Uso de Count() para contar registros por grupo [cite: 221]
                int subtotal = grupo.Count();
                sw.WriteLine($"→ Subtotal {grupo.Key.ToUpper()}: {subtotal} alumno(s)");
                sw.WriteLine(new string('-', 60));
                sw.WriteLine("");

                totalGeneral += subtotal;
            }

            sw.WriteLine(new string('=', 60));
            // Ejemplo de uso de PadLeft para el número final
            sw.WriteLine("Total Alumnos Registrados: " + totalGeneral.ToString().PadLeft(5));
            sw.WriteLine(new string('=', 60));

            string reporte = sw.ToString();
            Console.WriteLine(reporte);

            Console.Write("Guardar reporte? (S/N): ");
            if (Console.ReadLine()?.ToUpper() == "S")
            {
                File.WriteAllText($"Reporte_{DateTime.Now:yyyyMMdd_HHmm}.txt", reporte);
                Console.WriteLine("Reporte guardado.");
            }
            Console.ReadKey();
        }
    }
}
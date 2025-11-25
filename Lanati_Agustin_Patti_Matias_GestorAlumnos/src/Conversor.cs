using Lanati_Agustin_Patti_Matias_GestorAlumnos.src.Models;

namespace Lanati_Agustin_Patti_Matias_GestorAlumnos.src
{
    public class Conversor
    {
        private GestorArchivos _gestor = new GestorArchivos();

        public void ConvertirFormato()
        {
            Console.Clear();
            Console.WriteLine("=== CONVERTIR FORMATOS ===");
            Console.Write("Archivo origen (con extensión): ");
            string origen = Console.ReadLine();

            if (!File.Exists(origen)) { Console.WriteLine("No existe."); Console.ReadKey(); return; }

            try
            {
                var alumnos = _gestor.CargarAlumnos(origen);
                Console.WriteLine("Destino: 1.TXT 2.CSV 3.JSON 4.XML");
                GestorArchivos.Formato fmt = _gestor.ObtenerFormato(Console.ReadLine());
                Console.Write("Nombre destino (sin extensión): ");
                string nombre = Console.ReadLine();
                string path = $"{nombre}.{fmt.ToString().ToLower()}";

                _gestor.GuardarArchivo(path, alumnos, fmt);
                Console.WriteLine($"\n✓ Conversión exitosa a {path}");
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
            Console.ReadKey();
        }
    }
}
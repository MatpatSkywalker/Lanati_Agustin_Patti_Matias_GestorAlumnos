namespace Lanati_Agustin_Patti_Matias_GestorAlumnos.src
{
    public class Menu
    {
        GestorArchivos gestor = new GestorArchivos();
        Conversor conversor = new Conversor();
        GeneradorReportes reportes = new GeneradorReportes();

        public void MostrarMenu()
        {
            string op = "";
            do
            {
                Console.Clear();
                Console.WriteLine("=== GESTOR ALUMNOS (UAI) ===");
                Console.WriteLine("1. Crear Archivo");
                Console.WriteLine("2. Leer Archivo");
                Console.WriteLine("3. Modificar Archivo");
                Console.WriteLine("4. Eliminar Archivo");
                Console.WriteLine("5. Convertir Formato");
                Console.WriteLine("6. Reporte (Corte Control)");
                Console.WriteLine("0. Salir");
                Console.Write("Opción: ");
                op = Console.ReadLine();

                try
                {
                    switch (op)
                    {
                        case "1": gestor.CrearNuevoArchivo(); break;
                        case "2": gestor.LeerArchivo(); break;
                        case "3": gestor.ModificarArchivo(); break;
                        case "4": gestor.EliminarArchivoFisico(); break;
                        case "5": conversor.ConvertirFormato(); break;
                        case "6": reportes.GenerarReporte(); break;
                        case "0": break;
                    }
                }
                catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); Console.ReadKey(); }
            } while (op != "0");
        }
    }
}

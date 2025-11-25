using System.Text.Json;
using System.Xml.Serialization;
using Lanati_Agustin_Patti_Matias_GestorAlumnos.src.Models;

namespace Lanati_Agustin_Patti_Matias_GestorAlumnos.src
{
    public class GestorArchivos
    {
        public enum Formato { TXT, CSV, JSON, XML }

        // --- MÉTODOS DEL MENÚ ---
        public void CrearNuevoArchivo()
        {
            Console.Clear();
            Console.WriteLine("=== CREAR NUEVO ARCHIVO ===");

            // VALIDACIÓN 1: Nombre del archivo no puede estar vacío
            string nombre = "";
            do
            {
                Console.Write("Nombre del archivo (sin extensión): ");
                nombre = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.WriteLine(" Error: El nombre del archivo no puede estar vacío. Intente nuevamente.");
                }
            } while (string.IsNullOrWhiteSpace(nombre));

            Console.WriteLine("Formato: 1.TXT 2.CSV 3.JSON 4.XML");
            Formato formato = ObtenerFormato(Console.ReadLine());

            List<Alumno> alumnos = IngresarAlumnos();

            // Si por alguna razón la lista vuelve vacía (ej: canceló), no creamos nada
            if (alumnos.Count == 0)
            {
                Console.WriteLine("No se ingresaron alumnos. Operación cancelada.");
                Pausar();
                return;
            }

            string path = $"{nombre}.{formato.ToString().ToLower()}";
            GuardarArchivo(path, alumnos, formato);
            Console.WriteLine($"\n Archivo creado exitosamente en: {Path.GetFullPath(path)}");
            Pausar();
        }

        public void LeerArchivo()
        {
            Console.Clear();
            Console.WriteLine("=== LEER ARCHIVO ===");
            Console.Write("Nombre completo (ej: alumnos.csv): ");
            string path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                Console.WriteLine(" El archivo no existe o el nombre es inválido.");
                Pausar();
                return;
            }

            try
            {
                List<Alumno> alumnos = CargarAlumnos(path);
                MostrarTabla(alumnos);
            }
            catch (Exception ex) { Console.WriteLine($"Error crítico: {ex.Message}"); }
            Pausar();
        }

        public void ModificarArchivo()
        {
            Console.Clear();
            Console.WriteLine("=== MODIFICAR ARCHIVO ===");
            Console.Write("Nombre archivo a modificar: ");
            string path = Console.ReadLine();

            if (!File.Exists(path)) { Console.WriteLine("❌ Archivo no encontrado."); Pausar(); return; }

            List<Alumno> alumnos = CargarAlumnos(path);
            bool guardar = false;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Editando: {path} ({alumnos.Count} registros)");
                Console.WriteLine("1. Agregar alumno");
                Console.WriteLine("2. Modificar alumno");
                Console.WriteLine("3. Eliminar alumno");
                Console.WriteLine("4. Guardar y salir");
                Console.WriteLine("5. Cancelar");
                Console.Write("Opción: ");

                switch (Console.ReadLine())
                {
                    case "1": AgregarAlumnoLogica(alumnos); break;
                    case "2": ModificarAlumnoLogica(alumnos); break;
                    case "3": EliminarAlumnoLogica(alumnos); break;
                    case "4": guardar = true; goto FinMenu;
                    case "5": return;
                }
            }

        FinMenu:
            if (guardar)
            {
                File.Copy(path, path + ".bak", true);
                string ext = Path.GetExtension(path).TrimStart('.').ToUpper();
                Formato fmt = Enum.Parse<Formato>(ext);
                GuardarArchivo(path, alumnos, fmt);
                Console.WriteLine("✅ Cambios guardados correctamente.");
                Pausar();
            }
        }

        public void EliminarArchivoFisico()
        {
            Console.Clear();
            Console.Write("Nombre del archivo a eliminar: ");
            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                FileInfo fi = new FileInfo(path);
                Console.WriteLine($"\nInfo: {fi.Name}, {fi.Length / 1024.0:F2} KB, {fi.CreationTime}");
                Console.Write("Escriba 'CONFIRMAR' para borrar: ");
                if (Console.ReadLine()?.ToUpper() == "CONFIRMAR")
                {
                    File.Delete(path);
                    Console.WriteLine("🗑️ Archivo eliminado.");
                }
            }
            else Console.WriteLine("❌ El archivo no existe.");
            Pausar();
        }

        // --- MÉTODOS CORE ---

        public List<Alumno> CargarAlumnos(string path)
        {
            string contenido = File.ReadAllText(path);
            string ext = Path.GetExtension(path).ToLower();
            try
            {
                switch (ext)
                {
                    case ".json": return JsonSerializer.Deserialize<List<Alumno>>(contenido) ?? new List<Alumno>();
                    case ".xml":
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Alumno>), new XmlRootAttribute("Alumnos"));
                        using (StringReader reader = new StringReader(contenido)) return (List<Alumno>)serializer.Deserialize(reader);
                    case ".csv": return ParseCSV(contenido);
                    case ".txt": return ParseTXT(contenido);
                    default: throw new Exception("Formato no soportado.");
                }
            }
            catch (Exception ex) { throw new Exception($"Error lectura: {ex.Message}"); }
        }

        public void GuardarArchivo(string path, List<Alumno> alumnos, Formato formato)
        {
            string contenido = "";
            switch (formato)
            {
                case Formato.JSON: contenido = JsonSerializer.Serialize(alumnos, new JsonSerializerOptions { WriteIndented = true }); break;
                case Formato.XML:
                    XmlSerializer xml = new XmlSerializer(typeof(List<Alumno>), new XmlRootAttribute("Alumnos"));
                    using (StringWriter sw = new StringWriter()) { xml.Serialize(sw, alumnos); contenido = sw.ToString(); }
                    break;
                case Formato.CSV: contenido = "Legajo,Apellido,Nombres,NumeroDocumento,Email,Telefono\n" + string.Join("\n", alumnos.Select(a => a.ToCSV())); break;
                case Formato.TXT: contenido = string.Join("\n", alumnos.Select(a => a.ToTXT())); break;
            }
            File.WriteAllText(path, contenido);
        }

        // --- LÓGICA PRIVADA ---

        private void AgregarAlumnoLogica(List<Alumno> alumnos)
        {
            var nuevos = IngresarAlumnos(1);
            if (nuevos.Count > 0)
            {
                if (alumnos.Any(a => a.Legajo == nuevos[0].Legajo)) Console.WriteLine("⚠️ ¡Ese Legajo ya existe!");
                else alumnos.AddRange(nuevos);
            }
        }

        private void ModificarAlumnoLogica(List<Alumno> alumnos)
        {
            Console.Write("Legajo a modificar: ");
            var alu = alumnos.FirstOrDefault(a => a.Legajo == Console.ReadLine());
            if (alu == null) { Console.WriteLine("❌ Alumno no encontrado."); Pausar(); return; }

            Console.WriteLine("ℹ️  Presione Enter para mantener el valor actual.");
            alu.Apellido = EditarCampo("Apellido", alu.Apellido);
            alu.Nombres = EditarCampo("Nombres", alu.Nombres);
            alu.NumeroDocumento = EditarCampo("Documento", alu.NumeroDocumento);
            alu.Email = EditarCampo("Email", alu.Email);
            alu.Telefono = EditarCampo("Teléfono", alu.Telefono);
        }

        private void EliminarAlumnoLogica(List<Alumno> alumnos)
        {
            Console.Write("Legajo a eliminar: ");
            var alu = alumnos.FirstOrDefault(a => a.Legajo == Console.ReadLine());
            if (alu != null)
            {
                Console.Write($"Eliminar a {alu.Apellido}? (SI): ");
                if (Console.ReadLine()?.ToUpper() == "SI") alumnos.Remove(alu);
            }
            else
            {
                Console.WriteLine("❌ No encontrado.");
            }
        }

        private List<Alumno> IngresarAlumnos(int cantidad = -1)
        {
            List<Alumno> lista = new List<Alumno>();

            // VALIDACIÓN 2: Cantidad de alumnos debe ser número válido > 0
            if (cantidad == -1)
            {
                string inputCant;
                do
                {
                    Console.Write("Cantidad de alumnos a registrar: ");
                    inputCant = Console.ReadLine();
                    if (!int.TryParse(inputCant, out cantidad) || cantidad <= 0)
                    {
                        Console.WriteLine("⚠️ Error: Debe ingresar un número mayor a 0.");
                        cantidad = -1; // reset para seguir en el bucle
                    }
                } while (cantidad <= 0);
            }

            for (int i = 0; i < cantidad; i++)
            {
                Console.WriteLine($"\n--- Alumno {i + 1} ---");
                lista.Add(new Alumno
                {
                    Legajo = PedirDato("Legajo"),
                    Apellido = PedirDato("Apellido"),
                    Nombres = PedirDato("Nombres"),
                    NumeroDocumento = PedirDato("Documento"),
                    Email = PedirDato("Email", true),
                    Telefono = PedirDato("Teléfono")
                });
            }
            return lista;
        }

        // VALIDACIÓN 3: El dato no puede ser vacío
        private string PedirDato(string campo, bool esEmail = false)
        {
            string val;
            do
            {
                Console.Write($"{campo}: ");
                val = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(val))
                {
                    Console.WriteLine($"⚠️ El campo '{campo}' no puede estar vacío. Intente nuevamente.");
                }
                else if (esEmail && !val.Contains("@"))
                {
                    Console.WriteLine("⚠️ El email debe contener un '@'.");
                    val = ""; // Forzar repetición
                }
            } while (string.IsNullOrWhiteSpace(val));
            return val;
        }

        private string EditarCampo(string n, string v)
        {
            Console.Write($"{n} ({v}): ");
            string i = Console.ReadLine();
            // En editar permitimos vacío porque significa "no cambiar"
            return string.IsNullOrEmpty(i) ? v : i;
        }

        private List<Alumno> ParseCSV(string c) => c.Split('\n', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(l => l.Trim().Split(',')).Where(p => p.Length >= 6).Select(p => new Alumno { Legajo = p[0], Apellido = p[1], Nombres = p[2], NumeroDocumento = p[3], Email = p[4], Telefono = p[5] }).ToList();
        private List<Alumno> ParseTXT(string c) => c.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim().Split('|')).Where(p => p.Length >= 6).Select(p => new Alumno { Legajo = p[0], Apellido = p[1], Nombres = p[2], NumeroDocumento = p[3], Email = p[4], Telefono = p[5] }).ToList();

        private void MostrarTabla(List<Alumno> alumnos)
        {
            string linea = new string('=', 110);
            Console.WriteLine(linea);
            Console.WriteLine("| " + "Legajo".PadRight(10) + " | " + "Apellido".PadRight(15) + " | " + "Nombres".PadRight(20) + " | " + "Email".PadRight(30) + " | " + "Teléfono".PadRight(15) + " |");
            Console.WriteLine(linea);

            foreach (var a in alumnos)
            {
                string leg = a.Legajo.PadRight(10);
                string ape = a.Apellido.Length > 15 ? a.Apellido.Substring(0, 15) : a.Apellido.PadRight(15);
                string nom = a.Nombres.Length > 20 ? a.Nombres.Substring(0, 20) : a.Nombres.PadRight(20);
                string mail = a.Email.Length > 30 ? a.Email.Substring(0, 30) : a.Email.PadRight(30);
                string tel = a.Telefono.PadRight(15);
                Console.WriteLine($"| {leg} | {ape} | {nom} | {mail} | {tel} |");
            }
            Console.WriteLine($"Total: {alumnos.Count}");
        }

        public Formato ObtenerFormato(string op) => op switch { "2" => Formato.CSV, "3" => Formato.JSON, "4" => Formato.XML, _ => Formato.TXT };
        private void Pausar() { Console.WriteLine("\nTecla para continuar..."); Console.ReadKey(); }
    }
}
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

            string nombre = "";
            do
            {
                Console.Write("Nombre del archivo (sin extensión): ");
                nombre = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(nombre)) Console.WriteLine("⚠️ Error: El nombre no puede estar vacío.");
            } while (string.IsNullOrWhiteSpace(nombre));

            Formato formato = PedirFormato();

            List<Alumno> alumnos = IngresarAlumnos(new List<Alumno>());

            if (alumnos.Count == 0)
            {
                Console.WriteLine("Operación cancelada.");
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
            Console.Write("Nombre completo (ej: alumnos.txt): ");
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
            catch (Exception ex) { Console.WriteLine($"Error crítico leyendo archivo: {ex.Message}"); }
            Pausar();
        }

        public void ModificarArchivo()
        {
            Console.Clear();
            Console.WriteLine("=== MODIFICAR ARCHIVO ===");
            Console.Write("Nombre archivo a modificar: ");
            string path = Console.ReadLine();

            if (!File.Exists(path)) { Console.WriteLine(" Archivo no encontrado."); Pausar(); return; }

            List<Alumno> alumnos = CargarAlumnos(path);
            bool guardar = false;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Editando: {path} ({alumnos.Count} registros)");
                Console.WriteLine("1. Agregar alumno");
                Console.WriteLine("2. Modificar alumno (por legajo)");
                Console.WriteLine("3. Eliminar alumno (por legajo)");
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
                Console.WriteLine(" Cambios guardados correctamente.");
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
                    Console.WriteLine(" Archivo eliminado.");
                }
            }
            else Console.WriteLine(" El archivo no existe.");
            Pausar();
        }

        // --- MÉTODOS CORE ---

        // ESTE ES EL MÉTODO QUE USA EL CONVERSOR PARA NO ROMPERSE
        public Formato PedirFormato()
        {
            string op;
            do
            {
                Console.WriteLine("Seleccione Formato: 1.TXT 2.CSV 3.JSON 4.XML");
                Console.Write("Opción: ");
                op = Console.ReadLine();

                if (op != "1" && op != "2" && op != "3" && op != "4")
                {
                    Console.WriteLine(" Opción inválida. Ingrese un número del 1 al 4.");
                }
            } while (op != "1" && op != "2" && op != "3" && op != "4");

            return op switch { "2" => Formato.CSV, "3" => Formato.JSON, "4" => Formato.XML, _ => Formato.TXT };
        }

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

        private void AgregarAlumnoLogica(List<Alumno> alumnosExistentes)
        {
            var nuevos = IngresarAlumnos(alumnosExistentes, 1);
            if (nuevos.Count > 0) alumnosExistentes.AddRange(nuevos);
        }

        private void ModificarAlumnoLogica(List<Alumno> alumnos)
        {
            Console.Write("Legajo a modificar: ");
            var alu = alumnos.FirstOrDefault(a => a.Legajo == Console.ReadLine());
            if (alu == null) { Console.WriteLine(" Alumno no encontrado."); Pausar(); return; }

            Console.WriteLine("  Enter para mantener valor actual.");

            // AQUÍ SE USA LA NUEVA EDICIÓN VALIDADA
            alu.Apellido = EditarDatoValidado("Apellido", alu.Apellido);
            alu.Nombres = EditarDatoValidado("Nombres", alu.Nombres);
            alu.NumeroDocumento = EditarDatoValidado("Documento", alu.NumeroDocumento);
            alu.Email = EditarDatoValidado("Email", alu.Email, true); // Valida formato email
            alu.Telefono = EditarDatoValidado("Teléfono", alu.Telefono);
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
            else Console.WriteLine(" No encontrado.");
        }

        private List<Alumno> IngresarAlumnos(List<Alumno> alumnosExistentes, int cantidad = -1)
        {
            List<Alumno> listaNueva = new List<Alumno>();

            if (cantidad == -1)
            {
                string inputCant;
                do
                {
                    Console.Write("Cantidad de alumnos: ");
                    inputCant = Console.ReadLine();
                    if (!int.TryParse(inputCant, out cantidad) || cantidad <= 0)
                    {
                        Console.WriteLine(" Ingrese un número mayor a 0.");
                        cantidad = -1;
                    }
                } while (cantidad <= 0);
            }

            for (int i = 0; i < cantidad; i++)
            {
                Console.WriteLine($"\n--- Alumno {i + 1} ---");
                Alumno a = new Alumno();

                do
                {
                    string input = PedirDato("Legajo");
                    if (EsDuplicado(input, alumnosExistentes, listaNueva, x => x.Legajo))
                    {
                        Console.WriteLine(" Error: El Legajo ya existe.");
                        a.Legajo = "";
                    }
                    else a.Legajo = input;
                } while (string.IsNullOrEmpty(a.Legajo));

                a.Apellido = PedirDato("Apellido");
                a.Nombres = PedirDato("Nombres");

                do
                {
                    string input = PedirDato("Documento");
                    if (EsDuplicado(input, alumnosExistentes, listaNueva, x => x.NumeroDocumento))
                    {
                        Console.WriteLine(" Error: El Documento ya existe.");
                        a.NumeroDocumento = "";
                    }
                    else a.NumeroDocumento = input;
                } while (string.IsNullOrEmpty(a.NumeroDocumento));

                do
                {
                    string input = PedirDato("Email", true);
                    if (EsDuplicado(input, alumnosExistentes, listaNueva, x => x.Email))
                    {
                        Console.WriteLine(" Error: El Email ya existe.");
                        a.Email = "";
                    }
                    else a.Email = input;
                } while (string.IsNullOrEmpty(a.Email));

                do
                {
                    string input = PedirDato("Teléfono");
                    if (EsDuplicado(input, alumnosExistentes, listaNueva, x => x.Telefono))
                    {
                        Console.WriteLine(" Error: El Teléfono ya existe.");
                        a.Telefono = "";
                    }
                    else a.Telefono = input;
                } while (string.IsNullOrEmpty(a.Telefono));

                listaNueva.Add(a);
            }
            return listaNueva;
        }

        private bool EsDuplicado(string valor, List<Alumno> existentes, List<Alumno> nuevos, Func<Alumno, string> selector)
        {
            return existentes.Any(a => selector(a) == valor) || nuevos.Any(a => selector(a) == valor);
        }

        private string PedirDato(string campo, bool esEmail = false)
        {
            string val;
            do
            {
                Console.Write($"{campo}: ");
                val = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(val))
                {
                    Console.WriteLine($" El campo '{campo}' no puede estar vacío.");
                }
                else if (esEmail)
                {
                    if (!val.Contains("@") || !val.Contains("."))
                    {
                        Console.WriteLine(" Formato incorrecto. Debe contener '@' y un punto.");
                        val = "";
                    }
                }
            } while (string.IsNullOrWhiteSpace(val));
            return val;
        }

        // NUEVO MÉTODO PARA EDITAR CON VALIDACIÓN DE FORMATO
        private string EditarDatoValidado(string nombre, string valorActual, bool esEmail = false)
        {
            while (true)
            {
                Console.Write($"{nombre} ({valorActual}): ");
                string input = Console.ReadLine();

                // Si está vacío, devuelve el valor viejo (no cambia)
                if (string.IsNullOrEmpty(input)) return valorActual;

                // Si escribió algo, validamos formato si es email
                if (esEmail)
                {
                    if (!input.Contains("@") || !input.Contains("."))
                    {
                        Console.WriteLine(" Formato de email inválido (debe tener @ y .). Intente de nuevo.");
                        continue; // Vuelve a pedir
                    }
                }

                // Si pasa las validaciones, devuelve el nuevo valor
                return input;
            }
        }

        private List<Alumno> ParseCSV(string c)
        {
            return c.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1)
                    .Select(l => l.Split(','))
                    .Where(p => p.Length >= 6)
                    .Select(p => new Alumno
                    {
                        Legajo = p[0].Trim(),
                        Apellido = p[1].Trim(),
                        Nombres = p[2].Trim(),
                        NumeroDocumento = p[3].Trim(),
                        Email = p[4].Trim(),
                        Telefono = p[5].Trim()
                    }).ToList();
        }

        private List<Alumno> ParseTXT(string c)
        {
            return c.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.Split('|'))
                    .Where(p => p.Length >= 6)
                    .Select(p => new Alumno
                    {
                        Legajo = p[0].Trim(),
                        Apellido = p[1].Trim(),
                        Nombres = p[2].Trim(),
                        NumeroDocumento = p[3].Trim(),
                        Email = p[4].Trim(),
                        Telefono = p[5].Trim()
                    }).ToList();
        }

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
                string telRaw = a.Telefono ?? "";
                string tel = telRaw.Length > 15 ? telRaw.Substring(0, 15) : telRaw.PadRight(15);

                Console.WriteLine($"| {leg} | {ape} | {nom} | {mail} | {tel} |");
            }
            Console.WriteLine($"Total: {alumnos.Count}");
        }
        private void Pausar() { Console.WriteLine("\nTecla para continuar..."); Console.ReadKey(); }
    }
}
using System.Xml.Serialization;

namespace Lanati_Agustin_Patti_Matias_GestorAlumnos.src.Models
{
    [XmlRoot("Alumno")]
    public class Alumno
    {
        public string Legajo { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        public Alumno() { }

        public string ToCSV() => $"{Legajo},{Apellido},{Nombres},{NumeroDocumento},{Email},{Telefono}";
        public string ToTXT() => $"{Legajo}|{Apellido}|{Nombres}|{NumeroDocumento}|{Email}|{Telefono}";
    }
}

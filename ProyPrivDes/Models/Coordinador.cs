using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyPrivDes.Models
{
    public class Coordinador
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaAlt { get; set; }
        public int EstadoId { get; set; }
    }
}
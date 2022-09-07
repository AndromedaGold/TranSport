using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyPrivDes.Models
{
    public class Fabricacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int TipoPisId { get; set; }
        public string Color { get; set; }
        public string CantidMaterPrim { get; set; }
        public int CantidadFabri { get; set; }
        public DateTime Fecha { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyPrivDes.Models
{
    public class Extraccion
    {
        public int Id { get; set; }
        public string NombreMaterial { get; set; }
        public decimal CantidadTons { get; set; }
        public DateTime FechaExtraccion { get; set; }
        public string Pureza { get; set; }
    }
}
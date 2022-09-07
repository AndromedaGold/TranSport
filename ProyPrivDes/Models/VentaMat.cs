using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyPrivDes.Models
{
    public class VentaMat
    {
        public int Id { get; set; }
        public string NombreMat { get; set; }
        public string Cantidad { get; set; }
        public string Cliente { get; set; }
        public string Destino { get; set; }
        public Decimal Precio { get; set; }
        public DateTime FechaVent { get; set; }
    }
}
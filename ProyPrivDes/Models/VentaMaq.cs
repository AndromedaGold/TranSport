using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyPrivDes.Models
{
    public class VentaMaq
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Color { get; set; }
        public string Peso { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public Decimal Precio { get; set; }
        public DateTime FechaVenta { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyPrivDes.Models
{
    public class Mezclado
    {
        public int Id { get; set; }
        public string MateriaPrim { get; set; }
        public string Cantidad { get; set; }
        public int FinuraId { get; set; }
        public DateTime Fecha { get; set; }
    }
}
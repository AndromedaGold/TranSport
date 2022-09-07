using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyPrivDes.Models
{
    public class Usuarios
    {
        public int idUsuario { get; set; }
        public string nombrePersona { get; set; }
        public string nombreUsuario { get; set; }
        public string nombreRol { get; set; }
        public string nombreTipoEmpleado { get; set; }
    }
}
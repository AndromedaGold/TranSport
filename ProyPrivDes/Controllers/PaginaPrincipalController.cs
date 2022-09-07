using ProyPrivDes.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyPrivDes.Controllers
{
    [Seguridad]
    public class PaginaPrincipalController : Controller
    {
        // GET: PaginaPrincipal
        public ActionResult Index()
        {
            int idusuario = (int)Session["idusuario"];

            using (ConexionDataContext bd = new ConexionDataContext())
            {
                string nombre = "";
                Usuario usuario = bd.Usuarios.Where(p => p.IdUsuario == idusuario).First();
                if (usuario.TipoUsuario == 'C')
                {
                    Coordinador oCoordinador = bd.Coordinadors.Where(p => p.Id == usuario.Id).First();
                    nombre = oCoordinador.Nombre + " " + oCoordinador.Apellido;
                    ViewBag.nombre = nombre;
                }
                else
                {
                    Operador oOperador = bd.Operadors.Where(p => p.Id == usuario.Id).First();
                    nombre = oOperador.Nombre + " " + oOperador.Apellido;
                    ViewBag.nombre = nombre;
                }
            }
            return View();
        }
    }
}
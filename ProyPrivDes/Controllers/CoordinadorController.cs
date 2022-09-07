using ProyPrivDes.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyPrivDes.Controllers
{
    [Seguridad]
    public class CoordinadorController : Controller
    {
        // GET: Coordinador
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult listarCoordinador()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.Coordinadors.Where(p => p.EstadoId.Equals(1)).Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Apellido,
                p.Telefono,
                FechaAlt = ((DateTime)p.FechaAlt).ToShortDateString()
            }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult mostrarInfo(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var mostrar = bd.Coordinadors.Where(p => p.Id.Equals(id)).Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Apellido,
                p.Telefono,
                FechaAlt = ((DateTime)p.FechaAlt).ToShortDateString(),
                p.EstadoId
            }).ToList();
            return Json(mostrar, JsonRequestBehavior.AllowGet);
        }

        public JsonResult listarEstado()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var est = bd.Estados.Select(p => new
            {
                ID = p.Id,
                p.Descripcion
            });
            return Json(est, JsonRequestBehavior.AllowGet);
        }

        public int eliminar(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                Coordinador oCoordina = bd.Coordinadors.Where(p => p.Id.Equals(id)).First();
                oCoordina.EstadoId = 0;
                bd.SubmitChanges();
                nregistrosAfectados = 1;
            }
            catch (Exception ex)
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }

        public int guardarDatos(Coordinador oCoordinador)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                int idCoordinador = oCoordinador.Id;
                if (idCoordinador.Equals(0))
                {
                    //Guarda nuevo
                    int nVeces = bd.Coordinadors.Where(p => p.Nombre.Equals(oCoordinador.Nombre) && p.Apellido.Equals(oCoordinador.Apellido)).Count();
                    if (nVeces == 0)
                    {
                        oCoordinador.IdTipoUsuario = 'C';
                        oCoordinador.bTieneUsuario = 0;
                        bd.Coordinadors.InsertOnSubmit(oCoordinador);
                        bd.SubmitChanges();
                        nregistrosAfectados = 1;
                    }
                    else
                    {
                        //el dato ya existe
                        nregistrosAfectados = -1;
                    }
                }
                else
                {
                    //editar
                    int nVeces = bd.Coordinadors.Where(p => p.Nombre.Equals(oCoordinador.Nombre) && p.Apellido.Equals(oCoordinador.Apellido) && !p.Id.Equals(idCoordinador)).Count();
                    if (nVeces == 0)
                    {
                        Coordinador coo = bd.Coordinadors.Where(p => p.Id.Equals(idCoordinador)).First();
                        coo.Nombre = oCoordinador.Nombre;
                        coo.Apellido = oCoordinador.Apellido;
                        coo.Telefono = oCoordinador.Telefono;
                        coo.FechaAlt = oCoordinador.FechaAlt;
                        coo.EstadoId = oCoordinador.EstadoId;
                        bd.SubmitChanges();
                        nregistrosAfectados = 1;
                    }
                    else
                    {
                        //no edita si ya existe
                        nregistrosAfectados = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }

        public JsonResult filtrarEstado(int idEstado)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var filtrar = bd.Coordinadors.Where(p => p.EstadoId.Equals(idEstado)).Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Apellido,
                p.Telefono,
                FechaAlt = ((DateTime)p.FechaAlt).ToShortDateString(),
                p.EstadoId
            }).ToList();
            return Json(filtrar, JsonRequestBehavior.AllowGet);
        }
    }
}
using ProyPrivDes.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyPrivDes.Controllers
{
    [Seguridad]
    public class OperadorController : Controller
    {
        // GET: Operador
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult filtrarEstado(int idEstado)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var filtrar = bd.Operadors.Where(p => p.EstadoId.Equals(idEstado)).Select(p => new
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
        public JsonResult listarOperador()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.Operadors.Where(p => p.EstadoId.Equals(1)).Select(p => new
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
            var mostrar = bd.Operadors.Where(p => p.Id.Equals(id)).Select(p => new
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
                Operador oOpera = bd.Operadors.Where(p => p.Id.Equals(id)).First();
                oOpera.EstadoId = 0;
                bd.SubmitChanges();
                nregistrosAfectados = 1;
            }
            catch (Exception ex)
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }

        public int guardar(Operador oOperador)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                int idOperador = oOperador.Id;
                if (idOperador.Equals(0))
                {
                    int nVeces = bd.Operadors.Where(p => p.Nombre.Equals(oOperador.Nombre) && p.Apellido.Equals(oOperador.Apellido)).Count();
                    if (nVeces == 0)
                    {
                        oOperador.IdTipoUsuario = 'O';
                        oOperador.bTieneUsuario = 0;
                        //Guarda nuevo
                        bd.Operadors.InsertOnSubmit(oOperador);
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
                    int nVeces = bd.Operadors.Where(p => p.Nombre.Equals(oOperador.Nombre) && p.Apellido.Equals(oOperador.Apellido) && !p.Id.Equals(idOperador)).Count();
                    if (nVeces == 0)
                    {
                        Operador ope = bd.Operadors.Where(p => p.Id.Equals(idOperador)).First();
                        ope.Nombre = oOperador.Nombre;
                        ope.Apellido = oOperador.Apellido;
                        ope.Telefono = oOperador.Telefono;
                        ope.FechaAlt = oOperador.FechaAlt;
                        ope.EstadoId = oOperador.EstadoId;
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using ProyPrivDes.Filtros;

namespace ProyPrivDes.Controllers
{
    [Seguridad]
    public class RolPaginaController : Controller
    {
        // GET: RolPagina
        public ActionResult Index()
        {
            return View();
        }
        //Listamos los roles
        public JsonResult listarRol()
        {
            using (ConexionDataContext bd = new ConexionDataContext())
            {
                
                var lista = bd.Rols.Where(p => p.Habilitado == 1).Select(p => new
                {
                    p.IdRol,
                    p.Nombre,
                    p.Descripcion
                }).ToList();
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
        }
        //Listamos las paginas
        public JsonResult listarPaginas()
        {
            using (ConexionDataContext bd = new ConexionDataContext())
            {
                var lista = bd.Paginas.Where(p => p.Habilitado == 1).Select(p => new
                {
                    p.IdPagina,
                    p.Mensaje,
                    p.Habilitado
                }).ToList();
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult obtenerRol(int oRol)
        {
            using (ConexionDataContext bd = new ConexionDataContext())
            {
                var rol = bd.Rols.Where(p => p.IdRol == oRol).Select(p => new
                {
                    p.IdRol,
                    p.Nombre,
                    p.Descripcion
                }).First();
                return Json(rol, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult obtenerRolPagina(int oRol)
        {
            using (ConexionDataContext bd = new ConexionDataContext())
            {
                var lista = bd.RolPaginas.Where(p => p.IdRol == oRol && p.Habilitado == 1).Select(p => new
                {
                    p.IdRol,
                    p.IdPagina,
                    p.Habilitado
                }).ToList();
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
        }

        public int guardarDatos(Rol oRol, string dataEnviar)
        {
            int rpta = 0;
            try
            {
                using (ConexionDataContext bd = new ConexionDataContext())
                {
                    using (var transaccion = new TransactionScope())
                    {
                        if (oRol.IdRol == 0)
                        {
                            Rol orol = new Rol();
                            orol.Nombre = oRol.Nombre;
                            orol.Descripcion = oRol.Descripcion;
                            orol.Habilitado = oRol.Habilitado;
                            bd.Rols.InsertOnSubmit(orol);
                            bd.SubmitChanges();

                            string[] codes = dataEnviar.Split('$');
                            for (var i = 0; i < codes.Length; i++)
                            {
                                RolPagina oRolpagina = new RolPagina();
                                oRolpagina.IdRol = orol.IdRol;
                                oRolpagina.IdPagina = int.Parse(codes[i]);
                                oRolpagina.Habilitado = 1;
                                bd.RolPaginas.InsertOnSubmit(oRolpagina);
                            }
                            rpta = 1;
                            bd.SubmitChanges();
                            transaccion.Complete();
                        }
                        else
                        {
                            //Editamos
                            Rol orol = bd.Rols.Where(p => p.IdRol == oRol.IdRol).First();
                            orol.Nombre = oRol.Nombre;
                            orol.Descripcion = oRol.Descripcion;

                            //Deshabilitamos
                            var lista = bd.RolPaginas.Where(p => p.IdRol == oRol.IdRol);
                            foreach (RolPagina oRolpagina in lista)
                            {
                                oRolpagina.Habilitado = 0;
                            }
                            //habilitamos
                            string[] codes = dataEnviar.Split('$');
                            for (var i = 0; i < codes.Length; i++)
                            {
                                int cantidad = bd.RolPaginas.Where(p => p.IdRol == oRol.IdRol && p.IdPagina == int.Parse(codes[i])).Count();
                                if (cantidad == 0)
                                {
                                    RolPagina oRolpagina = new RolPagina();
                                    oRolpagina.IdRol = orol.IdRol;
                                    oRolpagina.IdPagina = int.Parse(codes[i]);
                                    oRolpagina.Habilitado = 1;
                                    bd.RolPaginas.InsertOnSubmit(oRolpagina);
                                }
                                else
                                {
                                    RolPagina oRolpagina = bd.RolPaginas.Where(p => p.IdRol == oRol.IdRol && p.IdPagina == int.Parse(codes[i])).First();
                                    oRolpagina.Habilitado = 1;
                                }
                            }
                            rpta = 1;
                            bd.SubmitChanges();
                            transaccion.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                rpta = 0;
            }
            return rpta;
        }
    }
}
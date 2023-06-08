using ProyPrivDes.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ProyPrivDes.Reportes;
using System.IO;

namespace ProyPrivDes.Controllers
{
    [Seguridad]
    public class FabricacionController : Controller
    {
        // GET: Fabricacion
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RptFabricacion()
        {
            var reporte = new ReportClass();
            reporte.FileName = Server.MapPath("/Reportes/RptFabricacion.rpt");

            //conexion para el reporte
            var coninfo = ReportesConexion.GetConexion();
            TableLogOnInfo logoninfo = new TableLogOnInfo();
            Tables tables;
            tables = reporte.Database.Tables;

            foreach (Table item in tables)
            {
                logoninfo = item.LogOnInfo;
                logoninfo.ConnectionInfo = coninfo;
                item.ApplyLogOnInfo(logoninfo);
            }
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Stream stream = reporte.ExportToStream(ExportFormatType.PortableDocFormat);
            return new FileStreamResult(stream, "application/pdf");
        }

        public JsonResult listarFabricacion()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.Fabricacions.Where(p => p.Habilitado.Equals(1)).Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Color,
                p.CantidMaterPrim,
                p.CantidadFabri,
                Fecha = ((DateTime)p.Fecha).ToShortDateString()
            }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscarFabricacion(string nombrePi)
        {
            ConexionDataContext bd = new ConexionDataContext();
           var lista = bd.Fabricacions.Where(p => p.Nombre.Contains(nombrePi)).Select(p => new
            {
               p.Id,
               p.Nombre,
               p.Color,
               p.CantidMaterPrim,
               p.CantidadFabri,
               Fecha = ((DateTime)p.Fecha).ToShortDateString()
           }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult listarTipoPiso()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.TipoPisos.Select(p => new
            {
                ID = p.Id,
                p.Descripcion
            });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult mostrarInfo(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.Fabricacions.Where(p => p.Id.Equals(id)).Select(p => new
            {
                p.Id,
                p.Nombre,
                p.TipoPisoId,
                p.Color,
                p.CantidMaterPrim,
                p.CantidadFabri,
                Fecha = ((DateTime)p.Fecha).ToShortDateString(),
                p.Habilitado
            }).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int guardar(Fabricacion oFabricacion)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                //inserta si es nuevo
                if (oFabricacion.Id == 0)
                {
                    bd.Fabricacions.InsertOnSubmit(oFabricacion);
                    bd.SubmitChanges();
                    nregistrosAfectados = 1;
                }
                //Edita si ya existe
                else
                {
                    Fabricacion fab = bd.Fabricacions.Where(p => p.Id.Equals(oFabricacion.Id)).First();
                    fab.Nombre = oFabricacion.Nombre;
                    fab.TipoPisoId = oFabricacion.TipoPisoId;
                    fab.Color = oFabricacion.Color;
                    fab.CantidMaterPrim = oFabricacion.CantidMaterPrim;
                    fab.CantidadFabri = oFabricacion.CantidadFabri;
                    fab.Fecha = oFabricacion.Fecha;
                    fab.Habilitado = oFabricacion.Habilitado;

                    bd.SubmitChanges();
                    nregistrosAfectados = 1;
                }
            }
            catch (Exception ex)
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }

        public int eliminar(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nergistrosAfectados = 0;
            try
            {
                Fabricacion oFabricacion = bd.Fabricacions.Where(p => p.Id.Equals(id)).First();
                oFabricacion.Habilitado = 0;
                bd.SubmitChanges();
                nergistrosAfectados = 1;
            }
            catch (Exception ex)
            {
                nergistrosAfectados = 0;
            }

            return nergistrosAfectados;
        }
    }
}
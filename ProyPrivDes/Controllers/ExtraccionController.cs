using ProyPrivDes.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//Usings para trabajar los reportes
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ProyPrivDes.Reportes;
using System.IO;

namespace ProyPrivDes.Controllers
{
    [Seguridad]
    public class ExtraccionController : Controller
    {
        // GET: Extraccion
        public ActionResult Index()
        {
            return View();
        }
        //Reporte
        public ActionResult RptExtraccion()
        {
            var reporte = new ReportClass();
            reporte.FileName = Server.MapPath("/Reportes/RptExtraccion.rpt");

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
        //Metodo para mostrar lista de materiales de extraccion
        public JsonResult listarExtraccion()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.Extraccions.Where(p => p.Habilitado.Equals(1)).Select(p => new
            {
                p.Id,
                p.NombreMaterial,
                p.CantidadTons,
                FechaExtraccion = ((DateTime)p.FechaExtraccion).ToShortDateString(),
                p.Pureza
            }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscarExtraccion(string nombre)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var buscar = bd.Extraccions.Where(p => p.Habilitado.Equals(1) && p.NombreMaterial.Contains(nombre)).Select(p => new
            {
                p.Id,
                p.NombreMaterial,
                p.CantidadTons,
                FechaExtraccion = ((DateTime)p.FechaExtraccion).ToShortDateString(),
                p.Pureza
            }).ToList();
            return Json(buscar, JsonRequestBehavior.AllowGet);
        }

        public JsonResult mostrarInfo(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.Extraccions.Where(p => p.Id.Equals(id)).Select(p => new
            {
                p.Id,
                p.NombreMaterial,
                p.CantidadTons,
                FechaExtraccion = ((DateTime)p.FechaExtraccion).ToShortDateString(),
                p.Pureza,
                p.Habilitado
            }).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int eliminar(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                Extraccion oExtraccion = bd.Extraccions.Where(p => p.Id.Equals(id)).First();
                oExtraccion.Habilitado = 0;
                bd.SubmitChanges();
                nregistrosAfectados = 1;
            }
            catch (Exception ex)
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }

        public int guardar(Extraccion oExtraccion)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                //inserta si es nuevo
                if (oExtraccion.Id == 0)
                {
                    bd.Extraccions.InsertOnSubmit(oExtraccion);
                    bd.SubmitChanges();
                    nregistrosAfectados = 1;
                }
                //Edita si ya existe
                else
                {
                    Extraccion obj = bd.Extraccions.Where(p => p.Id.Equals(oExtraccion.Id)).First();
                    obj.NombreMaterial = oExtraccion.NombreMaterial;
                    obj.CantidadTons = oExtraccion.CantidadTons;
                    obj.FechaExtraccion = oExtraccion.FechaExtraccion;
                    obj.Pureza = oExtraccion.Pureza;
                    obj.Habilitado = oExtraccion.Habilitado;

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
    }
}
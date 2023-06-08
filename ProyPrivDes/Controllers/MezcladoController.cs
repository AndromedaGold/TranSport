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
    public class MezcladoController : Controller
    {
        // GET: Mezclado
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult listarFinura()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.Finuras.Select(p => new
            {
                ID = p.Id,
                p.Descripcion
            });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult listarMezclado()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.Mezclados.Where(p => p.Habilitado.Equals(1)).Select(p => new
            {
                p.Id,
                p.MateriaPrim,
                p.Cantidad,
                p.FinuraId,
                Fecha = ((DateTime)p.Fecha).ToShortDateString()
            }).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscarMezclado(string nombreMat)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.Mezclados.Where(p => p.MateriaPrim.Contains(nombreMat)).Select(p => new
            {
                p.Id,
                p.MateriaPrim,
                p.Cantidad,
                p.FinuraId,
                Fecha = ((DateTime)p.Fecha).ToShortDateString()
            }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult mostrarInfo(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.Mezclados.Where(p => p.Id.Equals(id)).Select(p => new
            {
                p.Id,
                p.MateriaPrim,
                p.Cantidad,
                p.FinuraId,
                Fecha = ((DateTime)p.Fecha).ToShortDateString(),
                p.Habilitado
            }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int eliminar(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistradosAfectados = 0;
            try
            {
                Mezclado oMezclado = bd.Mezclados.Where(p => p.Id.Equals(id)).First();
                oMezclado.Habilitado = 0;
                bd.SubmitChanges();
                nregistradosAfectados = 1;
            }
            catch (Exception ex)
            {
                nregistradosAfectados = 0;
            }
            return nregistradosAfectados;
        }

        public int guardar(Mezclado oMezclado)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                //Inserta si es nuevo
                int idMez = oMezclado.Id;
                if (idMez == 0)
                {
                    //valida si ya existe
                    int nVeces = bd.Mezclados.Where(p => p.Fecha.Equals(oMezclado.Fecha) && p.Id.Equals(oMezclado.Id)).Count();
                    if (nVeces == 0)
                    {
                        bd.Mezclados.InsertOnSubmit(oMezclado);
                        bd.SubmitChanges();
                        nregistrosAfectados = 1;
                    }
                    else
                    {
                        nregistrosAfectados = -1;
                    }
                }
                //edita si ya existe
                else
                {
                    //valida si ya existe
                    int nVeces = bd.Mezclados.Where(p => p.Fecha.Equals(oMezclado.Fecha) && p.Id.Equals(oMezclado.Id) && !p.Id.Equals(idMez)).Count();
                    if (nVeces == 0)
                    {
                        Mezclado mez = bd.Mezclados.Where(p => p.Id.Equals(oMezclado.Id)).First();
                        mez.MateriaPrim = oMezclado.MateriaPrim;
                        mez.Cantidad = oMezclado.Cantidad;
                        mez.FinuraId = oMezclado.FinuraId;
                        mez.Fecha = oMezclado.Fecha;
                        mez.Habilitado = oMezclado.Habilitado;
                        bd.SubmitChanges();
                        nregistrosAfectados = 1;
                    }
                    else
                    {
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

        public ActionResult RptMezclado()
        {
            var reporte = new ReportClass();
            reporte.FileName = Server.MapPath("/Reportes/RptMezclado.rpt");

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
    }
}
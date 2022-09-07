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
    public class RentaMaqController : Controller
    {
        // GET: RentaMaq
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult listarRenta()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.RentaMaqs.Where(p => p.Habilitado.Equals(1)).Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Color,
                p.Marca,
                p.Modelo,
                p.Precio,
                FechaRenta = ((DateTime)p.FechaRenta).ToShortDateString()
            }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscarRentaMaq(string nombre)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var buscar = bd.RentaMaqs.Where(p => p.Habilitado.Equals(1) && p.Nombre.Contains(nombre)).Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Color,
                p.Marca,
                p.Modelo,
                p.Precio,
                FechaRenta = ((DateTime)p.FechaRenta).ToShortDateString()
            }).ToList();
            return Json(buscar, JsonRequestBehavior.AllowGet);
        }

        public JsonResult mostrarInfo(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var mostrar = bd.RentaMaqs.Where(p => p.Id.Equals(id)).Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Color,
                p.Marca,
                p.Modelo,
                p.Precio,
                p.CombustibleId,
                p.TipoMaqId,
                FechaRenta = ((DateTime)p.FechaRenta).ToShortDateString(),
                p.Habilitado
            }).ToList();
            return Json(mostrar, JsonRequestBehavior.AllowGet);
        }

        public JsonResult listarCombustible()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.Combustibles.Select(p => new
            {
                ID = p.Id,
                p.Descripcion
            });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult listarTipoMaq()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.TipoMaqs.Select(p => new
            {
                ID = p.Id,
                p.Descripcion
            });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int eliminar(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                RentaMaq oRentaMaq = bd.RentaMaqs.Where(p => p.Id.Equals(id)).First();
                oRentaMaq.Habilitado = 0;
                bd.SubmitChanges();
                nregistrosAfectados = 1;
            }
            catch (Exception ex)
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }

        public int guardar(RentaMaq oRentaMaq)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfetados = 0;
            try
            {
                //inserta si es nuevo
                if (oRentaMaq.Id == 0)
                {
                    bd.RentaMaqs.InsertOnSubmit(oRentaMaq);
                    bd.SubmitChanges();
                    nregistrosAfetados = 1;
                }
                else
                {
                    RentaMaq grd = bd.RentaMaqs.Where(p => p.Id.Equals(oRentaMaq.Id)).First();
                    grd.Nombre = oRentaMaq.Nombre;
                    grd.Color = oRentaMaq.Color;
                    grd.Marca = oRentaMaq.Marca;
                    grd.Modelo = oRentaMaq.Modelo;
                    grd.Precio = oRentaMaq.Precio;
                    grd.CombustibleId = oRentaMaq.CombustibleId;
                    grd.TipoMaqId = oRentaMaq.TipoMaqId;
                    grd.FechaRenta = oRentaMaq.FechaRenta;
                    grd.Habilitado = oRentaMaq.Habilitado;

                    bd.SubmitChanges();
                    nregistrosAfetados = 1;
                }
            }catch (Exception ex)
            {
                nregistrosAfetados = 0;
            }
            return nregistrosAfetados;
        }

        public ActionResult RptRentaMaq()
        {
            var reporte = new ReportClass();
            reporte.FileName = Server.MapPath("/Reportes/RptRentaMaq.rpt");

            //conexion para el reporte
            var coninfo = ReportesConexion.getConexion();
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
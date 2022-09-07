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
    public class VentaMaqController : Controller
    {
        // GET: VentaMaq
        public ActionResult Index()
        {
            return View();
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

        public JsonResult listarEstadoMaq()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.EstadoMaqs.Select(p => new
            {
                ID = p.Id,
                p.Descripcion
            });
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult listarVentaMaq()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.VentaMaqs.Where(p => p.Habilitado.Equals(1)).Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Color,
                p.Peso,
                p.Marca,
                p.Modelo,
                p.Precio,
                FechaVenta = ((DateTime)p.FechaVenta).ToShortDateString()
            }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscarVentaMaq(string nombre)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var buscar = bd.VentaMaqs.Where(p => p.Habilitado.Equals(1) && p.Nombre.Contains(nombre)).Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Color,
                p.Peso,
                p.Marca,
                p.Modelo,
                p.Precio,
                FechaVenta = ((DateTime)p.FechaVenta).ToShortDateString(),
                p.Habilitado
            }).ToList();
            return Json(buscar, JsonRequestBehavior.AllowGet);
        }

        public JsonResult mostrarInfo(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.VentaMaqs.Where(p => p.Id.Equals(id)).Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Color,
                p.Peso,
                p.Marca,
                p.Modelo,
                p.Precio,
                p.CombustibleId,
                p.TipoMaqId,
                FechaVenta = ((DateTime)p.FechaVenta).ToShortDateString(),
                p.Habilitado,
                p.EstadoId
            }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int guardar(VentaMaq oVentaMaq)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                //inserta si es nuevo
                if (oVentaMaq.Id == 0)
                {
                    bd.VentaMaqs.InsertOnSubmit(oVentaMaq);
                    bd.SubmitChanges();
                    nregistrosAfectados = 1;
                }
                //Edita si ya existe
                else
                {
                    VentaMaq obj = bd.VentaMaqs.Where(p => p.Id.Equals(oVentaMaq.Id)).First();
                    obj.Nombre = oVentaMaq.Nombre;
                    obj.Color = oVentaMaq.Color;
                    obj.Peso = oVentaMaq.Peso;
                    obj.Marca = oVentaMaq.Marca;
                    obj.Modelo = oVentaMaq.Modelo;
                    obj.CombustibleId = oVentaMaq.CombustibleId;
                    obj.TipoMaqId = oVentaMaq.TipoMaqId;
                    obj.FechaVenta = oVentaMaq.FechaVenta;
                    obj.Habilitado = oVentaMaq.Habilitado;
                    obj.EstadoId = oVentaMaq.EstadoId;

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
            int nregistrosAfectados = 0;
            try
            {
                VentaMaq oVentaMaq = bd.VentaMaqs.Where(p => p.Id.Equals(id)).First();
                oVentaMaq.Habilitado = 0;
                bd.SubmitChanges();
                nregistrosAfectados = 1;
            }
            catch (Exception ex)
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }

        public ActionResult RptVentaMaq()
        {
            var reporte = new ReportClass();
            reporte.FileName = Server.MapPath("/Reportes/RptVentaMaq.rpt");

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
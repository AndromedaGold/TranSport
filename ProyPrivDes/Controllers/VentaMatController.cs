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
    public class VentaMatController : Controller
    {
        // GET: VentaMat
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult listarVentaMat()
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.VentaMats.Where(p => p.Habilitado.Equals(1)).Select(p => new
            {
                p.Id,
                p.NombreMat,
                p.Cantidad,
                p.Cliente,
                p.Destino,
                p.Precio,
                FechaVent = ((DateTime)p.FechaVent).ToShortDateString()
            }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult mostrarInfo(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var lista = bd.VentaMats.Where(p => p.Id.Equals(id)).Select(p => new
            {
                p.Id,
                p.NombreMat,
                p.Cantidad,
                p.Cliente,
                p.Destino,
                p.Precio,
                FechaVent = ((DateTime)p.FechaVent).ToShortDateString(),
                p.Habilitado,
                p.IdRentaMaq,
                p.IdOperador
            }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int eliminar(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                VentaMat oVentaMat = bd.VentaMats.Where(p => p.Id.Equals(id)).First();
                oVentaMat.Habilitado = 0;
                bd.SubmitChanges();
                nregistrosAfectados = 1;
            }
            catch (Exception ex)
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }

        public JsonResult buscarVentaMat(string nombre)
        {
            ConexionDataContext bd = new ConexionDataContext();
            var buscar = bd.VentaMats.Where(p => p.Habilitado.Equals(1) && p.NombreMat.Contains(nombre)).Select(p => new
            {
                p.Id,
                p.NombreMat,
                p.Cantidad,
                p.Cliente,
                p.Destino,
                p.Precio,
                FechaVent = ((DateTime)p.FechaVent).ToShortDateString(),
            }).ToList();
            return Json(buscar, JsonRequestBehavior.AllowGet);
        }

        public int guardar(VentaMat oVentaMat)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                //inserta si es nuevo
                if (oVentaMat.Id == 0)
                {
                    bd.VentaMats.InsertOnSubmit(oVentaMat);
                    bd.SubmitChanges();
                    nregistrosAfectados = 1;
                }
                //Edita si ya existe
                else
                {
                    VentaMat ven = bd.VentaMats.Where(p => p.Id.Equals(oVentaMat.Id)).First();
                    ven.NombreMat = oVentaMat.NombreMat;
                    ven.Cantidad = oVentaMat.Cantidad;
                    ven.Cliente = oVentaMat.Cliente;
                    ven.Destino = oVentaMat.Destino;
                    ven.Precio = oVentaMat.Precio;                  
                    ven.FechaVent = oVentaMat.FechaVent;
                    ven.Habilitado = oVentaMat.Habilitado;
                    ven.IdRentaMaq = oVentaMat.IdRentaMaq;
                    ven.IdOperador = oVentaMat.IdOperador;

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

        public ActionResult RptVentaMat()
        {
            var reporte = new ReportClass();
            reporte.FileName = Server.MapPath("/Reportes/RptVentaMat.rpt");

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
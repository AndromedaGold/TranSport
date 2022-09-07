using ProyPrivDes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ProyPrivDes.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public int validarUsuario(string usuario, string contraseña)
        {
            //Si es igual a cero es un error
            int rpta = 0;
            try
            {
                using (ConexionDataContext bd = new ConexionDataContext())
                {
                    SHA256Managed sha = new SHA256Managed();
                    byte[] dataNoCifrada = Encoding.Default.GetBytes(contraseña);
                    byte[] dataCifrada = sha.ComputeHash(dataNoCifrada);
                    //Contraseña
                    string contraCifrada = BitConverter.ToString(dataCifrada).Replace("-", "");

                    rpta = bd.Usuarios.Where(p => p.NombreUsuario == usuario && p.Contra == contraCifrada).Count();
                    if (rpta == 1)
                    {
                        int idusuario = bd.Usuarios.Where(p => p.NombreUsuario == usuario && p.Contra == contraCifrada).First().IdUsuario;
                        Session["idusuario"] = idusuario;

                        var roles = from usu in bd.Usuarios
                                    join rol in bd.Rols
                                    on usu.IdRol equals rol.IdRol
                                    join rolpagina in bd.RolPaginas
                                    on rol.IdRol equals rolpagina.IdRol
                                    join pagina in bd.Paginas
                                    on rolpagina.IdPagina equals pagina.IdPagina
                                    where usu.Habilitado == 1 && rolpagina.Habilitado == 1
                                    && usu.IdUsuario == idusuario
                                    select new
                                    {
                                        accion = pagina.Accion,
                                        controlador = pagina.Controlador,
                                        mensaje = pagina.Mensaje
                                    };
                        //Iniciando
                        Variable.acciones = new List<string>();
                        Variable.controladores = new List<string>();
                        Variable.mensajes = new List<string>();

                        //Llenando valores
                        foreach (var item in roles)
                        {
                            Variable.acciones.Add(item.accion);
                            Variable.controladores.Add(item.controlador);
                            Variable.mensajes.Add(item.mensaje);
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

        public ActionResult Cerrar()
        {
            return RedirectToAction("Index");
        }
    }
}
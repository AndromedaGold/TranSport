using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyPrivDes.Models;
using System.Transactions;
using System.Security.Cryptography;
using System.Text;
using ProyPrivDes.Filtros;

namespace ProyPrivDes.Controllers
{
    [Seguridad]
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult listarRol()
        {
            using(ConexionDataContext bd = new ConexionDataContext())
            {
                var lista = bd.Rols.Where(p => p.Habilitado == 1).Select(p => new
                {
                    ID = p.IdRol,
                    p.Nombre
                }).ToList();
                return Json(lista, JsonRequestBehavior.AllowGet);
            }       
        }

        public JsonResult listarPersonas()
        {
            List<Personas> listarPersona = new List<Personas>();
            
            using (ConexionDataContext bd = new ConexionDataContext())
            {
                List<Personas> listaOperador = (from item in bd.Operadors
                                               where item.bTieneUsuario == 0
                                               select new Personas
                                               {
                                                   ID = item.Id,
                                                   Nombre = item.Nombre + " " + item.Apellido + " (O)"
                                               }).ToList();
                listarPersona.AddRange(listaOperador);
                var listaSupervisor = (from item in bd.Coordinadors
                                       where item.bTieneUsuario == 0
                                       select new Personas
                                       {
                                           ID = item.Id,
                                           Nombre = item.Nombre + " " + item.Apellido + " (C)"
                                       }).ToList();
                listarPersona.AddRange(listaSupervisor);
                listarPersona = listarPersona.OrderBy(p => p.Nombre).ToList();
                return Json(listarPersona, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult listarUsuarios()
        {
            List<Usuarios> listaUsuario = new List<Usuarios>();
            using (ConexionDataContext bd = new ConexionDataContext())
            {
                List<Usuarios> listaOperador = (from usuario in bd.Usuarios
                                               join operador in bd.Operadors
                                               on usuario.Id equals operador.Id
                                               join rol in bd.Rols
                                               on usuario.IdRol equals rol.IdRol
                                               where usuario.Habilitado == 1 && usuario.TipoUsuario == 'O'
                                               select new Usuarios
                                               {
                                                   idUsuario = usuario.IdUsuario,
                                                   nombrePersona = operador.Nombre + " " + operador.Apellido,
                                                   nombreUsuario = usuario.NombreUsuario,
                                                   nombreRol = rol.Nombre,
                                                   nombreTipoEmpleado = "Operador"
                                               }).ToList();
                listaUsuario.AddRange(listaOperador);
                List<Usuarios> listaCoordinador = (from usuario in bd.Usuarios
                                                  join coordinador in bd.Coordinadors
                                                  on usuario.Id equals coordinador.Id
                                                  join rol in bd.Rols
                                                  on usuario.IdRol equals rol.IdRol
                                                  where usuario.Habilitado == 1 && usuario.TipoUsuario == 'C'
                                                  select new Usuarios
                                                  {
                                                      idUsuario = usuario.IdUsuario,
                                                      nombrePersona = coordinador.Nombre + " " + coordinador.Apellido,
                                                      nombreUsuario = usuario.NombreUsuario,
                                                      nombreRol = rol.Nombre,
                                                      nombreTipoEmpleado = "Coordinador"
                                                  }).ToList();
                listaUsuario.AddRange(listaCoordinador);
                listaUsuario = listaUsuario.OrderBy(p => p.idUsuario).ToList();
            }
            return Json(listaUsuario, JsonRequestBehavior.AllowGet);
        }

        public int guardarDatos(Usuario oUsuario, string nombreCompleto)
        {
            int rpta = 0;
            try
            {
                int idusuario = oUsuario.IdUsuario;
                using (ConexionDataContext bd = new ConexionDataContext())
                {
                    using (var transaccion = new TransactionScope())
                    {
                        if (idusuario == 0)
                        {
                            //Cifrar contraseña
                            string clave = oUsuario.Contra;
                            SHA256Managed sha = new SHA256Managed();
                            byte[] dataNoCifrada = Encoding.Default.GetBytes(clave);
                            byte[] dataCifrada = sha.ComputeHash(dataNoCifrada);
                            //Contraseña
                            oUsuario.Contra = BitConverter.ToString(dataCifrada).Replace("-", "");

                            //Escogemos usuario segun su tipo y lo eliminamos de la lista
                            char tipo = char.Parse(nombreCompleto.Substring(nombreCompleto.Length - 2, 1));
                            oUsuario.TipoUsuario = tipo;
                            bd.Usuarios.InsertOnSubmit(oUsuario);

                            if (tipo.Equals('O'))
                            {
                                Operador oOperador = bd.Operadors.Where(p => p.Id == oUsuario.Id).First();
                                oOperador.bTieneUsuario = 1;
                            }
                            else
                            {
                                Coordinador oCoordinador = bd.Coordinadors.Where(p => p.Id == oUsuario.Id).First();
                                oCoordinador.bTieneUsuario = 1;
                            }
                            bd.SubmitChanges();
                            transaccion.Complete();
                            rpta = 1;
                        }
                        else
                        {
                            Usuario oUsuarioCL = bd.Usuarios.Where(p => p.IdUsuario == idusuario).First();
                            oUsuarioCL.IdRol = oUsuario.IdRol;
                            oUsuarioCL.NombreUsuario = oUsuario.NombreUsuario;
                            bd.SubmitChanges();
                            transaccion.Complete();
                            rpta = 1;
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

        public JsonResult recuperarDatos(int idusuario)
        {
            using (ConexionDataContext bd = new ConexionDataContext())
            {

                var oUsuario = bd.Usuarios.Where(p => p.IdUsuario == idusuario).Select(p => new
                {
                    p.IdUsuario,
                    p.NombreUsuario,
                    p.IdRol
                }).First();
                return Json(oUsuario, JsonRequestBehavior.AllowGet);
            }
        }

        public int eliminar(int id)
        {
            ConexionDataContext bd = new ConexionDataContext();
            int nregistrosAfectados = 0;
            try
            {
                Usuario oUsuario = bd.Usuarios.Where(p => p.IdUsuario.Equals(id)).First();
                oUsuario.Habilitado = 0;
                bd.SubmitChanges();
                nregistrosAfectados = 1;
            }
            catch (Exception ex)
            {
                nregistrosAfectados = 0;
            }
            return nregistrosAfectados;
        }
    }
}
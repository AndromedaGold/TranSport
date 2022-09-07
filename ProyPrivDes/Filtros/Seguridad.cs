using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyPrivDes.Filtros
{
    public class Seguridad : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = HttpContext.Current.Session["idusuario"];

            //List<string> controladores = Variable.controladores.Select(p=> p.ToUpper()).ToList();
            //string nControlador = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            if (user == null /*|| !controladores.Contains(nControlador.ToUpper())*/)
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
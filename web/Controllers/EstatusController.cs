using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Models;
using System.Data.Entity;
using PagedList;

namespace web.Controllers
{
    public class EstatusController : OwnController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Estatus
        public ActionResult IndexAnticipo(int? page, string searchString)
        {
            var idUsuario = GetUserId(User);
            var anticipos = db.Anticipos.Where(a=>a.Eliminado != true && a.Viaje.IdUsuarioViaja == idUsuario).Include(a => a.Viaje.Usuario.Pais.Moneda);
            if (!String.IsNullOrEmpty(searchString))
            {
                anticipos = anticipos.Where(s => s.NoSolicitud.Contains(searchString)
                                       || s.Viaje.Viaje.Contains(searchString));
            }
            ViewBag.CurrentFilter = searchString;
            anticipos = anticipos.OrderByDescending(p => p.NoSolicitud);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(anticipos.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult IndexLiquidacion(int? page, string searchString)
        {
            var idUsuario = GetUserId(User);
            var liquidaciones = db.LiquidacionesViaje.Where(a => a.Eliminado != true && a.Viaje.IdUsuarioViaja == idUsuario).Include(a => a.Viaje.Usuario.Pais.Moneda).Include(a=>a.Moneda);
            if (!String.IsNullOrEmpty(searchString))
            {
                liquidaciones = liquidaciones.Where(s => s.NoSolicitud.Contains(searchString)
                                       || s.Viaje.Viaje.Contains(searchString));
            }
            ViewBag.CurrentFilter = searchString;
            liquidaciones = liquidaciones.OrderByDescending(p => p.NoSolicitud);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(liquidaciones.ToPagedList(pageNumber, pageSize));
        }

    }
}
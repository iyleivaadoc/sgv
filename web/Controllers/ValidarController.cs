using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using web.Models;
using PagedList;

namespace web.Controllers
{
    public class ValidarController : OwnController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Validar
        public ActionResult Index(int? page, string searchString)
        {
            var id = GetUserId(User);
            var userlog = db.Users.Find(id);
            var paises = db.JefesCreditoContabilidad.Where(j => j.IdJefeUsuario == id);
            if (paises.Count() > 0)
            {
                List<string> pais = new List<string>();
                foreach (var item in paises)
                {
                    pais.Add(item.IdPais.ToString());
                }
                var list = db.LiquidacionesViaje.Where(a => pais.Any(p => p == a.Viaje.Usuario.IdPais.ToString() && a.Eliminado != true && a.IdEstado == Estado.Terminado && a.TotalAnticipo > 0)).Include(l => l.Viaje.Usuario.Pais.Moneda).Include(a => a.Moneda);
                if (!String.IsNullOrEmpty(searchString))
                {
                    list = list.Where(s => s.NoSolicitud.Contains(searchString)
                                           || s.Viaje.Viaje.Contains(searchString));
                }
                ViewBag.CurrentFilter = searchString;
                list = list.OrderByDescending(p => p.NoSolicitud);
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(list.ToPagedList(pageNumber, pageSize));
            }

            return View();
        }

        public ActionResult ValidarLiq(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var liquidaciones = db.LiquidacionesViaje.Where(a => a.IdLiquidacionViaje == id && a.Eliminado != true).Include(l => l.DetallesLiquidacion).Include(l=>l.Moneda);

            if (liquidaciones.ToList().Count() > 0)
            {
                var liquidacion = liquidaciones.SingleOrDefault();
                ViewBag.IdMoneda = new SelectList(db.Moneda.Where(m => m.Eliminado != true), "IdMoneda", "MonedaCambio", liquidacion.IdMoneda);
                return View(liquidaciones.FirstOrDefault());
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }


        public ActionResult EditDet(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallesLiquidacion detallesLiquidacion = db.DetallesLiquidacion.Find(id);
            if (detallesLiquidacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.CuentaGasto = new SelectList(db.CuentasGasto, "IdCuentaGasto", "cuenta", detallesLiquidacion.CuentaGasto);
            ViewBag.IdLiquidacionViaje = new SelectList(db.LiquidacionesViaje, "IdLiquidacionViaje", "UsuarioCrea", detallesLiquidacion.IdLiquidacionViaje);
            ViewBag.CentroCosto = new SelectList(db.Users.Where(u => u.Nombres != "Administrador" && u.Apellidos != "Administrador"), "CentroCosto", "FullName", detallesLiquidacion.CentroCosto);
            return View(detallesLiquidacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDet(DetallesLiquidacion detalle)
        {
            detalle.UsuarioMod = GetUserId(User);
            detalle.FechaMod = DateTime.Now;
            db.Entry(detalle).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ValidarLiq", "Validar", new { id = detalle.IdLiquidacionViaje });
        }

        public ActionResult ValidadoLiq(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var liquidacion = db.LiquidacionesViaje.Where(a => a.IdLiquidacionViaje == id).Include(a => a.Viaje.Usuario.Pais.Moneda).SingleOrDefault();
            if (liquidacion.TotalAnticipo - (liquidacion.TotalAsignado * liquidacion.TasaCambio) < 0)
            {
                liquidacion.IdEstado = Estado.Validado;
            }
            else {
                liquidacion.IdEstado = Estado.Aprobado;
            }
            liquidacion.UsuarioMod = GetUserId(User);
            liquidacion.FechaMod = DateTime.Now;
            db.Entry(liquidacion).State = EntityState.Modified;
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Proceso finalizado.');</script>";
            db.SaveChanges();
            if ((liquidacion.TotalAnticipo - (liquidacion.TotalAsignado * liquidacion.TasaCambio) )< 0)
            {
                string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ValidacionAprobada.html");
                string readText2 = System.IO.File.ReadAllText(@"C:\FormatosCorreo\AprobarLiquidacion.html");
                readText2 = readText2.Replace("$$nombre##", liquidacion.Viaje.Usuario.FullName).Replace("$$monto##", (liquidacion.TotalAnticipo - (liquidacion.TotalAsignado * liquidacion.TasaCambio)).ToString(liquidacion.Viaje.Usuario.Pais.Moneda.First().Simbolo+"###,###.00"));
                var us = db.Users.Find(liquidacion.UsuarioAutoriza);

                if (!EnviarCorreo(liquidacion.Viaje.Usuario.Email, "Validación de liquidación aprobada", readText) || !EnviarCorreo(us.Email, "Aprobación de liquidación", readText2))
                {
                    Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor notificar al área de sistemas.');</script>";
                }
            }
            else {
                string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ValidacionAprobadaReintegroEmpresa.html");
                var us = db.Users.Find(liquidacion.UsuarioAutoriza);

                if (!EnviarCorreo(liquidacion.Viaje.Usuario.Email, "Validación de liquidación aprobada", readText))
                {
                    Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor notificar al área de sistemas.');</script>";
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult RechazadoLiq(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var liquidacion = db.LiquidacionesViaje.Where(a => a.IdLiquidacionViaje == id).Include(a => a.Viaje.Usuario).SingleOrDefault();
            liquidacion.IdEstado = Estado.Creado;
            liquidacion.UsuarioMod = GetUserId(User);
            liquidacion.FechaMod = DateTime.Now;
            db.Entry(liquidacion).State = EntityState.Modified;
            db.SaveChanges();
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Proceso finalizado.');</script>";
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ValidacionRechazada.html");
            if (!EnviarCorreo(liquidacion.Viaje.Usuario.Email, "Validación de liquidación rechazada", readText))
            {
                Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
            }
            return RedirectToAction("Index");
        }

    }
}
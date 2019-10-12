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
    public class AprobarController : OwnController
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Aprobar
        public ActionResult IndexAnticipo(int? page, string searchString)
        {
            var id = GetUserId(User);
            var us = db.Users.Where(u=>u.AprobadorSuplente==id);
            List<string> suplidos = new List<string>();
            foreach(var item in us)
            {
                suplidos.Add(item.Id);
            }
            var list = db.Anticipos.Where(a => (a.Eliminado != true && a.IdEstado == Estado.Terminado) && (a.UsuarioAutoriza == id || suplidos.Any(s=>s==a.UsuarioAutoriza)));
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
        public ActionResult AprobarDenegar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var anticipos = db.Anticipos.Where(a => a.IdAnticipo == id && a.Eliminado != true).Include(a => a.Viaje).Include(a => a.ConceptosAdicionales);
            ViewBag.Porcentaje = new List<SelectListItem>()
                                            {new SelectListItem() { Text = "25%", Value = "25" },
                                            new SelectListItem() { Text = "50%", Value = "50" },
                                            new SelectListItem() { Text = "75%", Value = "75" },
                                            new SelectListItem() { Text = "100%", Value = "100" }};
            if (anticipos.ToList().Count() > 0)
            {
                return View(anticipos.FirstOrDefault());
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }



        public ActionResult Aprobado(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var anticipo = db.Anticipos.Where(a => a.IdAnticipo == id).Include(a => a.Viaje.Usuario).SingleOrDefault();
            anticipo.IdEstado = Estado.Aprobado;
            anticipo.UsuarioMod = GetUserId(User);
            anticipo.FechaMod = DateTime.Now;
            db.Entry(anticipo).State = EntityState.Modified;
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Proceso finalizado.');</script>";
            db.SaveChanges();
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ViaticosAprobados.html");
            string readText2 = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ProcesarViatico.html");
            readText2 = readText2.Replace("$$nombre##", anticipo.Viaje.Usuario.FullName).Replace("$$$monto##", anticipo.TotalAnticipar.ToString("$###,###.00"));
            var asist = db.AsistenteTesoreria.Include(a => a.Asistente);
            List<string> to = new List<string>();
            foreach (var item in asist.ToList())
            {
                to.Add(item.Asistente.Email);
            }
            if (!EnviarCorreo(anticipo.Viaje.Usuario.Email, "Anticipo aprobado", readText) || !EnviarCorreo(to, "Procesar anticipo", readText2))
            {
                Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
            }
            return RedirectToAction("IndexAnticipo");
        }

        public ActionResult Denegado(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var anticipo = db.Anticipos.Where(a => a.IdAnticipo == id).Include(a => a.Viaje.Usuario).SingleOrDefault();
            anticipo.IdEstado = Estado.Creado;
            anticipo.UsuarioMod = GetUserId(User);
            anticipo.FechaMod = DateTime.Now;
            db.Entry(anticipo).State = EntityState.Modified;
            db.SaveChanges();
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Proceso finalizado.');</script>";
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ViaticosRechazados.html");
            if (!EnviarCorreo(anticipo.Viaje.Usuario.Email, "Anticipo rechazado", readText))
            {
                Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
            }
            return RedirectToAction("IndexAnticipo");
        }

        public ActionResult IndexLiquidacion(int? page, string searchString)
        {
            var id = GetUserId(User);
            var us = db.Users.Where(u => u.AprobadorSuplente == id);
            List<string> suplidos = new List<string>();
            foreach (var item in us)
            {
                suplidos.Add(item.Id);
            }
            var list = db.LiquidacionesViaje.Where(a => ((a.Eliminado != true  && a.IdEstado == Estado.Terminado && a.TotalAnticipo == 0) && (a.UsuarioAutoriza == id || suplidos.Any(s => s == a.UsuarioAutoriza))) || ((a.Eliminado != true && a.IdEstado == Estado.Validado) && (a.UsuarioAutoriza == id || suplidos.Any(s => s == a.UsuarioAutoriza)))).Include(a => a.Moneda);
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


        public ActionResult AprobarDenegarLiq(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var liquidaciones = db.LiquidacionesViaje.Where(a => a.IdLiquidacionViaje == id && a.Eliminado != true).Include(l => l.Viaje).Include(l => l.DetallesLiquidacion);

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
            return RedirectToAction("AprobarDenegarLiq", "Aprobar", new { id = detalle.IdLiquidacionViaje });
        }

        public ActionResult AprobadoLiq(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var liquidacion = db.LiquidacionesViaje.Where(a => a.IdLiquidacionViaje == id).Include(a => a.Viaje.Usuario).SingleOrDefault();
            liquidacion.IdEstado = Estado.Aprobado;
            liquidacion.UsuarioMod = GetUserId(User);
            liquidacion.FechaMod = DateTime.Now;
            db.Entry(liquidacion).State = EntityState.Modified;
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Proceso finalizado.');</script>";
            db.SaveChanges();
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\LiquidacionAprobada.html");
            if (liquidacion.TotalAnticipo==0) {
                string readText2 = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ProcesarLiquidacion.html");
                readText2 = readText2.Replace("$$nombre##", liquidacion.Viaje.Usuario.FullName).Replace("$$$monto##", liquidacion.TotalAsignado.ToString("###,###.00"));
                var asist = db.AsistenteTesoreria.Include(a => a.Asistente);
                List<string> to = new List<string>();
                foreach(var item in asist.ToList())
                {
                    to.Add(item.Asistente.Email);
                }
                if (!EnviarCorreo(liquidacion.Viaje.Usuario.Email, "Liquidación aprobada", readText) || !EnviarCorreo(to, "Procesar liquidación", readText2))
                {
                    Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
                }
            }
            else
            {
                var jefe = db.JefesCreditoContabilidad.Where(j => j.IdPais == liquidacion.Viaje.Usuario.IdPais).FirstOrDefault();
                var us = db.Users.Find(jefe.IdJefeUsuario);
                string readText2 = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ProcesarLiquidacion.html");
                readText2 = readText2.Replace("$$nombre##", liquidacion.Viaje.Usuario.FullName).Replace("$$$monto##", (liquidacion.TotalAnticipo - liquidacion.TotalAsignado).ToString("$ ###,###.00"));

                if (!EnviarCorreo(liquidacion.Viaje.Usuario.Email, "Liquidación aprobada", readText) || !EnviarCorreo(us.Email, "Procesar liquidación", readText2))
                {
                    Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
                }
            }
            
            return RedirectToAction("IndexLiquidacion");
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
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\LiquidacionRechazada.html");
            if (!EnviarCorreo(liquidacion.Viaje.Usuario.Email, "Liquidación rechazada", readText))
            {
                Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
            }
            return RedirectToAction("IndexLiquidacion");
        }

    }
}
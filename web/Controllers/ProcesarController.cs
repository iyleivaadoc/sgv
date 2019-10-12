using ClosedXML.Excel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using web.Models;

namespace web.Controllers
{
    public class ProcesarController : OwnController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Procesar
        public ActionResult IndexAnticipo(int? page, string searchString)
        {
            var id = GetUserId(User);
            var list = db.Anticipos.Where(a => a.Eliminado != true && a.IdEstado == Estado.Aprobado);
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


        public ActionResult Finalizado(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var anticipo = db.Anticipos.Where(a => a.IdAnticipo == id).Include(a => a.Viaje.Usuario).SingleOrDefault();
            anticipo.IdEstado = Estado.Finalizado;
            anticipo.UsuarioMod = GetUserId(User);
            anticipo.FechaMod = DateTime.Now;
            db.Entry(anticipo).State = EntityState.Modified;
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Proceso finalizado.');</script>";
            db.SaveChanges();
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ViaticosProcesados.html");
            if (!EnviarCorreo(anticipo.Viaje.Usuario.Email, "Anticipo aprobado en procesamiento", readText))
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
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ViaticosRechazadosProsesamiento.html");
            if (!EnviarCorreo(anticipo.Viaje.Usuario.Email, "Anticipo rechazado en el procesamiento", readText))
            {
                Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
            }
            return RedirectToAction("IndexAnticipo");
        }


        public ActionResult IndexLiquidacion(int? page, string searchString)
        {
            var id = GetUserId(User);
            var list = db.LiquidacionesViaje.Where(a => a.Eliminado != true && a.IdEstado == Estado.Aprobado && a.TotalAnticipo > 0).Include(a => a.Moneda);
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
            return RedirectToAction("AprobarDenegarLiq", "Procesar", new { id = detalle.IdLiquidacionViaje });
        }

        public ActionResult AprobadoLiq(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var liquidacion = db.LiquidacionesViaje.Where(a => a.IdLiquidacionViaje == id).Include(a => a.Viaje.Usuario).SingleOrDefault();
            liquidacion.IdEstado = Estado.Finalizado;
            liquidacion.UsuarioMod = GetUserId(User);
            liquidacion.FechaMod = DateTime.Now;
            db.Entry(liquidacion).State = EntityState.Modified;
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Proceso finalizado.');</script>";
            db.SaveChanges();
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\LiquidacionProcesada.html");
            if (!EnviarCorreo(liquidacion.Viaje.Usuario.Email, "Liquidación Procesada", readText))
            {
                Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
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
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\LiquidacionRechazadaProcesar.html");
            if (!EnviarCorreo(liquidacion.Viaje.Usuario.Email, "Liquidación rechazada por contabilidad", readText))
            {
                Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
            }
            return RedirectToAction("IndexLiquidacion");
        }

        public ActionResult IndexLiquidacionTesoreria(int? page, string searchString)
        {
            var id = GetUserId(User);
            var list = db.LiquidacionesViaje.Where(a => a.Eliminado != true && a.IdEstado == Estado.Aprobado && a.TotalAnticipo == 0).Include(a => a.Moneda);
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

        public ActionResult AprobarDenegarLiqTesoreria(int? id)
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

        public ActionResult EditDetTesoreria(int? id)
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
        public ActionResult EditDetTesoreria(DetallesLiquidacion detalle)
        {
            detalle.UsuarioMod = GetUserId(User);
            detalle.FechaMod = DateTime.Now;
            db.Entry(detalle).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("AprobarDenegarLiqTesoreria", "Procesar", new { id = detalle.IdLiquidacionViaje });
        }

        public ActionResult AprobadoLiqTesoreria(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var liquidacion = db.LiquidacionesViaje.Where(a => a.IdLiquidacionViaje == id).Include(a => a.Viaje.Usuario).SingleOrDefault();
            liquidacion.IdEstado = Estado.Finalizado;
            liquidacion.UsuarioMod = GetUserId(User);
            liquidacion.FechaMod = DateTime.Now;
            db.Entry(liquidacion).State = EntityState.Modified;
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Proceso finalizado.');</script>";
            db.SaveChanges();
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\LiquidacionProcesadaTesoreria.html");
            if (!EnviarCorreo(liquidacion.Viaje.Usuario.Email, "Liquidación Procesada", readText))
            {
                Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
            }
            return RedirectToAction("IndexLiquidacionTesoreria");
        }

        public ActionResult RechazadoLiqTesoreria(int? id)
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
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\LiquidacionRechazadaProcesarTesoreria.html");
            if (!EnviarCorreo(liquidacion.Viaje.Usuario.Email, "Liquidación rechazada por contabilidad", readText))
            {
                Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
            }
            return RedirectToAction("IndexLiquidacionTesoreria");
        }


        public ActionResult GenerateExcel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XLWorkbook wb = new XLWorkbook();
            var worksheet = wb.Worksheets.Add("FB60");
            worksheet.Cell(1, 1).Value = "Cuenta de Mayor";
            worksheet.Cell(1, 4).Value = "Importe Moneda";
            worksheet.Cell(1, 10).Value = "Texto";
            worksheet.Cell(1, 16).Value = "Centro Costo";
            var liq= db.LiquidacionesViaje.Where(a => a.IdLiquidacionViaje == id && a.Eliminado != true).Include(l => l.Viaje).Include(l => l.DetallesLiquidacion).FirstOrDefault();
            int index = 2;
            foreach(var item in liq.DetallesLiquidacion)
            {
                worksheet.Cell(index, 1).Value = item.CuentaGasto;
                worksheet.Cell(index, 4).Value = (item.Monto * liq.TasaCambio).ToString("#####0.00");
                worksheet.Cell(index, 10).Value = item.ComentariosSolicitante;
                worksheet.Cell(index, 16).Value = liq.Viaje.Usuario.CentroCosto;
                index++;
            }
            return new ExcelResult(wb, liq.Viaje.Viaje+"-"+liq.Viaje.Usuario.UserName);
        }


    }
}
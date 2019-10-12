using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using web.Models;

namespace web.Controllers
{
    public class DetallesLiquidacionsController : OwnController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DetallesLiquidacions
        public ActionResult Index()
        {
            var detallesLiquidacion = db.DetallesLiquidacion.Include(d => d.LiquidacionViaje);
            return View(detallesLiquidacion.ToList());
        }

        // GET: DetallesLiquidacions/Details/5
        public ActionResult Details(int? id)
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
            return View(detallesLiquidacion);
        }

        // GET: DetallesLiquidacions/Create
        public ActionResult Create(int? idLiquidacionViaje, int? idViaje)
        {
            if (idLiquidacionViaje == null || idViaje == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var iduser = GetUserId(User);
            var usuario = db.Users.Find(iduser);
            ViewBag.CentroCosto= new SelectList( db.Users.Where(u => u.Nombres != "Administrador" && u.Apellidos!= "Administrador").OrderBy(u=>u.Nombres), "CentroCosto", "FullName",usuario.CentroCosto);
            ViewBag.idViaje = idViaje;
            DetallesLiquidacion det = new DetallesLiquidacion();
            var viaj = db.Viajes.Find(idViaje);
            det.FechaGasto = viaj.FechaInicio;
            var cuga = db.CuentasGasto.Where(c => c.CeCo == usuario.CentroCosto && c.IdClasificacion==viaj.ClasificacionViaje && c.IdPais==usuario.IdPais);
            ViewBag.CuentaGasto = new SelectList(cuga, "IdCuentaGasto", "cuenta");
            det.IdLiquidacionViaje = (int)idLiquidacionViaje;
            return View(det);
        }

        // POST: DetallesLiquidacions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DetallesLiquidacion detallesLiquidacion)
        {
            if (ModelState.IsValid)
            {
                detallesLiquidacion.UsuarioCrea = GetUserId(User);
                detallesLiquidacion.FechaCrea = DateTime.Now;
                db.DetallesLiquidacion.Add(detallesLiquidacion);
                var via = ViewBag.idViaje;
                db.SaveChanges();
                LiquidacionesViaje lv = db.LiquidacionesViaje.Find(detallesLiquidacion.IdLiquidacionViaje);
                lv.TotalAsignado += detallesLiquidacion.Monto;
                db.Entry(lv).State = EntityState.Modified;
                db.SaveChanges();
                Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Gasto Agregado');</script>";
                return RedirectToAction("Index", "LiquidacionesViajes",new { idViaje=lv.IdViaje});
            }
            var lvj = db.LiquidacionesViaje.Find(detallesLiquidacion.IdLiquidacionViaje);
            ViewBag.idViaje = lvj.IdViaje;
            ViewBag.CuentaGasto = new SelectList(db.CuentasGasto, "IdCuentaGasto", "cuenta",detallesLiquidacion.CuentaGasto);
            ViewBag.IdLiquidacionViaje = new SelectList(db.LiquidacionesViaje, "IdLiquidacionViaje", "UsuarioCrea", detallesLiquidacion.IdLiquidacionViaje);
            return View(detallesLiquidacion);
        }

        // GET: DetallesLiquidacions/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: DetallesLiquidacions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DetallesLiquidacion detallesLiquidacion)
        {
            if (ModelState.IsValid)
            {
                detallesLiquidacion.UsuarioMod = GetUserId(User);
                detallesLiquidacion.FechaMod = DateTime.Now;
                db.Entry(detallesLiquidacion).State = EntityState.Modified;
                db.SaveChanges();
                Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Gasto Editado');</script>";
                var lv = db.LiquidacionesViaje.Where(l=>l.IdLiquidacionViaje== detallesLiquidacion.IdLiquidacionViaje).Include(l=>l.DetallesLiquidacion).SingleOrDefault();
                var sumatoria = 0.0;
                foreach (var item in lv.DetallesLiquidacion.Where(dl => dl.IdDetalleLiquidacion != detallesLiquidacion.IdDetalleLiquidacion && dl.Eliminado!=true)){
                    sumatoria += item.Monto;
                }
                sumatoria += detallesLiquidacion.Monto;
                lv.TotalAsignado = sumatoria;
                db.Entry(lv).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "LiquidacionesViajes", new { idViaje = lv.IdViaje });
            }
            var lvj = db.LiquidacionesViaje.Find(detallesLiquidacion.IdLiquidacionViaje);
            detallesLiquidacion.LiquidacionViaje = lvj;
            ViewBag.CuentaGasto = new SelectList(db.CuentasGasto, "IdCuentaGasto", "cuenta", detallesLiquidacion.CuentaGasto);
            ViewBag.CentroCosto = new SelectList(db.Users.Where(u => u.Nombres != "Administrador" && u.Apellidos != "Administrador"), "CentroCosto", "FullName", detallesLiquidacion.CentroCosto);
            return View(detallesLiquidacion);
        }

        // GET: DetallesLiquidacions/Delete/5
        public ActionResult Delete(int? id)
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
            return View(detallesLiquidacion);
        }

        // POST: DetallesLiquidacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetallesLiquidacion detallesLiquidacion = db.DetallesLiquidacion.Find(id);
            detallesLiquidacion.Eliminado = true;
            detallesLiquidacion.UsuarioMod = GetUserId(User);
            detallesLiquidacion.FechaMod = DateTime.Now;
            db.Entry(detallesLiquidacion).State=EntityState.Modified;
            db.SaveChanges();
            LiquidacionesViaje lv = db.LiquidacionesViaje.Find(detallesLiquidacion.IdLiquidacionViaje);
            lv.TotalAsignado -= detallesLiquidacion.Monto;
            db.SaveChanges();
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Gasto Eliminado');</script>";
            return RedirectToAction("Index", "LiquidacionesViajes", new { idViaje = detallesLiquidacion.LiquidacionViaje.IdViaje });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

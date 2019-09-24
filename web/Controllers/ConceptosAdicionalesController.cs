using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using web.Models;

namespace web.Controllers
{
    public class ConceptosAdicionalesController : OwnController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ConceptosAdicionales
        public ActionResult Index()
        {
            var conceptosAdicionales = db.ConceptosAdicionales.Include(c => c.Anticipo);
            return View(conceptosAdicionales.ToList());
        }

        // GET: ConceptosAdicionales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConceptosAdicionales conceptosAdicionales = db.ConceptosAdicionales.Find(id);
            if (conceptosAdicionales == null)
            {
                return HttpNotFound();
            }
            return View(conceptosAdicionales);
        }

        // GET: ConceptosAdicionales/Create
        public ActionResult Create(int? idAnticipo, int? idViaje)
        {
            if (idAnticipo == null || idViaje==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                ViewBag.idAnticipoRet = idAnticipo;
                ViewBag.idViaje = idViaje;
                ViewBag.IdAnticipo = new SelectList(db.Anticipos, "IdAnticipo", "Porcentaje");
                ConceptosAdicionales ca = new ConceptosAdicionales();
                ca.IdAnticipo = (int)idAnticipo;
                return View(ca);
            }
        }

        // POST: ConceptosAdicionales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( ConceptosAdicionales conceptosAdicionales)
        {
            if (ModelState.IsValid)
            {
                db.ConceptosAdicionales.Add(conceptosAdicionales);
                db.SaveChanges();
                var s = db.ConceptosAdicionales.Where(c => c.IdConceptoAdicional == conceptosAdicionales.IdConceptoAdicional).Include(c=>c.Anticipo.Viaje).SingleOrDefault();
                Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Gasto Agregado');</script>";
                var anticipo = db.Anticipos.Find(s.IdAnticipo);
                anticipo.TotalAdicionales += conceptosAdicionales.Monto;
                anticipo.TotalViaje = anticipo.TotalAsignado + anticipo.TotalAdicionales;
                anticipo.TotalAnticipar = anticipo.TotalViaje * (anticipo.Porcentaje / 100.00);
                db.Entry(anticipo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index","Anticipos",new { idViaje = s.Anticipo.Viaje.IdViaje });
            }

            ViewBag.IdAnticipo = new SelectList(db.Anticipos, "IdAnticipo", "UsuarioCrea", conceptosAdicionales.IdAnticipo);
            return View(conceptosAdicionales);
        }

        // GET: ConceptosAdicionales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConceptosAdicionales conceptosAdicionales = db.ConceptosAdicionales.Find(id);
            if (conceptosAdicionales == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAnticipo = new SelectList(db.Anticipos, "IdAnticipo", "UsuarioCrea", conceptosAdicionales.IdAnticipo);
            return View(conceptosAdicionales);
        }

        // POST: ConceptosAdicionales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdConceptoAdicional,Concepto,Monto,IdAnticipo,UsuarioCrea,UsuarioMod,FechaCrea,FechaMod,Eliminado")] ConceptosAdicionales conceptosAdicionales)
        {
            if (ModelState.IsValid)
            {
                db.Entry(conceptosAdicionales).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdAnticipo = new SelectList(db.Anticipos, "IdAnticipo", "UsuarioCrea", conceptosAdicionales.IdAnticipo);
            return View(conceptosAdicionales);
        }

        // GET: ConceptosAdicionales/Delete/5
        public ActionResult Delete(int? id,int? idViaje)
        {
            if (id == null || idViaje==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.idViaje = idViaje;
            ConceptosAdicionales conceptosAdicionales = db.ConceptosAdicionales.Find(id);
            if (conceptosAdicionales == null)
            {
                return HttpNotFound();
            }
            return View(conceptosAdicionales);
        }

        // POST: ConceptosAdicionales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConceptosAdicionales conceptosAdicionales = db.ConceptosAdicionales.Find(id);
            conceptosAdicionales.Eliminado = true;
            conceptosAdicionales.UsuarioMod = GetUserId(User);
            conceptosAdicionales.FechaMod = DateTime.Now;
            db.Entry(conceptosAdicionales).State=EntityState.Modified;
            db.SaveChanges(); 
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Gasto eliminado');</script>";
            var anticipo = db.Anticipos.Find(conceptosAdicionales.IdAnticipo);
            anticipo.TotalAdicionales -= conceptosAdicionales.Monto;
            anticipo.TotalViaje = anticipo.TotalAsignado + anticipo.TotalAdicionales;
            anticipo.TotalAnticipar = anticipo.TotalViaje * (anticipo.Porcentaje / 100.00);
            db.Entry(anticipo).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index","Anticipos",new { idViaje=conceptosAdicionales.Anticipo.Viaje.IdViaje});
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

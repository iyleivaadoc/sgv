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
    public class MonedasController : OwnController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Monedas
        public ActionResult Index()
        {
            var moneda = db.Moneda.Where(m=>m.Eliminado!=true).Include(m => m.Pais);
            return View(moneda.ToList());
        }

        // GET: Monedas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Moneda moneda = db.Moneda.Where(m=>m.IdMoneda==id).Include(m=>m.Pais).SingleOrDefault();
            if (moneda == null)
            {
                return HttpNotFound();
            }
            return View(moneda);
        }

        // GET: Monedas/Create
        public ActionResult Create()
        {
            ViewBag.IdPais = new SelectList(db.Paises, "IdPais", "Pais");
            return View();
        }

        // POST: Monedas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Moneda moneda)
        {
            if (ModelState.IsValid)
            {
                moneda.FechaCrea = DateTime.Now;
                moneda.UsuarioCrea = GetUserId(User);
                db.Moneda.Add(moneda);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdPais = new SelectList(db.Paises, "IdPais", "Pais", moneda.IdPais);
            return View(moneda);
        }

        // GET: Monedas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Moneda moneda = db.Moneda.Find(id);
            if (moneda == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdPais = new SelectList(db.Paises, "IdPais", "Pais", moneda.IdPais);
            return View(moneda);
        }

        // POST: Monedas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Moneda moneda)
        {
            if (ModelState.IsValid)
            {
                moneda.UsuarioMod = GetUserId(User);
                moneda.FechaMod = DateTime.Now;
                db.Entry(moneda).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdPais = new SelectList(db.Paises, "IdPais", "Pais", moneda.IdPais);
            return View(moneda);
        }

        // GET: Monedas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Moneda moneda = db.Moneda.Where(m=>m.IdMoneda==id).Include(m=>m.Pais).SingleOrDefault();
            if (moneda == null)
            {
                return HttpNotFound();
            }
            return View(moneda);
        }

        // POST: Monedas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Moneda moneda = db.Moneda.Find(id);
            moneda.UsuarioMod = GetUserId(User);
            moneda.FechaMod = DateTime.Now;
            moneda.Eliminado = true;
            db.Entry(moneda).State=EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
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

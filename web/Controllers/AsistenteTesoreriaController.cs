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
    public class AsistenteTesoreriaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AsistenteTesoreria
        public ActionResult Index()
        {
            var asistenteTesoreria = db.AsistenteTesoreria.Include(a => a.Asistente);
            return View(asistenteTesoreria.ToList());
        }

        // GET: AsistenteTesoreria/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AsistenteTesoreria asistenteTesoreria = db.AsistenteTesoreria.Find(id);
            if (asistenteTesoreria == null)
            {
                return HttpNotFound();
            }
            return View(asistenteTesoreria);
        }

        // GET: AsistenteTesoreria/Create
        public ActionResult Create()
        {
            ViewBag.IdUsuario = new SelectList(db.Users, "Id", "Nombres");
            return View();
        }

        // POST: AsistenteTesoreria/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdAsistente,IdUsuario")] AsistenteTesoreria asistenteTesoreria)
        {
            if (ModelState.IsValid)
            {
                db.AsistenteTesoreria.Add(asistenteTesoreria);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUsuario = new SelectList(db.Users, "Id", "Nombres", asistenteTesoreria.IdUsuario);
            return View(asistenteTesoreria);
        }

        // GET: AsistenteTesoreria/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AsistenteTesoreria asistenteTesoreria = db.AsistenteTesoreria.Find(id);
            if (asistenteTesoreria == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUsuario = new SelectList(db.Users, "Id", "Nombres", asistenteTesoreria.IdUsuario);
            return View(asistenteTesoreria);
        }

        // POST: AsistenteTesoreria/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdAsistente,IdUsuario")] AsistenteTesoreria asistenteTesoreria)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asistenteTesoreria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUsuario = new SelectList(db.Users, "Id", "Nombres", asistenteTesoreria.IdUsuario);
            return View(asistenteTesoreria);
        }

        // GET: AsistenteTesoreria/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AsistenteTesoreria asistenteTesoreria = db.AsistenteTesoreria.Find(id);
            if (asistenteTesoreria == null)
            {
                return HttpNotFound();
            }
            return View(asistenteTesoreria);
        }

        // POST: AsistenteTesoreria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AsistenteTesoreria asistenteTesoreria = db.AsistenteTesoreria.Find(id);
            db.AsistenteTesoreria.Remove(asistenteTesoreria);
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

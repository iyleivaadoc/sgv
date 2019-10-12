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
    public class JefesCreditoContabilidadsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: JefesCreditoContabilidads
        public ActionResult Index()
        {
            var jefesCreditoContabilidad = db.JefesCreditoContabilidad.Include(j => j.JefeUsuario).Include(j => j.Pais);
            return View(jefesCreditoContabilidad.ToList());
        }

        // GET: JefesCreditoContabilidads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JefesCreditoContabilidad jefesCreditoContabilidad = db.JefesCreditoContabilidad.Find(id);
            if (jefesCreditoContabilidad == null)
            {
                return HttpNotFound();
            }
            return View(jefesCreditoContabilidad);
        }

        // GET: JefesCreditoContabilidads/Create
        public ActionResult Create()
        {
            ViewBag.IdJefeUsuario = new SelectList(db.Users, "Id", "Nombres");
            ViewBag.IdPais = new SelectList(db.Paises, "IdPais", "Pais");
            return View();
        }

        // POST: JefesCreditoContabilidads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdJefe,IdJefeUsuario,IdPais")] JefesCreditoContabilidad jefesCreditoContabilidad)
        {
            if (ModelState.IsValid)
            {
                db.JefesCreditoContabilidad.Add(jefesCreditoContabilidad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdJefeUsuario = new SelectList(db.Users, "Id", "Nombres", jefesCreditoContabilidad.IdJefeUsuario);
            ViewBag.IdPais = new SelectList(db.Paises, "IdPais", "Pais", jefesCreditoContabilidad.IdPais);
            return View(jefesCreditoContabilidad);
        }

        // GET: JefesCreditoContabilidads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JefesCreditoContabilidad jefesCreditoContabilidad = db.JefesCreditoContabilidad.Find(id);
            if (jefesCreditoContabilidad == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdJefeUsuario = new SelectList(db.Users, "Id", "Nombres", jefesCreditoContabilidad.IdJefeUsuario);
            ViewBag.IdPais = new SelectList(db.Paises, "IdPais", "Pais", jefesCreditoContabilidad.IdPais);
            return View(jefesCreditoContabilidad);
        }

        // POST: JefesCreditoContabilidads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdJefe,IdJefeUsuario,IdPais")] JefesCreditoContabilidad jefesCreditoContabilidad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jefesCreditoContabilidad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdJefeUsuario = new SelectList(db.Users, "Id", "Nombres", jefesCreditoContabilidad.IdJefeUsuario);
            ViewBag.IdPais = new SelectList(db.Paises, "IdPais", "Pais", jefesCreditoContabilidad.IdPais);
            return View(jefesCreditoContabilidad);
        }

        // GET: JefesCreditoContabilidads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JefesCreditoContabilidad jefesCreditoContabilidad = db.JefesCreditoContabilidad.Find(id);
            if (jefesCreditoContabilidad == null)
            {
                return HttpNotFound();
            }
            return View(jefesCreditoContabilidad);
        }

        // POST: JefesCreditoContabilidads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JefesCreditoContabilidad jefesCreditoContabilidad = db.JefesCreditoContabilidad.Find(id);
            db.JefesCreditoContabilidad.Remove(jefesCreditoContabilidad);
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

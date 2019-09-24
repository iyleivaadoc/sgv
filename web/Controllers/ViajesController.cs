using PagedList;
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
    public class ViajesController : OwnController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Viajes
        public ActionResult Index(int? page, string searchString)
        {
            var usuario = GetUserId(User);
            var viajes = db.Viajes.Where(v => v.Eliminado != true && v.IdUsuarioViaja == usuario).Include(v => v.Usuario);
            if (!String.IsNullOrEmpty(searchString))
            {
                viajes = viajes.Where(s => s.Viaje.Contains(searchString)
                                       || s.DescripcionViaje.Contains(searchString));
            }
            ViewBag.CurrentFilter = searchString;
            viajes = viajes.OrderByDescending(p => p.FechaInicio);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(viajes.ToPagedList(pageNumber, pageSize));
        }

        // GET: Viajes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Viajes viajes = db.Viajes.Find(id);
            if (viajes == null)
            {
                return HttpNotFound();
            }
            return View(viajes);
        }

        // GET: Viajes/Create
        public ActionResult Create()
        {
            ViewBag.IdUsuarioViaja = new SelectList(db.Users, "Id", "Nombres");
            ViewBag.IdPaisDestino = new SelectList(db.Paises, "IdPais", "Pais");
            ViewBag.IdPaisOrigen = new SelectList(db.Paises, "IdPais", "Pais");
            Viajes viaje = new Viajes();
            return View(viaje);
        }

        // POST: Viajes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Viajes viajes)
        {
            if (ModelState.IsValid)
            {
                viajes.UsuarioCrea = GetUserId(User);
                viajes.IdUsuarioViaja = viajes.UsuarioCrea;
                viajes.FechaCrea = DateTime.Now;
                db.Viajes.Add(viajes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdPaisDestino = new SelectList(db.Paises, "IdPais", "Pais", viajes.IdPaisDestino);
            ViewBag.IdPaisOrigen = new SelectList(db.Paises, "IdPais", "Pais", viajes.IdPaisOrigen);
            ViewBag.IdUsuarioViaja = new SelectList(db.Users, "Id", "Nombres", viajes.IdUsuarioViaja);
            return View(viajes);
        }

        // GET: Viajes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Viajes viajes = db.Viajes.Find(id);
            if (viajes == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUsuarioViaja = new SelectList(db.Users, "Id", "Nombres", viajes.IdUsuarioViaja);
            ViewBag.IdPaisDestino = new SelectList(db.Paises, "IdPais", "Pais", viajes.IdPaisDestino);
            ViewBag.IdPaisOrigen = new SelectList(db.Paises, "IdPais", "Pais", viajes.IdPaisOrigen);
            return View(viajes);
        }

        // POST: Viajes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Viajes viajes)
        {
            if (ModelState.IsValid)
            {
                viajes.UsuarioMod = GetUserId(User);
                viajes.FechaMod = DateTime.Now;
                db.Entry(viajes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUsuarioViaja = new SelectList(db.Users, "Id", "Nombres", viajes.IdUsuarioViaja);
            ViewBag.IdPaisDestino = new SelectList(db.Paises, "IdPais", "Pais", viajes.IdPaisDestino);
            ViewBag.IdPaisOrigen = new SelectList(db.Paises, "IdPais", "Pais", viajes.IdPaisOrigen);
            return View(viajes);
        }

        // GET: Viajes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Viajes viajes = db.Viajes.Find(id);
            if (viajes == null)
            {
                return HttpNotFound();
            }
            return View(viajes);
        }

        // POST: Viajes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Viajes viajes = db.Viajes.Find(id);
            viajes.Eliminado = true;
            viajes.UsuarioMod = GetUserId(User);
            viajes.FechaMod = DateTime.Now;
            db.Entry(viajes).State=EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Gestionar(int id)
        {
            db.Viajes.Find(id);
            return View(db.Viajes.Find(id));
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

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
    public class DepartamentosController : OwnController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Departamentos
        public ActionResult Index()
        {
            return View(db.Departamentos.Where(d=>d.Eliminado!=true).ToList());
        }

        // GET: Departamentos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamentos departamentos = db.Departamentos.Find(id);
            if (departamentos == null)
            {
                return HttpNotFound();
            }
            return View(departamentos);
        }

        // GET: Departamentos/Create
        public ActionResult Create()
        {
            ViewBag.IdPersonaACargo = new SelectList(db.Users.Where(u => u.Apellidos != "Administrador" && u.Nombres!="Administrador"),"Id","FullName");
            return View();
        }

        // POST: Departamentos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Departamentos departamentos)
        {
            if (ModelState.IsValid)
            {
                departamentos.FechaCrea = DateTime.Now;
                departamentos.FechaMod = departamentos.FechaCrea;
                var user = db.Users.Where(u => u.Id == departamentos.IdPersonaACargo).SingleOrDefault();
                departamentos.EmailNotificacion = user.Email;
                departamentos.PersonaACargo = user.FullName;
                departamentos.UsuarioCrea = GetUserId(User);
                db.Departamentos.Add(departamentos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdPersonaACargo = new SelectList(db.Users.Where(u => u.Apellidos != "Administrador" && u.Nombres != "Administrador"), "Id", "FullName");
            return View(departamentos);
        }

        // GET: Departamentos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamentos departamentos = db.Departamentos.Find(id);
            if (departamentos == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdPersonaACargo = new SelectList(db.Users.Where(u => u.Apellidos != "Administrador" && u.Nombres != "Administrador"), "Id", "FullName",departamentos.IdPersonaACargo);
            return View(departamentos);
        }

        // POST: Departamentos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Departamentos departamentos)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(u => u.Id == departamentos.IdPersonaACargo).SingleOrDefault();
                departamentos.EmailNotificacion = user.Email;
                departamentos.PersonaACargo = user.FullName;
                departamentos.UsuarioMod = GetUserId(User);
                departamentos.FechaMod = DateTime.Now;
                db.Entry(departamentos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdPersonaACargo = new SelectList(db.Users.Where(u => u.Apellidos != "Administrador" && u.Nombres != "Administrador"), "Id", "FullName",departamentos.IdPersonaACargo);
            return View(departamentos);
        }

        // GET: Departamentos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Departamentos departamentos = db.Departamentos.Find(id);
            if (departamentos == null)
            {
                return HttpNotFound();
            }
            return View(departamentos);
        }

        // POST: Departamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Departamentos departamentos = db.Departamentos.Find(id);
            departamentos.Eliminado = true;
            departamentos.UsuarioMod = GetUserId(User);
            departamentos.FechaMod = DateTime.Now;
            db.Entry(departamentos).State=EntityState.Modified;
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

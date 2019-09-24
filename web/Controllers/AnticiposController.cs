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
    public class AnticiposController : OwnController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Anticipos
        public ActionResult Index(int? idViaje)
        {

            if (idViaje == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var idusuario = GetUserId(User);
            ViewBag.usuario = UserManager.Users.Where(u => u.Id == idusuario).SingleOrDefault();
            var anticipos = db.Anticipos.Where(a => a.IdViaje == idViaje && a.Eliminado != true).Include(a => a.Viaje).Include(a => a.ConceptosAdicionales);
            ViewBag.Porcentaje = new List<SelectListItem>()
                                            {new SelectListItem() { Text = "25%", Value = "25" },
                                            new SelectListItem() { Text = "50%", Value = "50" },
                                            new SelectListItem() { Text = "75%", Value = "75" } };
            if (anticipos.ToList().Count() > 0)
            {
                return View(anticipos.FirstOrDefault());
            }
            var liquidacion = db.LiquidacionesViaje.Where(a => a.IdViaje == idViaje && a.Eliminado != true);
            if (liquidacion.ToList().Count() > 0)
            {
                Session["MyAlert"] = "<script type='text/javascript'>alertify.error('No se puede crear un anticipo porque ya hay una liquidación pendiente.');</script>";
                return RedirectToAction("Gestionar", "Viajes", new { id = idViaje });
            }
            else {
                Anticipos anticipo = new Anticipos();
                anticipo.Eliminado = false;
                anticipo.FechaCrea = DateTime.Now;
                anticipo.UsuarioCrea = GetUserId(User);
                anticipo.IdViaje = (int)idViaje;
                anticipo.IdEstado = Estado.Creado;
                anticipo.Porcentaje = 25;
                anticipo.TasaCambioApp = 0;
                anticipo.TotalAdicionales = 0;
                anticipo.TotalAsignado = 100;
                anticipo.TotalViaje = anticipo.TotalAsignado;
                anticipo.TotalAnticipar = anticipo.TotalViaje * (anticipo.Porcentaje / 100.00);
                db.Entry(anticipo).State = EntityState.Added;
                db.SaveChanges();
                anticipos = db.Anticipos.Where(a => a.IdViaje == idViaje && a.Eliminado != true).Include(a => a.Viaje).Include(a => a.ConceptosAdicionales);
                Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Se ha creado la solicitud exitosamente.');</script>";
                return View(anticipos.FirstOrDefault());
            }
        }

        // GET: Anticipos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Anticipos anticipos = db.Anticipos.Find(id);
            if (anticipos == null)
            {
                return HttpNotFound();
            }
            return View(anticipos);
        }

        // GET: Anticipos/Create
        public ActionResult Create()
        {
            ViewBag.IdViaje = new SelectList(db.Viajes, "IdViaje", "Viaje");
            return View();
        }

        // POST: Anticipos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdAnticipo,Porcentaje,TotalAnticipar,TotalAdicionales,TotalAsignado,TasaCambioApp,IdEstado,IdViaje,UsuarioCrea,UsuarioMod,FechaCrea,FechaMod,Eliminado")] Anticipos anticipos)
        {
            if (ModelState.IsValid)
            {
                db.Anticipos.Add(anticipos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdViaje = new SelectList(db.Viajes, "IdViaje", "Viaje", anticipos.IdViaje);
            return View(anticipos);
        }



        public ActionResult Enviar(int? id, int? idViaje) {
            if (id == null || idViaje == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var antc = db.Anticipos.Find(id);
            antc.IdEstado = Estado.Terminado;
            antc.UsuarioMod = GetUserId(User);
            antc.FechaMod = DateTime.Now;
            db.Entry(antc).State = EntityState.Modified;
            db.SaveChanges();
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Se ha enviado la solicitud exitosamente para su evaluación.');</script>";
            // aqui se debe enviar el correo a la persona que aprueba el anticipo.
            return RedirectToAction("index",new { idViaje=idViaje});
        }


        // GET: Anticipos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Anticipos anticipos = db.Anticipos.Find(id);
            if (anticipos == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdViaje = new SelectList(db.Viajes, "IdViaje", "Viaje", anticipos.IdViaje);
            return View(anticipos);
        }

        // POST: Anticipos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Anticipos anticipos)
        {
            if (ModelState.IsValid)
            {
                var anticipo = db.Anticipos.Find(anticipos.IdAnticipo);
                anticipo.Porcentaje = anticipos.Porcentaje;
                anticipo.UsuarioMod = GetUserId(User);
                anticipo.FechaMod = DateTime.Now;
                anticipo.TotalAnticipar = anticipo.TotalViaje * (anticipo.Porcentaje / 100.00);
                db.Entry(anticipo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index",new { idViaje=anticipo.IdViaje});
            }
            ViewBag.IdViaje = new SelectList(db.Viajes, "IdViaje", "Viaje", anticipos.IdViaje);
            return View(anticipos);
        }

        // GET: Anticipos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Anticipos anticipos = db.Anticipos.Find(id);
            if (anticipos == null)
            {
                return HttpNotFound();
            }
            return View(anticipos);
        }

        // POST: Anticipos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Anticipos anticipos = db.Anticipos.Find(id);
            anticipos.FechaMod = DateTime.Now;
            anticipos.UsuarioMod = GetUserId(User);
            anticipos.Eliminado = true;
            db.Entry(anticipos).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Gestionar", "viajes", new { id = anticipos.IdViaje });
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

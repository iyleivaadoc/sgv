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
                                            new SelectListItem() { Text = "75%", Value = "75" },
                                            new SelectListItem() { Text = "100%", Value = "100" }};
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
                var anticiposCorrelativo = db.Anticipos.Where(ac => ac.Viaje.Usuario.Id == anticipo.UsuarioCrea).ToList().AsReadOnly();
                var corre="00000"+(anticiposCorrelativo.Count()+1);
                anticipo.NoSolicitud = corre.Substring(corre.Length-5,5);
                var use = db.Users.Where(u => u.Id == anticipo.UsuarioCrea).Include(u => u.Departamento).SingleOrDefault();
                anticipo.UsuarioAutoriza = use.Departamento.IdPersonaACargo;
                var viaje = db.Viajes.Find(idViaje);
                anticipo.Porcentaje = 100;
                anticipo.TasaCambioApp = 0;
                anticipo.TotalAdicionales = 0;
                anticipo.TotalAsignado = totalAsignado(viaje.IdPaisOrigen, viaje.IdPaisDestino, use.Cargo, viaje.ClasificacionViaje) * viaje.Duracion;
                anticipo.TotalViaje = anticipo.TotalAsignado;
                anticipo.TotalAnticipar = anticipo.TotalViaje * (anticipo.Porcentaje / 100.00);
                db.Entry(anticipo).State = EntityState.Added;
                db.SaveChanges();
                anticipos = db.Anticipos.Where(a => a.IdViaje == idViaje && a.Eliminado != true).Include(a => a.Viaje).Include(a => a.ConceptosAdicionales);
                Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Se ha creado la solicitud exitosamente.');</script>";
                return View(anticipos.FirstOrDefault());
            }
        }

        public double totalAsignado(int origen, int destino, Cargo cargo,ClasificacionViaje clasificacion)
        {
            var totaldiario = 0.0;
            var gastos = db.GastosIniciales.Where(g => g.IdPaisOrigen == origen && g.IdPaisDestino == destino && g.IdCargo == cargo && g.IdClasificacionViaje==clasificacion);
            foreach(var item in gastos)
            {
                totaldiario += item.Gasto;
            }
            return totaldiario;
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
            var antc = db.Anticipos.Where(a=>a.IdAnticipo==id).Include(a=>a.Viaje.Usuario).SingleOrDefault();
            var anticipos = db.Anticipos.Where(an => an.Eliminado != true && an.IdEstado!= Estado.Finalizado && an.IdEstado != Estado.Creado && an.IdEstado != Estado.Bloqueado && an.Viaje.Usuario.Id==antc.Viaje.Usuario.Id);
            string readText = "";
            ApplicationUser autorizador;
            string subject = "";
            if (anticipos == null || anticipos.Count() > 0)
            {
                antc.IdEstado = Estado.Bloqueado;
                readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\DesbloquearViatico.html");
                readText = readText.Replace("$$nombre##", antc.Viaje.Usuario.FullName).Replace("$$$monto##", antc.TotalAnticipar.ToString("###,###.00"));
                var jefe = db.JefesCreditoContabilidad.Where(j => j.IdPais == antc.Viaje.Usuario.IdPais).FirstOrDefault();
                autorizador = db.Users.Find(jefe.IdJefeUsuario);
                subject = "Desbloqueo de viáticos";
            }
            else
            {
                antc.IdEstado = Estado.Terminado;
                readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\AprobarViatico.html");
                readText = readText.Replace("$$nombre##", antc.Viaje.Usuario.FullName).Replace("$$$monto##", antc.TotalAnticipar.ToString("###,###.00"));
                subject = "Aprobación de viáticos";
                autorizador = db.Users.Find(antc.UsuarioAutoriza);
            }
            antc.UsuarioMod = GetUserId(User);
            antc.FechaMod = DateTime.Now;
            db.Entry(antc).State = EntityState.Modified;
            db.SaveChanges();
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Se ha enviado la solicitud exitosamente para su evaluación.');</script>";
            if (antc.IdEstado == Estado.Bloqueado)
            {
                var readText2 = System.IO.File.ReadAllText(@"C:\FormatosCorreo\AnticipoBloqueado.html");
                if (!EnviarCorreo(autorizador.Email, subject, readText) || !EnviarCorreo(antc.Viaje.Usuario.Email, "Solicitud de viáticos bloqueada", readText2))
                {
                    Session["MyAlert"] += "   <script type='text/javascript'>alertify.error('No se pudo envíar notificación a su superior favor hacerlo verbalmente.');</script>";
                }
            }
            else
            {
                if (!EnviarCorreo(autorizador.Email, subject, readText))
                {
                    Session["MyAlert"] += "   <script type='text/javascript'>alertify.error('No se pudo envíar notificación a su superior favor hacerlo verbalmente.');</script>";
                }
            }

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

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
    public class DesbloquearController : OwnController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Desbloquear
        public ActionResult Index(int? page, string searchString)
        {
            var id = GetUserId(User);
            var userlog = db.Users.Find(id);
            var paises = db.JefesCreditoContabilidad.Where(j => j.IdJefeUsuario == id);
            if (paises.Count() > 0)
            {
                List<string> pais = new List<string>();
                foreach (var item in paises)
                {
                    pais.Add(item.IdPais.ToString());
                }
                var list = db.Anticipos.Where(a => pais.Any(p => p == a.Viaje.Usuario.IdPais.ToString() && a.Eliminado != true && a.IdEstado == Estado.Bloqueado));
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
            return View();
        }

        public ActionResult DesbloquearAnular(int? id)
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

        public ActionResult Desbloquear(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var anticipo = db.Anticipos.Where(a => a.IdAnticipo == id).Include(a => a.Viaje.Usuario).SingleOrDefault();
            anticipo.IdEstado = Estado.Terminado;
            anticipo.UsuarioMod = GetUserId(User);
            anticipo.FechaMod = DateTime.Now;
            db.Entry(anticipo).State = EntityState.Modified;
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Proceso finalizado.');</script>";
            db.SaveChanges();
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\AnticipoDesbloqueado.html");
            string readText2 = System.IO.File.ReadAllText(@"C:\FormatosCorreo\AprobarViatico.html");
            readText2 = readText2.Replace("$$nombre##", anticipo.Viaje.Usuario.FullName).Replace("$$$monto##", anticipo.TotalAnticipar.ToString("###,###.00"));
            var to = db.Users.Find(anticipo.UsuarioAutoriza);
            if (!EnviarCorreo(anticipo.Viaje.Usuario.Email, "Anticipo Desbloqueado", readText) || !EnviarCorreo(to.Email, "Aprobar anticipo", readText2))
            {
                Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var anticipo = db.Anticipos.Where(a => a.IdAnticipo == id).Include(a => a.Viaje.Usuario).SingleOrDefault();
            anticipo.Eliminado = true;
            anticipo.UsuarioMod = GetUserId(User);
            anticipo.FechaMod = DateTime.Now;
            db.Entry(anticipo).State = EntityState.Modified;
            db.SaveChanges();
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Proceso finalizado.');</script>";
            string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ViaticosEliminados.html");
            if (!EnviarCorreo(anticipo.Viaje.Usuario.Email, "Anticipo eliminado", readText))
            {
                Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('No se pudo envíar notificación favor repórtelo al área de sistemas.');</script>";
            }
            return RedirectToAction("Index");
        }


    }
}
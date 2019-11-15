using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using web.Models;
using web.ViewModels;

namespace web.Controllers
{
    public class SustitutoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Sustituto
        public ActionResult Index()
        {
            var deptos = db.Departamentos.Where(d=>d.Eliminado!=true);
            List<string> usuarios = new List<string>();
            foreach(var item in deptos)
            {
                usuarios.Add(item.IdPersonaACargo);
            }
            var list = db.Users.Where(u=>usuarios.Any(us=>us==u.Id));
            return View(list.Where(u=> u.Apellidos != "ADMINISTRADOR" && u.Nombres != "ADMINISTRADOR"));
        }
        public ActionResult AprobadorSustituto(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var us = db.Users.Find(id);
            ViewBag.Principal = new SelectList(db.Users.Where(u => u.Apellidos != "Administrador" && u.Nombres != "Administrador"), "Id", "FullName",id);
            ViewBag.Sustituto = new SelectList(db.Users.Where(u => u.Apellidos != "Administrador" && u.Nombres != "Administrador" && u.Id!=id), "Id", "FullName",us.AprobadorSuplente);
            AprobadorVM ap = new AprobadorVM();
            ap.Principal = id;
            return View(ap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AprobadorSustituto(AprobadorVM aprobador)
        {
            var us = db.Users.Find(aprobador.Principal);
            us.AprobadorSuplente = aprobador.Sustituto;
            db.Entry(us).State = EntityState.Modified;
            db.SaveChanges();
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Aprobador sustituto asignado exitosamente');</script>";
            return RedirectToAction("Index");
        }
    }
}
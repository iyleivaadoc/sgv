﻿using Microsoft.Ajax.Utilities;
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
    public class LiquidacionesViajesController : OwnController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LiquidacionesViajes
        public ActionResult Index(int? idViaje)
        {
            if (idViaje == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var idusuario = GetUserId(User);
            //ViewBag.usuario = UserManager.Users.Where(u => u.Id == idusuario).SingleOrDefault();
            var liquidacionesViaje = db.LiquidacionesViaje.Where(a => a.IdViaje == idViaje && a.Eliminado != true).Include(l => l.Viaje.Usuario.Pais.Moneda).Include(l=>l.DetallesLiquidacion).Include(l=>l.Moneda).SingleOrDefault();
            if (liquidacionesViaje != null)
            {
                ViewBag.IdMoneda = new SelectList(db.Moneda.Where(m => m.Eliminado != true).DistinctBy(m=>m.TasaCambio), "IdMoneda", "MonedaCambio", liquidacionesViaje.IdMoneda);
                return View(liquidacionesViaje);
            }else
            {
                LiquidacionesViaje lv = new LiquidacionesViaje();
                lv.TotalAsignado = 0;
                var ant = db.Anticipos.Where(a => a.Eliminado != true && a.IdEstado==Estado.Finalizado && a.IdViaje==idViaje).SingleOrDefault();
                if (ant != null)
                {
                    lv.TotalAnticipo = ant.TotalAnticipar;
                }
                else {
                    lv.TotalAnticipo = 0;
                }
                var viaje = db.Viajes.Where(v => v.IdViaje == idViaje).Include(v=>v.Destino.Moneda).Include(v=>v.Usuario.Departamento).FirstOrDefault();
                foreach(var item in viaje.Destino.Moneda.Where(m => m.Eliminado != true))
                {
                    lv.IdMoneda = item.IdMoneda;
                    lv.TasaCambio = item.TasaCambio;
                }
                lv.UsuarioCrea = GetUserId(User);
                var lvCorrelativo = db.LiquidacionesViaje.Where(ac => ac.Viaje.Usuario.Id == lv.UsuarioCrea).ToList().AsReadOnly();
                var corre = "00000" + (lvCorrelativo.Count() + 1);
                lv.NoSolicitud = corre.Substring(corre.Length - 5, 5);
                lv.UsuarioAutoriza = viaje.Usuario.Departamento.IdPersonaACargo;
                lv.Eliminado = false;
                lv.IdViaje = (int)idViaje;
                lv.IdEstado = Estado.Creado;
                
                lv.FechaCrea = DateTime.Now;
                db.Entry(lv).State = EntityState.Added;
                db.SaveChanges();
                Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Se creó la solicitud exitosamente.');</script>";
                liquidacionesViaje = db.LiquidacionesViaje.Where(a => a.IdViaje == idViaje && a.Eliminado != true).Include(l => l.Viaje.Usuario.Pais.Moneda).Include(l => l.DetallesLiquidacion).Include(l=>l.Moneda).SingleOrDefault();
                ViewBag.IdMoneda = new SelectList(db.Moneda.Where(m => m.Eliminado != true), "IdMoneda", "MonedaCambio", liquidacionesViaje.IdMoneda);
                return View(liquidacionesViaje);
            }

        }

        // GET: LiquidacionesViajes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiquidacionesViaje liquidacionesViaje = db.LiquidacionesViaje.Find(id);
            if (liquidacionesViaje == null)
            {
                return HttpNotFound();
            }
            return View(liquidacionesViaje);
        }

        public ActionResult Enviar(int? id, int? idViaje)
        {
            if (id == null || idViaje == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var lv = db.LiquidacionesViaje.Where(l=>l.IdLiquidacionViaje==id).Include(l=>l.Viaje.Usuario.Pais.Moneda).SingleOrDefault();
            lv.IdEstado = Estado.Terminado;
            lv.UsuarioMod = GetUserId(User);
            lv.FechaMod = DateTime.Now;
            db.Entry(lv).State = EntityState.Modified;
            db.SaveChanges();
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('Se ha enviado la solicitud de liquidación exitosamente para su evaluación.');</script>";
            if (lv.TotalAnticipo==0) {
                var us = db.Users.Find(lv.UsuarioAutoriza);
                string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\AprobarLiquidacion.html");
                readText=readText.Replace("$$nombre##", lv.Viaje.Usuario.FullName).Replace("$$monto##", (lv.TotalAsignado * lv.TasaCambio).ToString(lv.Viaje.Usuario.Pais.Moneda.First().Simbolo+"###,###.00"));
                if (!EnviarCorreo(us.Email, "Aprobación de liquidación", readText)) {
                    Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('no se pudo enviar la notificación al director de área, favor notifique a sistemas.');</script>";
                }
            }else
            {
                var jefe = db.JefesCreditoContabilidad.Where(j=>j.IdPais==lv.Viaje.Usuario.IdPais).FirstOrDefault();
                var us =db.Users.Find(jefe.IdJefeUsuario);
                string readText = System.IO.File.ReadAllText(@"C:\FormatosCorreo\ValidarLiquidacion.html");
                readText = readText.Replace("$$nombre##", lv.Viaje.Usuario.FullName).Replace("$$monto##", (lv.TotalAnticipo - (lv.TotalAsignado * lv.TasaCambio)).ToString(lv.Viaje.Usuario.Pais.Moneda.First().Simbolo+"###,###.00"));
                if (!EnviarCorreo(us.Email, "Validación de liquidación", readText))
                {
                    Session["MyAlert"] += "  <script type='text/javascript'>alertify.error('no se pudo enviar la notificación al director de área, favor notifique a sistemas.');</script>";
                }
            }

            return RedirectToAction("index", new { idViaje = idViaje });
        }



        // GET: LiquidacionesViajes/Create
        public ActionResult Create()
        {
            ViewBag.idViaje = new SelectList(db.Viajes, "IdViaje", "Viaje");
            return View();
        }

        // POST: LiquidacionesViajes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdLiquidacionViaje,TotalAsignado,TotalViaticos,TasaCambio,TotalAnticipo,IdEstado,idViaje,UsuarioCrea,UsuarioMod,FechaCrea,FechaMod,Eliminado")] LiquidacionesViaje liquidacionesViaje)
        {
            if (ModelState.IsValid)
            {
                db.LiquidacionesViaje.Add(liquidacionesViaje);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idViaje = new SelectList(db.Viajes, "IdViaje", "Viaje", liquidacionesViaje.IdViaje);
            return View(liquidacionesViaje);
        }

        // GET: LiquidacionesViajes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiquidacionesViaje liquidacionesViaje = db.LiquidacionesViaje.Find(id);
            if (liquidacionesViaje == null)
            {
                return HttpNotFound();
            }
            ViewBag.idViaje = new SelectList(db.Viajes, "IdViaje", "Viaje", liquidacionesViaje.IdViaje);
            return View(liquidacionesViaje);
        }

        // POST: LiquidacionesViajes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LiquidacionesViaje liquidacionesViaje)
        {
            if (ModelState.IsValid)
            {
                LiquidacionesViaje lv = db.LiquidacionesViaje.Where(a => a.IdLiquidacionViaje==liquidacionesViaje.IdLiquidacionViaje).Include(l => l.Viaje).Include(l => l.DetallesLiquidacion).SingleOrDefault();
                //LiquidacionesViaje lv = db.LiquidacionesViaje.Find(liquidacionesViaje.IdLiquidacionViaje);
                lv.IdMoneda = liquidacionesViaje.IdMoneda;
                lv.TasaCambio = liquidacionesViaje.TasaCambio;
                Moneda mon = db.Moneda.Find(lv.IdMoneda);
                //if((mon.TasaCambio*0.9)>liquidacionesViaje.TasaCambio || (mon.TasaCambio * 1.1) < liquidacionesViaje.TasaCambio)
                //{
                //    Session["MyAlert"] = "<script type='text/javascript'>alertify.error('La tasa de cambio varía del + o - 10% de la registrada.');</script>";
                //    ViewBag.IdMoneda = new SelectList(db.Moneda.Where(m => m.Eliminado != true), "IdMoneda", "MonedaCambio", liquidacionesViaje.IdMoneda);
                //    return View("Index",lv);
                //}
                lv.UsuarioMod = GetUserId(User);
                lv.FechaMod = DateTime.Now;
                db.Entry(lv).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index",new { idviaje = lv.IdViaje});
            }
            ViewBag.IdMoneda = new SelectList(db.Moneda.Where(m => m.Eliminado != true), "IdMoneda", "MonedaCambio", liquidacionesViaje.IdMoneda);
            ViewBag.idViaje = new SelectList(db.Viajes, "IdViaje", "Viaje", liquidacionesViaje.IdViaje);
            return View(liquidacionesViaje);
        }

        // GET: LiquidacionesViajes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiquidacionesViaje liquidacionesViaje = db.LiquidacionesViaje.Find(id);
            if (liquidacionesViaje == null)
            {
                return HttpNotFound();
            }
            return View(liquidacionesViaje);
        }

        // POST: LiquidacionesViajes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LiquidacionesViaje liquidacionesViaje = db.LiquidacionesViaje.Find(id);
            liquidacionesViaje.Eliminado = true;
            liquidacionesViaje.FechaMod = DateTime.Now;
            liquidacionesViaje.UsuarioMod = GetUserId(User);
            db.Entry(liquidacionesViaje).State=EntityState.Modified;
            db.SaveChanges();
            Session["MyAlert"] = "<script type='text/javascript'>alertify.success('La solicitud se elimino con éxito.');</script>";
            return RedirectToAction("Gestionar","Viajes", new { id = liquidacionesViaje.IdViaje });
        }


        public double MonedaCambio(int IdMoneda)
        {
            var m = db.Moneda.Find(IdMoneda);
            return m.TasaCambio;
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using web.Models;

namespace web.Controllers
{
    [Authorize(Roles ="Administrador")]
    public class AccesosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accesos
        public async Task<ActionResult> Index()
        {
            var ace = await db.Permisos.ToListAsync();
            var list = (from a in db.Accesos
                        select new
                        {
                            id_acceso = a.id_acceso,
                            Nombre = a.Nombre,
                            Control = a.Control,
                            Metodo = a.Metodo,
                            Tipo = a.Tipo,
                            AccesoPredecesor =
                            (from a1 in db.Accesos
                             where a1.id_acceso.ToString() == a.AccesoPredecesor
                            select a1.Nombre).FirstOrDefault()


                        }).ToList();

            List<Accesos> ac = new List<Accesos>();

            foreach (var item in list)
            {

                Accesos clr = new Accesos();
                clr.id_acceso = item.id_acceso;
                clr.Nombre = item.Nombre;
                clr.Control = item.Control;
                clr.Metodo = item.Metodo;
                clr.Tipo = item.Tipo;
                if (item.AccesoPredecesor == null)
                {
                    clr.AccesoPredecesor = "";
                }else
                {
                    clr.AccesoPredecesor = item.AccesoPredecesor.ToString();
                }
                
                ac.Add(clr);


            }

            return View(ac);
        }

        // GET: Accesos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = null, Text = "Seleccione..." });
            foreach (var acceso in db.Accesos.Where(x => x.AccesoPredecesor == null).Where(x => x.Tipo == false))
                list.Add(new SelectListItem() { Value = acceso.id_acceso.ToString(), Text = acceso.Nombre });
            ViewBag.Accesos = list;

            Accesos accesos = await db.Accesos.FindAsync(id);

            //ViewBag.Predecesor = accesos
            if (accesos == null)
            {
                return HttpNotFound();
            }
            return View(accesos);
        }

        // GET: Accesos/Create
        public ActionResult Create()
        {

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = null, Text = "Seleccione..." });
            foreach (var acceso in db.Accesos.Where(x => x.AccesoPredecesor == null).Where(x=> x.Tipo == false))
                list.Add(new SelectListItem() { Value = acceso.id_acceso.ToString(), Text = acceso.Nombre });
            
            ViewBag.Accesos = list;
            return View();
        }

        // POST: Accesos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id_acceso,Nombre,Control,Metodo,Tipo,AccesoPredecesor")] Accesos accesos)
        {

            if (ModelState.IsValid)
            {
                db.Accesos.Add(accesos);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        // GET: Accesos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accesos accesos = await db.Accesos.FindAsync(id);
            if (accesos == null)
            {
                return HttpNotFound();
            }
            else
            {
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Value = null, Text = "Seleccione..." });
                foreach (var acceso in db.Accesos.Where(x => x.AccesoPredecesor == null).Where(x => x.Tipo == false))
                    list.Add(new SelectListItem() { Value = acceso.id_acceso.ToString(), Text = acceso.Nombre });
                ViewBag.Accesos = list;
            }
            return View(accesos);
        }

        // POST: Accesos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id_acceso,Nombre,Control,Metodo,Tipo,AccesoPredecesor")] Accesos accesos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accesos).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(accesos);
        }

        // GET: Accesos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Accesos accesos = await db.Accesos.FindAsync(id);
            if (accesos == null)
            {
                return HttpNotFound();
            }
            return View(accesos);
        }

        // POST: Accesos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Accesos accesos = await db.Accesos.FindAsync(id);
            db.Accesos.Remove(accesos);
            await db.SaveChangesAsync();
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

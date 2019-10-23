using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using web.Models;
using web.ViewModels;

namespace web.Controllers
{
    [Authorize()]
    public class RoleController : Controller
    {
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private IdentityUserRole _userRole;

        public RoleController()
        {
        }

        public RoleController(ApplicationRoleManager roleManager, IdentityUserRole userRole)
        {
            RoleManager = roleManager;
            UserRole = userRole;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public IdentityUserRole UserRole
        {
            get
            {
                return _userRole ?? HttpContext.GetOwinContext().Get<IdentityUserRole>();
            }
            set
            {
                _userRole = value;
            }
        }


        // GET: Role
        public ActionResult Index()
        {
            //var aaaa = HttpContext.GetOwinContext().Get<ApplicationUserRoleManager>();
            List<RoleViewModel> list = new List<RoleViewModel>();
            foreach (var role in RoleManager.Roles.Where(x => x.Eliminado == false))
                list.Add(new RoleViewModel(role));
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel model)
        {
            var role = new ApplicationRole() { Name = model.Name };
            role.Descripcion = model.Descripcion;
            role.UsuarioCrea = this.GetUserId(User);
            role.UsuarioModifica = null;
            role.FechaCrea = DateTime.Now;
            role.FechaModifica = role.FechaCrea;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var x = await RoleManager.CreateAsync(role);
            if (!x.Succeeded)
            {
                foreach (var error in x.Errors)
                {
                    if (error.EndsWith("is already taken."))
                        ModelState.AddModelError("", "El Rol ya existe.");
                    else ModelState.AddModelError("", error);
                }
                return View();
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return View(new RoleViewModel(role));
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RoleViewModel model)
        {
            var role = await RoleManager.FindByIdAsync(model.Id);
            if (role.Name == "Administrador")
            {
                ModelState.AddModelError("", "El Rol Administrador no puede ser editado.");
                return View();
            }
            else
            {
                role.Name = model.Name;
                role.Descripcion = model.Descripcion;
                role.FechaModifica = DateTime.Now;
                role.UsuarioModifica = this.GetUserId(User);
                var x = await RoleManager.UpdateAsync(role);
                if (!x.Succeeded)
                {
                    foreach (var error in x.Errors)
                    {
                        if (error.EndsWith("is already taken."))
                            ModelState.AddModelError("", "El Rol ingresado ya existe, por lo que no se puede actualizar a este.");
                        else ModelState.AddModelError("", error);
                    }
                    return View();
                }
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> Details(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return View(new RoleViewModel(role));
        }
        public async Task<ActionResult> Delete(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return View(new RoleViewModel(role));
        }

        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role.Name == "Administrador")
            {
                ModelState.AddModelError("", "El Rol Administrador no puede ser eliminado.");
                return View("Delete", new RoleViewModel(role));
            }
            else
            {
                role.Eliminado = true;
                role.Name = role.Name + "_deleted_" + DateTime.Now;
                role.FechaModifica = DateTime.Now;
                role.UsuarioModifica = this.GetUserId(User);
                await RoleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }
        }

        public string GetUserId(IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim.Value;
        }

        public async Task<ActionResult> AsignarAccesos(string id)
        {

            if (id == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = await RoleManager.FindByIdAsync(id);
            web.ViewModels.PermisosVM model = new web.ViewModels.PermisosVM();
            model.RolV = role;

            //Obtengo listado de accesos
            var lista = (from a in db.Accesos
                         select new web.ViewModels.PermisosVM.AccesosVMList()
                         {
                             id_acceso = a.id_acceso,
                             Nombre = a.Nombre,
                             Selected = false
                         }).ToList();

            //Obtengo listado de accesos asignados al rol
            var lista2 = (from a in db.Accesos
                          join p in db.Permisos on a.id_acceso equals p.id_acceso
                          where p.Id == id
                          select new web.ViewModels.PermisosVM.AccesosVMList()
                          {
                              id_acceso = a.id_acceso,
                              Nombre = a.Nombre,
                              Selected = true
                          }).ToList();


            var objetosAComparar = from item in lista2
                                   select item.id_acceso;

            foreach (var item in lista)
            {

                if (objetosAComparar.Contains(item.id_acceso))
                {
                    item.Selected = true;
                }
            }

            model.AccesosDisp = lista.OrderByDescending(o => o.Selected).ToList();
            model.AccesosSelect = lista2;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarAccesos(web.ViewModels.PermisosVM permisos)
        {
            var Rol = permisos.RolV;
            var Permisos = permisos;
            var lista1 = permisos.AccesosDisp.ToList();

            int i = 0;
            foreach (var row in lista1)
            {
                if (row.Selected == true)
                {
                    permisos.AccesosSelect.Add(permisos.AccesosDisp[i]);
                }
                i++;

            }

            //Elimino de la tabla permisos
            var toDelete = db.Permisos.Where(a => a.Id == Rol.Id).ToList();

            foreach (var item in toDelete)
                db.Permisos.Remove(item);
            db.SaveChanges();

            //inserto los accesos seleccionados en la tabla permisos
            foreach (var pa in permisos.AccesosSelect)
            {
                Permisos per = new Permisos();
                per.id_acceso = pa.id_acceso;
                per.Id = Rol.Id;
                db.Permisos.Add(per);
                db.SaveChanges();
                i++;
            }
            
            return RedirectToAction("Index");

        }

    }
}
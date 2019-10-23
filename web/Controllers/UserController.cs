using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using web.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Helpers;

namespace web.Controllers
{
    [Authorize()]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationSignInManager _signInManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: User
        public ActionResult Index()
        {
            List<ApplicationUser> list = new List<ApplicationUser>();
            foreach (var user in UserManager.Users.Where(x=>x.Eliminado==false))
                list.Add(user);
            return View(list);
        }

        public async Task<ActionResult> Details(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            return View(user);
        }

        // GET: /Account/create
        [AllowAnonymous]
        public ActionResult Create()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.Where(x=>x.Eliminado==false))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;
            List<SelectListItem> list2 = new List<SelectListItem>();
            foreach (var role in db.Departamentos.Where(x => x.Eliminado == false))
                list2.Add(new SelectListItem() { Value = role.idDepartamento.ToString(), Text = role.NombreDepartamento });
            List<SelectListItem> paises = new List<SelectListItem>();
            foreach (var pais in db.Paises)
                paises.Add(new SelectListItem() { Value = pais.IdPais.ToString(), Text = pais.Pais });
            ViewBag.Roles = list;
            ViewBag.Departamentos = list2;
            ViewBag.paises = paises;

            return View();
        }
        //Comentario 1
        // POST: /Account/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, Nombres = model.Nombres,
                                                Apellidos = model.Apellidos,CodigoEmpleado=model.CodigoEmpleado,Cargo=model.Cargo,IdDepartamento=model.IdDepartamento,
                                                PhoneNumber=model.PhoneNumber,CentroCosto=model.CentroCosto,IdPais=model.IdPais};
                model.Password = model.Password == null ? "Q1w2e3r4t%" : model.Password;
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    result = await UserManager.AddToRoleAsync(user.Id, model.RoleName);
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "User");
                }
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var role in RoleManager.Roles.Where(r=>r.Eliminado==false))
                    list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
                ViewBag.Roles = list;
                List<SelectListItem> paises = new List<SelectListItem>();
                foreach (var pais in db.Paises)
                    paises.Add(new SelectListItem() { Value = pais.IdPais.ToString(), Text = pais.Pais });
                ViewBag.paises = paises;
                List<SelectListItem> list2 = new List<SelectListItem>();
                foreach (var role in db.Departamentos.Where(x => x.Eliminado == false))
                    list2.Add(new SelectListItem() { Value = role.idDepartamento.ToString(), Text = role.NombreDepartamento });
                ViewBag.Departamentos = list2;
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<ActionResult> Edit(string id)
        {
            List<SelectListItem> list2 = new List<SelectListItem>();
            foreach (var dpto in db.Departamentos.Where(x => x.Eliminado == false))
                list2.Add(new SelectListItem() { Value = dpto.idDepartamento.ToString(), Text = dpto.NombreDepartamento });
            ViewBag.Departamentos = list2;
            var user = await UserManager.FindByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ApplicationUser model)
        {
            var user = await UserManager.FindByIdAsync(model.Id);
            //var role = new ApplicationUser() { Id = model.Id, Email = model.Email,Nombres=model.Nombres,Apellidos=model.Apellidos, UserName=model.UserName };
            
            user.Nombres = model.Nombres;
            user.Apellidos = model.Apellidos;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.CodigoEmpleado = model.CodigoEmpleado;
            user.CentroCosto = model.CentroCosto;
            user.Cargo = model.Cargo;
            user.IdDepartamento = model.IdDepartamento;
            if (!ModelState.IsValid)
            {
                List<SelectListItem> list2 = new List<SelectListItem>();
                foreach (var dpto in db.Departamentos.Where(z => z.Eliminado == false))
                    list2.Add(new SelectListItem() { Value = dpto.idDepartamento.ToString(), Text = dpto.NombreDepartamento });
                ViewBag.Departamentos = list2;
                if(model.Nombres==null || model.Nombres.Equals(""))
                    ModelState.AddModelError("", "El nombre es requerido.");
                if(model.Apellidos==null || model.Apellidos.Equals(""))
                    ModelState.AddModelError("", "El Apellido es requerido.");
                if (model.Email == null || model.Email.Equals(""))
                    ModelState.AddModelError("", "El email es requerido.");
                if (model.PhoneNumber == null || model.PhoneNumber.Equals(""))
                    ModelState.AddModelError("", "El teléfono es requerido.");
                if (model.CodigoEmpleado == null || model.CodigoEmpleado.Equals(""))
                    ModelState.AddModelError("", "El código de empleado es requerido.");
                if (model.CentroCosto == null || model.CentroCosto.Equals(""))
                    ModelState.AddModelError("", "El centro de costos es requerido.");
                return View();
            }
                var x = UserManager.Update(user);
            if (!x.Succeeded)
            {
                foreach (var error in x.Errors)
                {
                     ModelState.AddModelError("", error.Replace("is already taken", "ya existe").Replace("cannot be null or empty.", "no puede estar vacío."));
                }
                List<SelectListItem> list2 = new List<SelectListItem>(); 

                foreach (var dpto in db.Departamentos.Where(z => z.Eliminado == false))
                    list2.Add(new SelectListItem() { Value = dpto.idDepartamento.ToString(), Text = dpto.NombreDepartamento });
                ViewBag.Departamentos = list2;
                return View();
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            return View(user);
        }

        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            user.Eliminado = true;
            user.UserName = user.UserName + "_deleted_" + DateTime.Now;
            var x=await UserManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }

       

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
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

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Replace("User Name","Usuario").Replace("Password","Contraseña").Replace("is already taken","ya existe").Replace("Name","Usuario"));
            }
        }

        //Métodos para asignar n roles al usuario
        public async Task<ActionResult> AsignarRoles(string id)
        {

            if (id == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);
            web.ViewModels.RolesUsuarioVM model = new web.ViewModels.RolesUsuarioVM();
            model.UsuarioV = user;

            //Obtengo listado de accesos
            var lista = (from a in RoleManager.Roles
                         select new web.ViewModels.RolesUsuarioVM.RolesVMList()
                         {
                             Id = a.Id,
                             Nombre = a.Name,
                             Selected = false
                         }).ToList();

            //Obtengo listado de accesos asignados al rol
            Models.ApplicationDbContext context = new ApplicationDbContext();
            ApplicationUser au = context.Users.First(u => u.Id == id);

            //web.ViewModels.RolesUsuarioVM list = new web.ViewModels.RolesUsuarioVM();

            foreach (IdentityUserRole role in au.Roles)
            {
                string RolId = role.RoleId;
                string RolName = context.Roles.First(r => r.Id == role.RoleId).Name;
                bool Select= true;
                model.RolSelect.Add( new web.ViewModels.RolesUsuarioVM.RolesVMList() {Id= RolId, Nombre = RolName, Selected=Select });
            }

            var objetosAComparar = from item in model.RolSelect select item.Id;

            foreach (var item in lista)
            {

                if (objetosAComparar.Contains(item.Id))
                {
                    item.Selected = true;
                }
            }

            model.RolDisp = lista.OrderByDescending(o => o.Selected).ToList();
            //model.AccesosSelect = lista2;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AsignarRoles(web.ViewModels.RolesUsuarioVM rolUsuario)
        {
            var User = rolUsuario.UsuarioV;
            var Roles = rolUsuario;
            //var lista1 = rolUsuario.RolDisp.ToList();

            int i = 0;
            foreach (var row in rolUsuario.RolDisp)
            {
                    rolUsuario.RolSelect.Add(rolUsuario.RolDisp[i]);
                    await UserManager.RemoveFromRoleAsync(User.Id, row.Nombre);
                i++;

            }

            i = 0;
            foreach (var row in rolUsuario.RolSelect)
            {
                if (row.Selected == true)
                {
                    await UserManager.AddToRoleAsync(User.Id, row.Nombre);
                }
                i++;

            }

            return RedirectToAction("Index");

        }
    }
}
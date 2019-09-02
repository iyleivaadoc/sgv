using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using web.Models;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;

namespace web.Controllers
{
    [Authorize(Roles ="Administrador")]
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
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, Nombres = model.Nombres, Apellidos = model.Apellidos };
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
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<ActionResult> Edit(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ApplicationUser model)
        {
            var user = await UserManager.FindByIdAsync(model.Id);
            var role = new ApplicationUser() { Id = model.Id, Email = model.Email,Nombres=model.Nombres,Apellidos=model.Apellidos, UserName=model.UserName };
            
            user.Nombres = model.Nombres;
            user.Apellidos = model.Apellidos;
            user.Email = model.Email;
            var x = UserManager.Update(user);
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
namespace web.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity.Owin;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;

    internal sealed class Configuration : DbMigrationsConfiguration<web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(web.Models.ApplicationDbContext context)
        {
            IdentityRole role = new IdentityRole();
            role.Name = "Administrador";
            context.Roles.Add(role);

            ApplicationUser user = new ApplicationUser();
            user.UserName = "Admin";
            user.LockoutEnabled = true;
            user.Nombres = "Administrador";
            user.Apellidos = "Administrador";
            user.PasswordHash = "ACssXeKYiIEeVMSCA+ecYFebhSVa18UGT/A+kbs8tP3Zh2XKfCadwxgJ2WQ0isILlg==";// 515Admin!
            user.SecurityStamp = "94d48f7c-93d8-4289-b73e-263370b63b62";
            user.Email = "admin@correo.com";
            user.Eliminado = true;
            context.Users.Add(user);
            context.SaveChanges();
            var usuario = context.Users.FirstOrDefault();
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(context);
            ApplicationUserManager man = new ApplicationUserManager(userStore);
            man.AddToRoles(usuario.Id,"Administrador");
            
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}

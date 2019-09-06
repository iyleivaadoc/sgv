using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "El apellido es requerido.")]
        public string Apellidos { get; set; }
        [Display(Name = "Código de empleado"), Required(ErrorMessage = "El código del empleado es requerido.")]
        public string CodigoEmpleado { get; set; }
        [Display(Name = "Cargo/Jerarquía")]
        public Cargo Cargo { get; set; }
        [Required(ErrorMessage = "El centro de costos es requerido.")]
        public string CentroCosto { get; set; }
        [ForeignKey("Departamento"), Display(Name = "Dirección reporta")]
        public int? IdDepartamento { get; set; }
        public bool Eliminado { get; set; }
        [Display(Name = "Dirección reporta")]
        public virtual Departamentos Departamento { get; set; }
        [Display(Name = "Número de teléfono"), Required(ErrorMessage = "El número de teléfono es requerido.")]
        override
        public string PhoneNumber
        { get; set; }
        [NotMapped]
        public string FullName
        {
            get
            {
                return Nombres + " " + Apellidos;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public static implicit operator ApplicationUser(IdentityResult v)
        {
            throw new NotImplementedException();
        }
    }

    public class ApplicationRole : IdentityRole
    {
        public string Descripcion { get; set; }
        [DefaultValue(false)]
        public bool Eliminado { get; set; }
        public string UsuarioCrea { get; set; }
        public string UsuarioModifica { get; set; }
        public DateTime FechaCrea { get; set; }

        public DateTime FechaModifica { get; set; }
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Persona> Persona { get; set; }
        public DbSet<Accesos> Accesos { get; set; }

        public DbSet<Permisos> Permisos { get; set; }
        public DbSet<Departamentos> Departamentos { get; set; }

    }
}
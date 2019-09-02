using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class RoleViewModel
    {
        public RoleViewModel() { }

        public RoleViewModel(ApplicationRole role) {
            Id = role.Id;
            Name = role.Name;
            Descripcion = role.Descripcion;
            UsuarioCrea = role.UsuarioCrea;
            UsuarioModifica = role.UsuarioModifica;
            FechaCrea = role.FechaCrea;
            FechaModifica = FechaModifica;
            Eliminado = role.Eliminado;
        }

        public string Id { get; set; }
        [Display(Name="Rol")]
        [Required(ErrorMessage ="El campo Rol no puede quedar vacío")]
        public string Name { get; set; }
        [Required(ErrorMessage = "El campo Descripción no puede quedar vacío")]
        [Display(Name="Descripción")]
        public string Descripcion { get; set; }
        public bool Eliminado { get; set; }
        public string UsuarioCrea { get; set; }
        public string UsuarioModifica { get; set; }
        public DateTime FechaCrea { get; set; }
        public DateTime FechaModifica { get; set; }

        public virtual ICollection<Permisos> Permisos { get; set; }

    }
}
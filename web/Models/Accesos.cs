using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.Models
{
    public class Accesos
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_acceso { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        [StringLength(50)]
        public String Nombre { get; set; }

        [Required]
        [Display(Name = "Controlador")]
        [StringLength(50)]
        public String Control { get; set; }

        [Required]
        [Display(Name = "Método")]
        [StringLength(50)]
        public String Metodo { get; set; }

        [Required]
        [Display(Name = "Último Nivel")]
        public Boolean Tipo { get; set; }

        //[Required]
        [Display(Name = "Opción Padre")]
        [StringLength(50)]
        public String AccesoPredecesor { get; set; }

        //[Required]
        public virtual ICollection<Permisos> Permisos { get; set; }

        //public virtual ICollection<Accesos> AccesosList { get; set; }


    }
}
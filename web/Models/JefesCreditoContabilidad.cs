using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class JefesCreditoContabilidad
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdJefe { get; set; }
        [ForeignKey("JefeUsuario")]
        public string IdJefeUsuario { get; set; }
        [ForeignKey("Pais")]
        public int IdPais { get; set; }
        public virtual ApplicationUser JefeUsuario { get; set; }
        public virtual Paises Pais { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.Models
{
    public class Permisos
    {

        [Key]//, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int id_permiso { get; set; [Column(Order = 0)]
        [Column(Order = 0)]
        [ForeignKey("Accesos")]
        public int id_acceso { get; set; }

        [Key]//, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        [ForeignKey("ApplicationRole")]
        [StringLength(128)]
        public String Id { get; set; }

        public virtual Accesos Accesos { get; set; }
        public virtual ApplicationRole ApplicationRole { get; set; }

    }
}
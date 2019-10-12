using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class AsistenteTesoreria
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAsistente { get; set; }
        [ForeignKey("Asistente")]
        public string IdUsuario { get; set; }
        public virtual ApplicationUser Asistente { get; set; }
    }
}
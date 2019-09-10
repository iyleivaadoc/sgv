using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class HuellaAuditoria
    {
        public string UsuarioCrea { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? FechaCrea { get; set; }
        public DateTime? FechaMod { get; set; }
        public bool Eliminado { get; set; }
    }
}
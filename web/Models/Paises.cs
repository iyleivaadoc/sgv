using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class Paises
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPais { get; set; }
        [Display(Name ="País"),Required(ErrorMessage ="Deebe ingresar el nombre del país.")]
        public string Pais { get; set; }
        public ICollection<Moneda> Moneda { get; set; }
    }
}
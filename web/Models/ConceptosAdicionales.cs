using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class ConceptosAdicionales:HuellaAuditoria
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdConceptoAdicional { get; set; }
        [Display(Name ="Concepto"),StringLength(128),Required(ErrorMessage ="Descripción de concepto requerida.")]
        public string Concepto { get; set; }
        [Display(Name ="Monto"),Required(ErrorMessage ="Monto requerido.")]
        public double Monto { get; set; }
        [ForeignKey("Anticipo")]
        public int IdAnticipo { get; set; }
        public virtual Anticipos Anticipo { get; set; }
    }
}
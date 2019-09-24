using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class Anticipos:HuellaAuditoria
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAnticipo { get; set; }
        [Display(Name ="Porcentaje de anticipo"), Range(0.1,100.0,ErrorMessage ="El porcentaje debe ser entre 0.1 y 100.")]
        public double Porcentaje { get; set; }
        [Display(Name ="Total anticipo")]
        public double TotalAnticipar { get; set; }
        [Display(Name ="Total Adicionales")]
        public double TotalAdicionales { get; set; }
        [Display(Name ="Total Asignado")]
        public double TotalAsignado { get; set; }
        [Display(Name ="Total")]
        public double TotalViaje { get; set; }
        [Display(Name ="Tasa de cambio")]
        public double TasaCambioApp { get; set; }
        public Estado IdEstado { get; set; }
        [ForeignKey("Viaje")]
        public int IdViaje { get; set; }
        public virtual Viajes Viaje { get; set; }
        public ICollection<ConceptosAdicionales> ConceptosAdicionales { get; set; }
    }
}
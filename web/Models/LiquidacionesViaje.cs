using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class LiquidacionesViaje:HuellaAuditoria
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdLiquidacionViaje { get; set; }
        [Display(Name ="Total Asignado")]
        public double TotalAsignado { get; set; }
        //[Display(Name ="Total Viáticos")]
        //public double TotalViaticos { get; set; }
        [Display(Name ="Tasa de cambio")]
        public double TasaCambio { get; set; }
        [Display(Name ="Total Anticipo")]
        public double TotalAnticipo { get; set; }
        public Estado IdEstado { get; set; }
        [ForeignKey("Viaje")]
        public int IdViaje { get; set; }
        [ForeignKey("Moneda")]
        public int IdMoneda { get; set; }
        public virtual Viajes Viaje { get; set; }
        public Moneda Moneda { get; set; }
        public ICollection<DetallesLiquidacion> DetallesLiquidacion { get; set; }
    }
}
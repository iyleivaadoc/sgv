using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class DetallesLiquidacion:HuellaAuditoria
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDetalleLiquidacion { get; set; }
        [Display(Name ="Fecha gasto"),Required(ErrorMessage ="La Fecha del gasto es requerida."), DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime FechaGasto { get; set; }
        [Display(Name ="Monto"),Required(ErrorMessage ="El Monto es requerido."),DisplayFormat(DataFormatString ="{0:N}")]
        public double Monto { get; set; }
        [Display(Name ="Centro de costo"),Required(ErrorMessage ="El centro de costos es requerido.")]
        public string CentroCosto { get; set; }
        [Display(Name ="Cuenta de gasto"),Required(ErrorMessage ="La cuenta de gasto es requerida.")]
        public string CuentaGasto { get; set; }
        [Display(Name ="Comentarios del solicitante.")]
        public string ComentariosSolicitante { get; set; }
        [Display(Name ="Comentarios de los aprobadores.")]
        public string ComentariosAprobador { get; set; }
        [ForeignKey("LiquidacionViaje")]
        public int IdLiquidacionViaje { get; set; }
        public virtual LiquidacionesViaje LiquidacionViaje { get; set; }
    }
}
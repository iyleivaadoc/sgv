using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class Viajes : HuellaAuditoria
    {
        public Viajes()
        {
            FechaInicio = DateTime.Now;
            FechaFin = FechaInicio.AddDays(1);
            Eliminado = false;
            ViaViaje = ViasViaje.Terrestre;
            ClasificacionViaje = ClasificacionViaje.Interior;

        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdViaje { get; set; }
        [Required(ErrorMessage = "Proprocione nombre al Proceso"), StringLength(128),Display(Name ="Proceso")]
        public string Viaje { get; set; }
        [Display(Name = "Descripción"), Required(ErrorMessage = "Proprocione una descripción"), StringLength(256)]
        public string DescripcionViaje { get; set; }
        [Required(ErrorMessage = "La Fecha de inicio es requerida.") Display(Name = "Inicio"), DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime FechaInicio { get; set; }
        [Required(ErrorMessage = "La Fecha de Finalización es requerida.") Display(Name = "Fin"), DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime FechaFin { get; set; }
        [Required(ErrorMessage = "La vía es requerida"), Display(Name = "Vía")]
        public ViasViaje ViaViaje { get; set; }
        [Required(ErrorMessage = "La clasificación es requerida"), Display(Name = "Clasificación")]
        public ClasificacionViaje ClasificacionViaje { get; set; }
        [StringLength(50),Display(Name ="Especifique")]
        public string ClasificacionOtro { get; set; }
        [ForeignKey("Usuario")]
        public string IdUsuarioViaja { get; set; }
        [ForeignKey("Origen"), Display(Name = "Origen"), Required(ErrorMessage = "El origen no ha sido establecido")]
        public int IdPaisOrigen { get; set; }
        [ForeignKey("Destino"), Display(Name = "Destino"), Required(ErrorMessage = "El destino no ha sido establecido")]
        public int IdPaisDestino { get; set; }
        public virtual ApplicationUser Usuario { get; set; }
        public virtual Paises Origen { get; set; }
        public virtual Paises Destino { get; set; }
        [NotMapped]
        public int Duracion
        {
            get
            {
                return (FechaFin-FechaInicio).Days + 1;
            }
        }

        public ICollection<LiquidacionesViaje> LiquidacionesViaje { get; set; }
        public ICollection<Anticipos> Anticipos { get; set; }
    }
}
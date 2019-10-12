using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using web.Models;

namespace web.Models
{
    public class GastosIniciales
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idGastosIniciales { get; set; }
        [StringLength(100)]
        public string Concepto { get; set; }
        public double Gasto { get; set; }
        public Cargo IdCargo { get; set; }
        [ForeignKey("Destino")]
        public int IdPaisDestino { get; set; }
        [ForeignKey("Origen")]
        public int IdPaisOrigen { get; set; }
        public ClasificacionViaje IdClasificacionViaje { get; set; }
        public virtual Paises Destino { get; set; }
        public virtual Paises Origen { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class CuentasGasto
    {
        [Key, StringLength(10), Column(Order = 0)]
        public string IdCuentaGasto { get; set; }
        [StringLength(100)]
        public string cuenta { get; set; }
        [Key, StringLength(10), Column(Order = 1)]
        public string CeCo { get; set; }
        [ForeignKey("Pais"),Key, Column(Order = 2)]
        public int IdPais { get; set; }
        [Key,Column(Order = 3)]
        public ClasificacionViaje IdClasificacion { get; set; }
        public virtual Paises Pais { get; set; }
    }
}
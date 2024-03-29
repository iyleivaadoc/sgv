﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class Moneda:HuellaAuditoria
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMoneda { get; set; }
        [Required]
        public string MonedaCambio { get; set; }
        [StringLength(5)]
        public string Simbolo { get; set; }
        [Required]
        public double TasaCambio { get; set; }
        [ForeignKey("Pais")]
        public int IdPais { get; set; }
        public Paises Pais { get; set; }
    }
}
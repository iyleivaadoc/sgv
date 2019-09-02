using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace web.Models
{
    public class Persona

    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idPersona { get; set;}

        [Required]
        [Display(Name = "Nombre")]
        [StringLength(50)]
        public String Nombre { get; set;}

        [Required]
        [Display(Name = "Apellido")]
        [StringLength(50)]
        public String apellido { get; set;}

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fechaNacimiento { get; set; }



    }
}
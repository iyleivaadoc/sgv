using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using web.Models;

namespace web.ViewModels
{
    public class menuItemVM
    {
        public int id_acceso { get; set; }

        
        public String Nombre { get; set; }
        
        public String Control { get; set; }
        
        public String Metodo { get; set; }
        
        public Boolean Tipo { get; set; }

        public List<Accesos> hijos { get; set; }
    }
}
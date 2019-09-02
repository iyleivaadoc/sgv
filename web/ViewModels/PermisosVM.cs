using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using web.Models;

namespace web.ViewModels
{
    public class PermisosVM
    {
        public Permisos PermisosV { get; set; }

        public Accesos AccesoV { get; set; }

        public ApplicationRole RolV { get; set; }

        public class AccesosVMList
        {
            public int id_acceso { get; set; }
            public String Nombre { get; set; }
            public bool Selected { get; set; }
        }

        public List<AccesosVMList> AccesosSelect{ get; set; }

        public List<AccesosVMList> AccesosDisp { get; set; }

        public List<ApplicationRole> RoleList { get; set; }

        public PermisosVM()
        {
            RolV = new ApplicationRole();
            AccesosDisp = new List<AccesosVMList>();
            AccesosSelect = new List<AccesosVMList>();

        }

    }
}
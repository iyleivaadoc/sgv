using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using web.Models;

namespace web.ViewModels
{
    public class RolesUsuarioVM
    {
        //public ApplicationU PermisosV { get; set; }

        public ApplicationUser UsuarioV { get; set; }

        public ApplicationRole RolV { get; set; }

        public class RolesVMList
        {
            public String Id { get; set; }
            public String Nombre { get; set; }
            public bool Selected { get; set; }
        }

        public List<RolesVMList> RolSelect { get; set; }

        public List<RolesVMList> RolDisp { get; set; }

        public List<ApplicationRole> RoleList { get; set; }

        public RolesUsuarioVM()
        {
            UsuarioV = new ApplicationUser();
            RolDisp = new List<RolesVMList>();
            RolSelect = new List<RolesVMList>();

        }
    }
}
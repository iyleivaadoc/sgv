using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace web.Models
{
    public enum Cargo
    {
        [Description("Otro")]
        Otros=0,
        [Description("Gerente")]
        Gerente=1,
        [Description("Director")]
        Director =2,
        [Description("Jefe")]
        Jefe =3
    }

    public enum ViasViaje
    {
        [Description("Terrestre")]
        Terrestre=1,
        [Description("Aéreo")]
        Aéreo=2
    }

    public enum Estado
    {
        [Description("Creado")]
        Creado =1,
        [Description("Terminado")]
        Terminado =2,
        [Description("Bloqueado")]
        Bloqueado =3,
        [Description("Eliminado")]
        Eliminado =4
    }

    public enum ClasificacionViaje {
        [Description("Interior")]
        Interior =1,
        [Description("Exterior")]
        Exterior =2
    }

}
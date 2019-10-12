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
        Aéreo=2,
        [Description("Otra")]
        Otra=3
    }

    public enum Estado
    {
        [Description("Creado")]
        Creado =1,
        [Description("Terminado")]
        Terminado =2,
        [Description("Aprobado")]
        Aprobado = 3,
        [Description("Bloqueado")]
        Bloqueado =4,
        [Description("Eliminado")]
        Finalizado =5,
        [Description("Validado")]
        Validado=6
    }

    public enum ClasificacionViaje {
        [Description("Interior")]
        Interior = 1,
        [Description("Exterior")]
        Exterior = 2,
        [Description("Otra")]
        Otra = 3
    }

}
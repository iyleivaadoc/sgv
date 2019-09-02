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
}
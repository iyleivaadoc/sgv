﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web.Controllers
{
    public class CalendarioController : Controller
    {
        // GET: Calendario
        public ActionResult Index()
        {
            return View();
        }
    }
}
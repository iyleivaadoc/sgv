﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static web.Controllers.ManageController;

namespace web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.saludo = Resourses.Strings.saludo;
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Tu contraseña ha sido cambiada."
                : message == ManageMessageId.SetPasswordSuccess ? "Tu contraseña ha sido configurada."
                : message == ManageMessageId.Error ? "Ocurrió un error."
                : "";
            return View();
        }


        [HttpPost]
        public ActionResult EnviarCorreo()
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("irvinadoc@gmail.com");
            mail.To.Add("irvin.leiva@empresasadoc.com");
            mail.Subject = "correo de prueba";
            mail.Body = "Este es un mensaje de prueba";
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 25;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential("irvinadoc@gmail.com", "Empr3s4adoc");
            smtp.Send(mail);
            return View("Index");

        }
    }
}